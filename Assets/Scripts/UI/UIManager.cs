using System.Collections.Generic;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// UI screen router. Panels are enabled/disabled by name; the scene builder
    /// creates the full hierarchy under one Canvas. Handles Android back button.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [System.Serializable]
        public class PanelEntry
        {
            public string name;
            public GameObject panel;
        }

        [SerializeField] private PanelEntry[] panels = new PanelEntry[0];
        [SerializeField] private CanvasGroup fadeGroup;

        private readonly Dictionary<string, GameObject> _panelByName = new Dictionary<string, GameObject>(12);
        private string _currentPanel = "";

        protected override void OnSingletonAwake()
        {
            foreach (var entry in panels)
            {
                if (entry != null && entry.panel != null)
                {
                    _panelByName[entry.name] = entry.panel;
                }
            }
        }

        private void Start()
        {
            // First launch shows the tutorial; every later launch goes to the menu.
            SaveSystem save = SaveSystem.Instance;
            bool showTutorial = save == null || !save.Data.tutorialShown;
            Show(showTutorial ? "Tutorial" : "MainMenu");
            GameEvents.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDestroy()
        {
            GameEvents.OnGameStateChanged -= HandleGameStateChanged;
            base.OnDestroy();
        }

        private void HandleGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    Show("MainMenu");
                    break;
                case GameState.Playing:
                    Show("HUD");
                    break;
                case GameState.Paused:
                    Show("PauseMenu");
                    break;
                case GameState.GameOver:
                    // Game over screen is shown by CrashSequence via event.
                    break;
            }
        }

        public void RegisterPanel(string panelName, GameObject panel)
        {
            _panelByName[panelName] = panel;
        }

        /// <summary>Shows one panel and hides all others.</summary>
        public void Show(string panelName)
        {
            foreach (var kvp in _panelByName)
            {
                kvp.Value.SetActive(kvp.Key == panelName);
            }
            _currentPanel = panelName;
            AudioManager.Instance?.PlayUISwoosh();
        }

        public string CurrentPanel => _currentPanel;

        // --- Button hooks (wired by the scene builder) ---

        public void OnPlayPressed()
        {
            AudioManager.Instance?.PlayButton();
            GameManager.Instance?.StartRun();
        }

        public void OnPausePressed()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null)
            {
                return;
            }
            if (gm.State == GameState.Playing)
            {
                gm.PauseGame();
            }
            else if (gm.State == GameState.Paused)
            {
                gm.ResumeGame();
            }
        }

        public void OnResumePressed()
        {
            AudioManager.Instance?.PlayButton();
            GameManager.Instance?.ResumeGame();
        }

        public void OnRestartPressed()
        {
            AudioManager.Instance?.PlayButton();
            GameManager.Instance?.RestartRun();
            Show("HUD");
        }

        public void OnMainMenuPressed()
        {
            AudioManager.Instance?.PlayButton();
            GameManager.Instance?.ReturnToMenu();
            Show("MainMenu");
        }

        public void OnQuitPressed()
        {
            AudioManager.Instance?.PlayButton();
            SaveSystem.Instance?.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void Update()
        {
            // Android back button = pause / menu back.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager gm = GameManager.Instance;
                if (gm == null)
                {
                    return;
                }
                switch (gm.State)
                {
                    case GameState.Playing:
                        gm.PauseGame();
                        break;
                    case GameState.Paused:
                        gm.ResumeGame();
                        break;
                    case GameState.GameOver:
                        Show("GameOver");
                        break;
                    case GameState.MainMenu:
                        // Already on the menu - ignore.
                        break;
                }
            }
        }
    }
}
