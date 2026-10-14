using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace EndlessRails.EditorTools
{
    /// <summary>
    /// Android build entry points. Use the menu items for one-click builds, or
    /// run from the command line (see Tools/build_android.sh):
    ///
    ///   Unity -batchmode -quit -projectPath . ^
    ///         -executeMethod EndlessRails.EditorTools.BuildScript.BuildAndroidApk
    /// </summary>
    public static class BuildScript
    {
        private const string MenuRoot = "Tools/Endless Rails/";
        private const string OutputDir = "Builds/Android";
        private const string KeystorePath = "Tools/keystore/endlessrails-release.keystore";
        private const string KeystorePass = "endlessrails2026";
        private const string KeyAlias = "endlessrails";

        [MenuItem(MenuRoot + "4. Build Android APK (release)")]
        public static void BuildAndroidApkMenu()
        {
            BuildAndroidApk();
        }

        [MenuItem(MenuRoot + "5. Build Android AAB (Play Store)")]
        public static void BuildAndroidAabMenu()
        {
            BuildAndroidAab();
        }

        /// <summary>CLI entry point: produces a signed release APK.</summary>
        public static void BuildAndroidApk()
        {
            BuildAndroid(BuildOptions.None, "EndlessRails.apk", buildAppBundle: false);
        }

        /// <summary>
        /// All-in-one entry point for Unity Cloud Build Automation (or any CI):
        /// runs project setup, generates all assets/scene, then builds a signed APK.
        /// Set this as the "Export method" / custom build method in Build Automation:
        ///   EndlessRails.EditorTools.BuildScript.BuildAndroidCloud
        /// Honours the UNITY_EXPORT_PATH environment variable used by cloud builders.
        /// </summary>
        public static void BuildAndroidCloud()
        {
            ProjectSetup.SetupProject();
            AssetFactory.GenerateAll();
            SceneBuilder.BuildScene();
            BuildAndroidApk();
        }

        /// <summary>CLI entry point: produces a signed release AAB.</summary>
        public static void BuildAndroidAab()
        {
            BuildAndroid(BuildOptions.None, "EndlessRails.aab", buildAppBundle: true);
        }

        private static void BuildAndroid(BuildOptions options, string fileName, bool buildAppBundle)
        {
            // Re-apply critical settings so CLI builds are always correct.
            ProjectSetup.ConfigurePlayerSettings();

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

            ConfigureSigning();

            EditorUserBuildSettings.buildAppBundle = buildAppBundle;
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;

            // Cloud builders (Unity Build Automation) provide UNITY_EXPORT_PATH.
            string outputPath;
            string exportPath = Environment.GetEnvironmentVariable("UNITY_EXPORT_PATH");
            if (!string.IsNullOrEmpty(exportPath))
            {
                outputPath = exportPath;
            }
            else
            {
                Directory.CreateDirectory(OutputDir);
                outputPath = Path.Combine(OutputDir, fileName);
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = options
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[EndlessRails] Build succeeded: {outputPath} ({summary.totalSize / (1024 * 1024)} MB)");
            }
            else
            {
                Debug.LogError($"[EndlessRails] Build FAILED: {summary.result}");
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }
            }
        }

        private static string[] GetEnabledScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            if (scenes == null || scenes.Length == 0)
            {
                // Fallback: build from the generated Main scene.
                return new[] { "Assets/Scenes/Main.unity" };
            }
            var result = new System.Collections.Generic.List<string>();
            foreach (var scene in scenes)
            {
                if (scene.enabled)
                {
                    result.Add(scene.path);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Configures release signing. Prefers the bundled development keystore
        /// (generated via Tools/generate_keystore.sh); replace with your own
        /// production keystore before publishing!
        /// </summary>
        private static void ConfigureSigning()
        {
            string projectRoot = Directory.GetCurrentDirectory();
            string keystoreFullPath = Path.Combine(projectRoot, KeystorePath);

            if (!File.Exists(keystoreFullPath))
            {
                Debug.LogWarning($"[EndlessRails] Keystore not found at {KeystorePath}. " +
                                 "Building with debug signing. Run Tools/generate_keystore.sh or " +
                                 "supply your own keystore via Edit > Project Settings > Player > Publishing Settings.");
                return;
            }

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = KeystorePath;
            PlayerSettings.Android.keystorePass = KeystorePass;
            PlayerSettings.Android.keyaliasName = KeyAlias;
            PlayerSettings.Android.keyaliasPass = KeystorePass;
            Debug.Log("[EndlessRails] Release signing configured with the development keystore.");
        }

        /// <summary>Generates a fresh development keystore via keytool (called by shell script too).</summary>
        [MenuItem(MenuRoot + "6. Generate Development Keystore")]
        public static void GenerateKeystore()
        {
            string keystoreDir = Path.Combine(Directory.GetCurrentDirectory(), "Tools/keystore");
            Directory.CreateDirectory(keystoreDir);
            string keystorePath = Path.Combine(keystoreDir, "endlessrails-release.keystore");

            if (File.Exists(keystorePath))
            {
                Debug.Log("[EndlessRails] Keystore already exists: " + keystorePath);
                return;
            }

            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "keytool",
                Arguments = $"-genkeypair -v -keystore \"{keystorePath}\" -alias {KeyAlias} " +
                            $"-keyalg RSA -keysize 2048 -validity 10000 " +
                            $"-storepass {KeystorePass} -keypass {KeystorePass} " +
                            $"-dname \"CN=Endless Rails Development, OU=Games, O=EndlessRailsStudio, L=Islamabad, S=Islamabad, C=PK\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using var process = System.Diagnostics.Process.Start(startInfo);
                process.WaitForExit(30000);
                if (File.Exists(keystorePath))
                {
                    Debug.Log("[EndlessRails] Keystore generated at " + keystorePath);
                }
                else
                {
                    Debug.LogError("[EndlessRails] Keystore generation failed. Is keytool on PATH?");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EndlessRails] Could not run keytool: {e.Message}");
            }
        }
    }
}
