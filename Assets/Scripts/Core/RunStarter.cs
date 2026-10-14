using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Scene bootstrap glue: starts world generation on run start, resets the
    /// world on restart, and shows the first-launch tutorial. Keeps GameManager
    /// decoupled from World/UI specifics.
    /// </summary>
    public class RunStarter : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private GameObject tutorialPanel;

        private void OnEnable()
        {
            GameEvents.OnRunStarted += HandleRunStarted;
            GameEvents.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            GameEvents.OnRunStarted -= HandleRunStarted;
            GameEvents.OnGameOver -= HandleGameOver;
        }

        private void Start()
        {
            // First-launch tutorial panel activation (UIManager decides which
            // panel is visible once the tutorial has been shown before).
            SaveSystem save = SaveSystem.Instance;
            bool showTutorial = save == null || !save.Data.tutorialShown;
            if (showTutorial && tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }

            // Show FPS if the player enabled it.
            if (save != null && save.Data.showFps)
            {
                EnableFpsText(true);
            }

            // Apply persisted audio settings.
            if (save != null)
            {
                AudioManager.Instance?.SetMusicVolume(save.Data.musicVolume);
                AudioManager.Instance?.SetSfxVolume(save.Data.sfxVolume);
                AudioManager.Instance?.SetAmbientVolume(save.Data.ambientVolume);
                SwipeInputManager.Instance?.SetSensitivity(save.Data.inputSensitivity);
                FindFirstObjectByType<FollowCamera>()?.SetFirstPerson(save.Data.firstPersonCamera);
            }

            // Reset the chaser for menu-idle display.
            ChaserAI.Instance?.ResetForRun();
        }

        private void HandleRunStarted()
        {
            WorldManager.Instance?.BeginRun(player);
            ChaserAI.Instance?.ResetForRun();
            WeatherController.Instance?.SetFollowTarget(FindFirstObjectByType<FollowCamera>()?.transform);
        }

        private void HandleGameOver(int score)
        {
            // World keeps simulating briefly during slow-motion; no teardown needed.
        }

        private void EnableFpsText(bool enabled)
        {
            // The HUD binds its own FpsText; this toggle simply activates it.
            var fps = FindFirstObjectByType<HUD>()?.transform.FindDeep<UnityEngine.UI.Text>("FpsText");
            if (fps != null)
            {
                fps.gameObject.SetActive(enabled);
            }
        }
    }
}
