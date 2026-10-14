using UnityEngine;
using UnityEngine.UI;

namespace EndlessRails
{
    /// <summary>
    /// Game over panel: score summary, best score, coins earned, revive
    /// (watch-ad placeholder) and restart. Shown after the crash slow-motion.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text bestScoreText;
        [SerializeField] private Text coinsText;
        [SerializeField] private Text distanceText;
        [SerializeField] private Text newBestLabel;

        private void Awake()
        {
            if (scoreText == null) scoreText = transform.FindDeep<Text>("FinalScoreText");
            if (bestScoreText == null) bestScoreText = transform.FindDeep<Text>("BestScoreText");
            if (coinsText == null) coinsText = transform.FindDeep<Text>("CoinsText");
            if (distanceText == null) distanceText = transform.FindDeep<Text>("DistanceText");
            if (newBestLabel == null) newBestLabel = transform.FindDeep<Text>("NewBestLabel");
        }

        private void OnEnable()
        {
            GameEvents.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            GameEvents.OnGameOver -= HandleGameOver;
        }

        private void HandleGameOver(int finalScore)
        {
            GameManager gm = GameManager.Instance;
            SaveSystem save = SaveSystem.Instance;

            if (scoreText != null)
            {
                scoreText.text = finalScore.ToString("N0");
            }
            if (bestScoreText != null && save != null)
            {
                bestScoreText.text = save.Data.highScore.ToString("N0");
            }
            if (coinsText != null && gm != null)
            {
                coinsText.text = $"+{gm.RunCoins:N0}";
            }
            if (distanceText != null && gm != null)
            {
                distanceText.text = $"{gm.Distance:0} m";
            }
            if (newBestLabel != null && save != null)
            {
                newBestLabel.gameObject.SetActive(save.Data.highScore > 0 && finalScore >= save.Data.highScore);
            }

            // Note: coins were already added to the wallet by GameManager.CrashSequence.

            AudioManager.Instance?.PlayGameOverSting();
            UIManager.Instance?.Show("GameOver");
        }

        // --- Buttons ---

        public void OnRevive()
        {
            // Ad placeholder: in production, hook your ad SDK here and call
            // GameManager.Instance.Revive() on reward completion.
            AudioManager.Instance?.PlayButton();
            GameManager.Instance?.Revive();
            UIManager.Instance?.Show("HUD");
        }

        public void OnRestart()
        {
            UIManager.Instance?.OnRestartPressed();
        }

        public void OnMainMenu()
        {
            UIManager.Instance?.OnMainMenuPressed();
        }
    }
}
