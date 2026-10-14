using UnityEngine;
using UnityEngine.UI;

namespace EndlessRails
{
    /// <summary>High scores / lifetime stats panel.</summary>
    public class HighScoresScreen : MonoBehaviour
    {
        [SerializeField] private Text statsText;

        private void Awake()
        {
            if (statsText == null)
            {
                statsText = transform.FindDeep<Text>("StatsText");
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null || statsText == null)
            {
                return;
            }
            SaveData data = save.Data;
            statsText.text =
                $"Best score:        {data.highScore:N0}\n" +
                $"Longest run:       {data.bestDistance:0} m\n" +
                $"Total coins:       {data.totalCoinsCollected:N0}\n" +
                $"Runs played:       {data.totalRuns:N0}\n" +
                $"Jumps:             {data.totalJumps:N0}\n" +
                $"Slides:            {data.totalSlides:N0}\n" +
                $"Pickups used:      {data.totalPickups:N0}\n" +
                $"Coin balance:      {data.coinsBalance:N0}";
        }

        public void OnBack()
        {
            AudioManager.Instance?.PlayButton();
            UIManager.Instance?.Show("MainMenu");
        }
    }
}
