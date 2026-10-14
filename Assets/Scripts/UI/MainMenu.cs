using UnityEngine;
using UnityEngine.UI;

namespace EndlessRails
{
    /// <summary>
    /// Main menu screen: Play, Character Select, Settings, High Scores, Quit,
    /// plus wallet and best-score readouts and the daily-mission list.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Text bestScoreText;
        [SerializeField] private Text coinsText;
        [SerializeField] private Text bestDistanceText;
        [SerializeField] private Text missionText;

        private void Awake()
        {
            if (bestScoreText == null) bestScoreText = transform.FindDeep<Text>("BestScoreText");
            if (coinsText == null) coinsText = transform.FindDeep<Text>("CoinsText");
            if (bestDistanceText == null) bestDistanceText = transform.FindDeep<Text>("BestDistanceText");
            if (missionText == null) missionText = transform.FindDeep<Text>("MissionText");
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null)
            {
                return;
            }
            if (bestScoreText != null)
            {
                bestScoreText.text = $"Best: {save.Data.highScore:N0}";
            }
            if (coinsText != null)
            {
                coinsText.text = $"{save.Data.coinsBalance:N0}";
            }
            if (bestDistanceText != null)
            {
                bestDistanceText.text = $"Longest run: {save.Data.bestDistance:0} m";
            }
            if (missionText != null)
            {
                MissionManager missions = MissionManager.Instance;
                if (missions != null && missions.ActiveMissions.Count > 0)
                {
                    MissionDefinition first = missions.ActiveMissions[0];
                    missionText.text = $"{first.displayName}: {missions.GetProgressCurrent(first)}/{first.target}";
                }
                else
                {
                    missionText.text = "Complete runs to earn rewards!";
                }
            }
        }

        // --- Button hooks ---

        public void OnPlay()
        {
            UIManager.Instance?.OnPlayPressed();
        }

        public void OnCharacterSelect()
        {
            AudioManager.Instance?.PlayButton();
            UIManager.Instance?.Show("CharacterSelect");
        }

        public void OnSettings()
        {
            AudioManager.Instance?.PlayButton();
            UIManager.Instance?.Show("Settings");
        }

        public void OnHighScores()
        {
            AudioManager.Instance?.PlayButton();
            UIManager.Instance?.Show("HighScores");
        }

        public void OnQuit()
        {
            UIManager.Instance?.OnQuitPressed();
        }
    }
}
