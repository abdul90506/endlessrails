using System.Collections.Generic;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Active power-up state machine for the player: magnet, jetpack, shield
    /// and score multiplier. Each effect exposes a 0..1 remaining-time ratio
    /// for the HUD timer rings.
    /// </summary>
    public class PlayerPowerups : MonoBehaviour
    {
        private struct ActivePowerup
        {
            public PickupType type;
            public float timeLeft;
            public float duration;
        }

        private readonly List<ActivePowerup> _active = new List<ActivePowerup>(4);
        private readonly List<Coin> _magnetBuffer = new List<Coin>(64);

        [SerializeField] private Transform magnetOrigin;
        [SerializeField] private GameObject shieldVisual;

        private PlayerMotor _motor;
        private float _magnetScanTimer;

        private GameConfig Config => GameManager.Instance != null ? GameManager.Instance.Config : null;

        private void Awake()
        {
            _motor = GetComponentInParent<PlayerMotor>();
            if (magnetOrigin == null)
            {
                magnetOrigin = transform;
            }
            SetShieldVisual(false);
        }

        public void ResetForRun()
        {
            _active.Clear();
            SetShieldVisual(false);
        }

        // ------------------------------------------------------------------
        // Public API (called by Pickup)
        // ------------------------------------------------------------------

        public void Activate(PickupType type)
        {
            GameConfig config = Config;
            if (config == null)
            {
                return;
            }

            float duration = type switch
            {
                PickupType.Magnet => config.magnetDuration,
                PickupType.Jetpack => config.jetpackDuration,
                PickupType.Shield => config.shieldDuration,
                PickupType.Multiplier => config.multiplierDuration,
                _ => 5f
            };

            // Refresh if already running.
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].type == type)
                {
                    var refresh = _active[i];
                    refresh.timeLeft = duration;
                    refresh.duration = duration;
                    _active[i] = refresh;
                    ApplyImmediate(type, config);
                    return;
                }
            }

            _active.Add(new ActivePowerup { type = type, timeLeft = duration, duration = duration });
            ApplyImmediate(type, config);
            GameEvents.RaisePickupCollected(type);
            GameManager.Instance?.RegisterPickup();
        }

        private void ApplyImmediate(PickupType type, GameConfig config)
        {
            switch (type)
            {
                case PickupType.Jetpack:
                    _motor?.ActivateJetpack(config.jetpackDuration, config.jetpackHeight);
                    break;
                case PickupType.Shield:
                    GameManager.Instance?.SetShieldActive(true);
                    SetShieldVisual(true);
                    break;
                case PickupType.Multiplier:
                    GameManager.Instance?.SetScoreMultiplier(config.multiplierValue);
                    break;
            }
        }

        // ------------------------------------------------------------------
        // Timers + magnet pull
        // ------------------------------------------------------------------

        private void Update()
        {
            if (_active.Count == 0)
            {
                return;
            }

            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.IsPlaying)
            {
                return;
            }

            float dt = Time.deltaTime;
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                var power = _active[i];
                power.timeLeft -= dt;
                if (power.timeLeft <= 0f)
                {
                    Expire(power.type);
                    _active.RemoveAt(i);
                }
                else
                {
                    _active[i] = power;
                }
            }

            TickMagnet(dt);
        }

        private void Expire(PickupType type)
        {
            switch (type)
            {
                case PickupType.Jetpack:
                    _motor?.DeactivateJetpack();
                    break;
                case PickupType.Shield:
                    GameManager.Instance?.SetShieldActive(false);
                    SetShieldVisual(false);
                    break;
                case PickupType.Multiplier:
                    GameManager.Instance?.SetScoreMultiplier(1);
                    break;
            }
            GameEvents.RaisePickupExpired(type);
        }

        private void TickMagnet(float dt)
        {
            if (!IsPowerupActive(PickupType.Magnet) || Config == null)
            {
                return;
            }

            _magnetScanTimer -= dt;
            if (_magnetScanTimer <= 0f)
            {
                _magnetScanTimer = 0.2f;
                _magnetBuffer.Clear();
                float radius = Config.magnetRadius;
                Collider[] hits = Physics.OverlapSphere(magnetOrigin.position, radius, ~0, QueryTriggerInteraction.Collide);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Coin") && hit.TryGetComponent(out Coin coin))
                    {
                        _magnetBuffer.Add(coin);
                    }
                }
            }

            // Pull coins toward the player.
            foreach (var coin in _magnetBuffer)
            {
                if (coin == null || !coin.isActiveAndEnabled)
                {
                    continue;
                }
                coin.PullToward(magnetOrigin.position, dt);
            }
        }

        public bool IsPowerupActive(PickupType type)
        {
            foreach (var power in _active)
            {
                if (power.type == type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>0..1 remaining fraction for HUD timer rings (1 = just started).</summary>
        public float GetRemainingFraction(PickupType type)
        {
            foreach (var power in _active)
            {
                if (power.type == type)
                {
                    return Mathf.Clamp01(power.timeLeft / Mathf.Max(0.01f, power.duration));
                }
            }
            return 0f;
        }

        private void SetShieldVisual(bool active)
        {
            if (shieldVisual != null)
            {
                shieldVisual.SetActive(active);
            }
        }
    }
}
