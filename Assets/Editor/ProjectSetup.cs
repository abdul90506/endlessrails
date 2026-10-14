using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace EndlessRails.EditorTools
{
    /// <summary>
    /// One-click project bootstrap: configures Android player settings, ensures
    /// URP is installed and assigned, sets quality tiers and verifies the
    /// generated placeholder assets are imported. Run once after opening the
    /// project:  Tools &gt; Endless Rails &gt; 1. Setup Project
    /// </summary>
    public static class ProjectSetup
    {
        private const string MenuRoot = "Tools/Endless Rails/";

        [MenuItem(MenuRoot + "1. Setup Project (do this first)")]
        public static void SetupProject()
        {
            Safe("EnsureTags", EnsureTags);
            Safe("ConfigurePlayerSettings", ConfigurePlayerSettings);
            Safe("EnsureUrp", EnsureUrp);
            Safe("ConfigureQuality", ConfigureQuality);
            Safe("ConfigureAudioImporterDefaults", ConfigureAudioImporterDefaults);
            Safe("ConfigureTextureImporterDefaults", ConfigureTextureImporterDefaults);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[EndlessRails] Project setup complete. Next: Tools > Endless Rails > 2. Generate Assets & Scene");
        }

        /// <summary>
        /// Runs a single setup step. A failure in one step never aborts the rest -
        /// important on cloud builders, so that asset/scene generation still runs
        /// even if (for example) URP setup throws an error.
        /// </summary>
        private static void Safe(string stepName, System.Action step)
        {
            try
            {
                step();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[EndlessRails] Setup step '{stepName}' failed (continuing): {e}");
            }
        }

        /// <summary>
        /// Registers every custom tag used by gameplay code. Hand-written
        /// TagManager.asset files are fragile (Unity's YAML parser is strict),
        /// so tags are created programmatically - idempotent and cloud-safe.
        /// </summary>
        public static void EnsureTags()
        {
            string[] requiredTags =
            {
                "Obstacle", "Coin", "Pickup", "Chaser", "Ground",
                "Train", "Vehicle", "Stumble", "FinishLine"
            };

            var existing = new HashSet<string>(UnityEditorInternal.InternalEditorUtility.tags);
            foreach (string tag in requiredTags)
            {
                if (!existing.Contains(tag))
                {
                    UnityEditorInternal.InternalEditorUtility.AddTag(tag);
                    Debug.Log($"[EndlessRails] Added tag: {tag}");
                }
            }
        }

        public static void ConfigurePlayerSettings()
        {
            PlayerSettings.companyName = "EndlessRailsStudio";
            PlayerSettings.productName = "Endless Rails";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.endlessrailsstudio.endlessrails");
            PlayerSettings.Android.bundleVersionCode = 1;

            // Android target: API 24+, arm64 only, IL2CPP + ARM64.
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            // Landscape-only game.
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            // Performance / graphics.
            PlayerSettings.graphicsJobs = true;
            PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, true);

            // No unnecessary permissions: only INTERNET is implied by nothing here,
            // we keep the manifest minimal (no camera/mic/location/etc.).
            PlayerSettings.Android.forceSDCardPermission = false;
            PlayerSettings.Android.forceInternetPermission = false;

            // Linear color space for PBR looks on modern devices.
            PlayerSettings.colorSpace = ColorSpace.Linear;

            // Legacy input handling (the project uses the built-in Input class).
            // Serialized as activeInputHandler in ProjectSettings.asset (0 = legacy).

            // Splash: keep the required Unity splash on Personal licenses.
            PlayerSettings.SplashScreen.show = true;

            Debug.Log("[EndlessRails] Android player settings configured (API 24+, ARM64, IL2CPP, landscape).");
        }

        private static void EnsureUrp()
        {
            // If URP is present, create pipeline assets and assign them.
            var urpAssetType = System.Type.GetType("UnityEditor.Rendering.Universal.UniversalRenderPipelineAssetEditor, Unity.RenderPipelines.Universal.Editor");
            var urpRuntimeType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset, Unity.RenderPipelines.Universal.Runtime");
            if (urpRuntimeType == null)
            {
                Debug.LogWarning("[EndlessRails] URP package is not installed yet. " +
                                 "Open Window > Package Manager, install 'Universal Render Pipeline', then re-run Setup.");
                return;
            }

            const string assetDir = "Assets/Settings";
            if (!AssetDatabase.IsValidFolder(assetDir))
            {
                AssetDatabase.CreateFolder("Assets", "Settings");
            }

            string pipelinePath = $"{assetDir}/URP-Pipeline.asset";
            UnityEngine.Object pipeline = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pipelinePath);
            if (pipeline == null)
            {
                var createMethod = urpRuntimeType.GetMethod("Create",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (createMethod != null)
                {
                    pipeline = createMethod.Invoke(null, null) as UnityEngine.Object;
                }
                else
                {
                    pipeline = ScriptableObject.CreateInstance(urpRuntimeType);
                }
                if (pipeline != null)
                {
                    AssetDatabase.CreateAsset(pipeline, pipelinePath);
                }
            }

            if (pipeline != null)
            {
                // Set a few URP quality knobs via serialized object (works across URP versions).
                var serialized = new SerializedObject(pipeline);
                SetSerialized(serialized, "m_ShadowDistance", 90f);
                SetSerialized(serialized, "m_MainLightShadowmapResolution", 2048);
                SetSerialized(serialized, "m_MSAA", 2);
                SetSerialized(serialized, "m_SupportsHDR", true);
                serialized.ApplyModifiedPropertiesWithoutUndo();

                // Unity 6 public API: assign the *default* render pipeline. Quality levels
                // without an override (the default in a fresh project) fall back to it.
                // QualitySettings.SetRenderPipelineAssetAt is NOT public API (CS0117 on Unity
                // 6000.0.32f1), so only the public property is used here.
                GraphicsSettings.defaultRenderPipeline = pipeline as RenderPipelineAsset;
                QualitySettings.renderPipeline = null;
                Debug.Log("[EndlessRails] URP pipeline asset created and assigned.");
            }
        }

        private static void SetSerialized(SerializedObject so, string property, object value)
        {
            SerializedProperty prop = so.FindProperty(property);
            if (prop == null)
            {
                return;
            }
            switch (value)
            {
                case float f: prop.floatValue = f; break;
                case int i: prop.intValue = i; break;
                case bool b: prop.boolValue = b; break;
            }
        }

        private static void ConfigureQuality()
        {
            // Three tiers for the settings dropdown: Low / Medium / High.
            int count = QualitySettings.names.Length;
            if (count >= 1)
            {
                QualitySettings.SetQualityLevel(0, false);
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.antiAliasing = 0;
                QualitySettings.pixelLightCount = 1;
                QualitySettings.lodBias = 0.7f;
            }
            if (count >= 2)
            {
                QualitySettings.SetQualityLevel(1, false);
                QualitySettings.shadows = ShadowQuality.HardOnly;
                QualitySettings.antiAliasing = 2;
                QualitySettings.pixelLightCount = 2;
                QualitySettings.lodBias = 1.0f;
            }
            if (count >= 3)
            {
                QualitySettings.SetQualityLevel(2, false);
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.antiAliasing = 2;
                QualitySettings.pixelLightCount = 3;
                QualitySettings.lodBias = 1.4f;
            }
            // Default to High for desktop preview.
            QualitySettings.SetQualityLevel(Mathf.Min(2, count - 1), true);
        }

        private static void ConfigureTextureImporterDefaults()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Textures/Generated" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetImporter.GetAtPath(path) is TextureImporter importer)
                {
                    bool isNormal = path.Contains("_normal");
                    bool isMask = path.Contains("_mask");

                    importer.textureType = isNormal ? TextureImporterType.NormalMap : TextureImporterType.Default;
                    importer.sRGBTexture = !isMask;
                    importer.mipmapEnabled = true;
                    importer.wrapMode = TextureWrapMode.Repeat;
                    importer.anisoLevel = 4;
                    importer.maxTextureSize = 1024;
                    importer.textureCompression = TextureImporterCompression.Compressed;
                    importer.SaveAndReimport();
                }
            }
        }

        private static void ConfigureAudioImporterDefaults()
        {
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio/Generated" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetImporter.GetAtPath(path) is AudioImporter importer)
                {
                    AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                    settings.loadType = AudioClipLoadType.DecompressOnLoad;
                    settings.compressionFormat = AudioCompressionFormat.Vorbis;
                    settings.quality = 0.72f;
                    settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
                    importer.defaultSampleSettings = settings;
                    importer.forceToMono = true;
                    importer.SaveAndReimport();
                }
            }
        }
    }
}
