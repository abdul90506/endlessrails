using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Single collision funnel for the player. Trigger volumes on the player
    /// report coins, pickups and obstacle hits here; results are routed to
    /// GameManager / PlayerController. Uses tags for category detection so
    /// obstacle prefabs need zero per-obstacle wiring.
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [SerializeField] private PlayerController controller;

        // Frame-budget guard: a single contact may fire multiple trigger events.
        private float _lastHitTime = -1f;
        private const float HitCooldown = 0.35f;

        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<PlayerController>();
            }
        }

        public void ResetForRun()
        {
            _lastHitTime = -1f;
        }

        private void OnTriggerEnter(Collider other)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.IsPlaying)
            {
                return;
            }

            // --- Coins ---
            if (other.CompareTag("Coin"))
            {
                if (other.TryGetComponent(out Coin coin))
                {
                    coin.Collect();
                }
                else
                {
                    Destroy(other.gameObject);
                }
                return;
            }

            // --- Pickups ---
            if (other.CompareTag("Pickup"))
            {
                if (other.TryGetComponent(out Pickup pickup))
                {
                    pickup.Collect(controller);
                }
                return;
            }

            // --- Obstacles (all variants) ---
            bool isObstacle = other.CompareTag("Obstacle")
                              || other.CompareTag("Train")
                              || other.CompareTag("Vehicle")
                              || other.CompareTag("Stumble");

            if (!isObstacle)
            {
                return;
            }

            if (Time.time - _lastHitTime < HitCooldown)
            {
                return;
            }

            // Overhead hazards only hit the player when NOT sliding.
            if (other.TryGetComponent(out Obstacle obstacle))
            {
                if (obstacle.MustSlideUnder && controller.Motor.IsSliding)
                {
                    return;
                }
                // Jumped clear of a jumpable obstacle?
                if (obstacle.CanJumpOver && controller.Motor.IsGrounded == false
                    && transform.position.y > obstacle.transform.position.y + obstacle.ClearanceHeight)
                {
                    return;
                }

                HandleObstacleHit(obstacle);
            }
            else
            {
                // Fallback for untagged obstacle objects.
                HandleObstacleHit(null);
            }
        }

        private void HandleObstacleHit(Obstacle obstacle)
        {
            _lastHitTime = Time.time;

            ObstacleImpact impact = obstacle != null ? obstacle.Impact : ObstacleImpact.Lethal;

            // Shield absorbs one lethal hit.
            if (impact == ObstacleImpact.Lethal && GameManager.Instance.ConsumeShield())
            {
                AudioManager.Instance?.PlayUIShieldBreak();
                if (obstacle != null)
                {
                    obstacle.NotifyShieldedHit();
                }
                return;
            }

            if (impact == ObstacleImpact.Stumble)
            {
                GameEvents.RaiseStumbled();
                controller.NotifyStumbled();
                AudioManager.Instance?.PlayStumble();
                // Stumbles also close the gap on the chaser.
                ChaserAI.Instance?.NotifyPlayerStumbled();
            }
            else
            {
                AudioManager.Instance?.PlayCrash();
                controller.NotifyCrashed();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // Keep ground height fresh while standing on platforms.
            if (other.CompareTag("Ground") && controller != null && controller.Motor != null)
            {
                controller.Motor.GroundY = other.bounds.max.y;
            }
        }
    }
}
