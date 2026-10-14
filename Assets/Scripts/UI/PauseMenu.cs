using UnityEngine;

namespace EndlessRails
{
    /// <summary>Pause menu: Resume, Restart, Settings, Main Menu.</summary>
    public class PauseMenu : MonoBehaviour
    {
        public void OnResume()
        {
            UIManager.Instance?.OnResumePressed();
        }

        public void OnRestart()
        {
            UIManager.Instance?.OnRestartPressed();
        }

        public void OnSettings()
        {
            AudioManager.Instance?.PlayButton();
            UIManager.Instance?.Show("Settings");
        }

        public void OnMainMenu()
        {
            UIManager.Instance?.OnMainMenuPressed();
        }
    }
}
