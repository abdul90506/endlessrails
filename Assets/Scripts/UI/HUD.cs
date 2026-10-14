using UnityEngine;
using UnityEngine.UI;

namespace EndlessRails
{
    /// <summary>
    /// In-run HUD: score, coins, distance, active power-up timer rings and the
    /// pause button. Binds to child objects by name so the scene builder can
    /// wire the hierarchy without serialized references.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text coinText;
        [SerializeField] private Text distanceText;
        [SerializeField] private Text multiplierText;
        [SerializeField] private Image magnetRing;
        [SerializeField] private Image jetpackRing;
        [SerializeField] private Image shieldRing;
        [SerializeField] private Image multiplierRing;
        [SerializeField] private GameObject magnetIcon;
        [SerializeField] private GameObject jetpackIcon;
        [SerializeField] private GameObject shieldIcon;
        [SerializeField] private GameObject multiplierIcon;
        [SerializeField] private Text fpsText;

        private PlayerPowerups _powerups;

        private void Awake()
        {
            // Fallback binding for scene-generated hierarchies.
            if (scoreText == null) scoreText = transform.FindDeep<Text>("ScoreText");
            if (coinText == null) coinText = transform.FindDeep<Text>("CoinText");
            if (distanceText == null) distanceText = transform.FindDeep<Text>("DistanceText");
            if (multiplierText == null) multiplierText = transform.FindDeep<Text>("MultiplierText");
            if (fpsText == null) fpsText = transform.FindDeep<Text>("FpsText");
            if (magnetRing == null) magnetRing = transform.FindDeep<Image>("MagnetRing");
            if (jetpackRing == null) jetpackRing = transform.FindDeep<Image>("JetpackRing");
            if (shieldRing == null) shieldRing = transform.FindDeep<Image>("ShieldRing");
            if (multiplierRing == null) multiplierRing = transform.FindDeep<Image>("MultiplierRing");
            if (magnetIcon == null) magnetIcon = FindDeepGO("MagnetIcon");
            if (jetpackIcon == null) jetpackIcon = FindDeepGO("JetpackIcon");
            if (shieldIcon == null) shieldIcon = FindDeepGO("ShieldIcon");
            if (multiplierIcon == null) multiplierIcon = FindDeepGO("MultiplierIcon");
        }

        private void OnEnable()
        {
            GameEvents.OnScoreChanged += HandleScore;
            GameEvents.OnCoinsChanged += HandleCoins;
            GameEvents.OnDistanceChanged += HandleDistance;
        }

        private void OnDisable()
        {
            GameEvents.OnScoreChanged -= HandleScore;
            GameEvents.OnCoinsChanged -= HandleCoins;
            GameEvents.OnDistanceChanged -= HandleDistance;
        }

        private void Start()
        {
            _powerups = FindFirstObjectByType<PlayerPowerups>();
        }

        private void Update()
        {
            RefreshPowerupRings();
            RefreshFps();
        }

        private void HandleScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = score.ToString("N0");
            }
        }

        private void HandleCoins(int coins)
        {
            if (coinText != null)
            {
                coinText.text = coins.ToString("N0");
            }
        }

        private void HandleDistance(float meters)
        {
            if (distanceText != null)
            {
                distanceText.text = $"{meters:0} m";
            }
        }

        private void RefreshPowerupRings()
        {
            if (_powerups == null)
            {
                _powerups = FindFirstObjectByType<PlayerPowerups>();
                if (_powerups == null)
                {
                    return;
                }
            }

            SetRing(magnetRing, magnetIcon, _powerups, PickupType.Magnet);
            SetRing(jetpackRing, jetpackIcon, _powerups, PickupType.Jetpack);
            SetRing(shieldRing, shieldIcon, _powerups, PickupType.Shield);
            SetRing(multiplierRing, multiplierIcon, _powerups, PickupType.Multiplier);

            if (multiplierText != null)
            {
                bool active = _powerups.IsPowerupActive(PickupType.Multiplier);
                multiplierText.gameObject.SetActive(active);
                if (active)
                {
                    int value = GameManager.Instance != null ? GameManager.Instance.ScoreMultiplier : 2;
                    multiplierText.text = $"x{value}";
                }
            }
        }

        private static void SetRing(Image ring, GameObject icon, PlayerPowerups powerups, PickupType type)
        {
            if (powerups == null)
            {
                return;
            }
            float remaining = powerups.GetRemainingFraction(type);
            bool active = remaining > 0f;
            if (ring != null)
            {
                ring.gameObject.SetActive(active);
                if (active)
                {
                    ring.fillAmount = remaining;
                }
            }
            if (icon != null)
            {
                icon.SetActive(active);
            }
        }

        private float _fpsTimer;
        private int _fpsFrames;

        private void RefreshFps()
        {
            if (fpsText == null || !fpsText.gameObject.activeSelf)
            {
                return;
            }
            _fpsTimer += Time.unscaledDeltaTime;
            _fpsFrames++;
            if (_fpsTimer >= 0.5f)
            {
                float fps = _fpsFrames / _fpsTimer;
                fpsText.text = $"{fps:0} FPS";
                _fpsTimer = 0f;
                _fpsFrames = 0;
            }
        }

        private GameObject FindDeepGO(string name)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }
            return null;
        }
    }

    /// <summary>Small transform search helpers used by UI binding.</summary>
    public static class TransformSearchExtensions
    {
        public static T FindDeep<T>(this Transform parent, string name) where T : Component
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == name && child.TryGetComponent(out T component))
                {
                    return component;
                }
            }
            return null;
        }
    }
}
