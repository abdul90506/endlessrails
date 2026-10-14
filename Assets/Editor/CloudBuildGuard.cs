using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace EndlessRails.EditorTools
{
    /// <summary>
    /// Self-healing bootstrap for cloud builders (Unity Build Automation, CI).
    ///
    /// This project generates its materials, prefabs, ScriptableObject configs
    /// and the Main scene from code (AssetFactory / SceneBuilder). On a cloud
    /// builder the clone has no generated assets yet and no scene, so a default
    /// build would fail with "no scenes". This guard runs the full generation
    /// automatically during editor startup in batch mode - before any build
    /// entry point (e.g. UnityEditor.CloudBuild.Builder.Build) executes - and
    /// registers the generated scene in EditorBuildSettings.
    ///
    /// Locally (non-batch) it does nothing: use the Tools &gt; Endless Rails menu.
    /// </summary>
    [InitializeOnLoad]
    public static class CloudBuildGuard
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";
        private static bool _generated;

        static CloudBuildGuard()
        {
            if (!Application.isBatchMode)
            {
                return;
            }

            // 1) Try immediately (deterministic - runs before -executeMethod).
            TryGenerate("startup");

            // 2) Retry on the first editor update in case asset operations were
            //    not permitted during domain load. Both paths are idempotent and
            //    both run before Unity's build entry point in practice.
            EditorApplication.delayCall += () => TryGenerate("delayCall");

            // 3) Final safety net at build start (ensures scene list is set).
            //    Implemented via BuildPreprocessor below.
        }

        private static void TryGenerate(string phase)
        {
            if (_generated && EditorBuildSettings.scenes.Length > 0)
            {
                return;
            }

            try
            {
                Debug.Log($"[EndlessRails][CloudBuild] Generating project content ({phase})...");
                ProjectSetup.SetupProject();
                AssetFactory.GenerateAll();
                SceneBuilder.BuildScene();
                _generated = true;
                Debug.Log($"[EndlessRails][CloudBuild] Generation complete ({phase}).");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[EndlessRails][CloudBuild] Generation pass '{phase}' did not complete: {e.Message}");
            }
        }

        /// <summary>
        /// Build-time safety net: guarantees the generated scene is registered
        /// before Unity exports the player.
        /// </summary>
        private class BuildPreprocessor : IPreprocessBuildWithReport
        {
            public int callbackOrder => -1000;

            public void OnPreprocessBuild(BuildReport report)
            {
                if (!Application.isBatchMode)
                {
                    return;
                }

                TryGenerate("preprocessBuild");

                bool registered = false;
                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (scene.path == ScenePath)
                    {
                        registered = true;
                        break;
                    }
                }
                if (!registered)
                {
                    EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
                    Debug.Log("[EndlessRails][CloudBuild] Scene registered in EditorBuildSettings at build time.");
                }
            }
        }
    }
}
