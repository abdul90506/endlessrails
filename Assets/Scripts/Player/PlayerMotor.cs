using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Handles all player locomotion: smooth eased lane changes, jumping with
    /// arcade gravity, sliding (collider squash), stumbling and jetpack flight.
    /// Pure movement - no input or game-state logic lives here.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CapsuleCollider bodyCollider;

        // --- Lane state ---
        public int CurrentLane { get; private set; }
        public int TargetLane { get; private set; }
        public bool IsChangingLane { get; private set; }
        public float LaneChangeBlend { get; private set; }   // 0..1 progress of current change
        public float LateralVelocity { get; private set; }   // signed m/s, used for lean

        private float _laneFromX;
        private float _laneToX;
        private float _laneChangeTime;

        // --- Vertical state ---
        public bool IsGrounded { get; private set; } = true;
        public bool IsJumping { get; private set; }
        public float VerticalVelocity { get; private set; }
        public float GroundY { get; set; }

        // --- Slide state ---
        public bool IsSliding { get; private set; }
        public float SlideTimeLeft { get; private set; }
        private float _slideCooldown;

        // --- Stumble / jetpack ---
        public bool IsStumbling { get; private set; }
        private float _stumbleTimer;
        public bool JetpackActive { get; private set; }
        private float _jetpackTimer;
        private float _jetpackHeight;
        private float _jetpackLerp;

        // Cached standing dimensions for slide squash.
        private float _standHeight;
        private Vector3 _standCenter;
        private Rigidbody _body;

        /// <summary>Speed multiplier currently applied (stumble slow-down, etc.)</summary>
        public float SpeedMultiplier { get; private set; } = 1f;

        /// <summary>0..1 how far the body is leaned into the current lane change (-1..1).</summary>
        public float LeanFactor { get; private set; }

        private GameConfig Config => GameManager.Instance != null ? GameManager.Instance.Config : null;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;   // Character is driven by script; collider is trigger-based.

            if (bodyCollider == null)
            {
                bodyCollider = GetComponent<CapsuleCollider>();
            }
            if (bodyCollider != null)
            {
                _standHeight = bodyCollider.height;
                _standCenter = bodyCollider.center;
            }
            CurrentLane = 0;
            TargetLane = 0;
        }

        /// <summary>Resets the motor to a clean run-start state.</summary>
        public void ResetForRun()
        {
            CurrentLane = 0;
            TargetLane = 0;
            IsChangingLane = false;
            LaneChangeBlend = 0f;
            transform.position = new Vector3(0f, GroundY, transform.position.z);
            IsGrounded = true;
            IsJumping = false;
            VerticalVelocity = 0f;
            IsSliding = false;
            SlideTimeLeft = 0f;
            _slideCooldown = 0f;
            IsStumbling = false;
            _stumbleTimer = 0f;
            JetpackActive = false;
            _jetpackTimer = 0f;
            SpeedMultiplier = 1f;
            RestoreCollider();
        }

        // ------------------------------------------------------------------
        // Lane changes
        // ------------------------------------------------------------------

        public void RequestLaneChange(int direction)
        {
            if (Config == null)
            {
                return;
            }
            if (JetpackActive)
            {
                return;
            }

            int newLane = Mathf.Clamp(TargetLane + (direction > 0 ? 1 : -1), -1, 1);
            if (newLane == TargetLane && !IsChangingLane)
            {
                return;
            }

            TargetLane = newLane;
            _laneFromX = transform.position.x;
            _laneToX = TargetLane * Config.laneWidth;
            _laneChangeTime = 0f;
            IsChangingLane = true;
            GameEvents.RaiseLaneChanged(TargetLane);
        }

        // ------------------------------------------------------------------
        // Jumping
        // ------------------------------------------------------------------

        public bool TryJump()
        {
            if (Config == null)
            {
                return false;
            }
            if (!IsGrounded || IsSliding || JetpackActive)
            {
                return false;
            }

            VerticalVelocity = Config.jumpVelocity;
            IsGrounded = false;
            IsJumping = true;
            GameManager.Instance?.RegisterJump();
            GameEvents.RaiseJumped();
            return true;
        }

        // ------------------------------------------------------------------
        // Sliding
        // ------------------------------------------------------------------

        public bool TrySlide()
        {
            if (Config == null)
            {
                return false;
            }
            if (IsSliding || _slideCooldown > 0f || JetpackActive)
            {
                return false;
            }
            if (!IsGrounded)
            {
                // Fast-fall into a slide when airborne - feels responsive.
                VerticalVelocity = -Mathf.Abs(Config.jumpVelocity) * 0.6f;
            }

            IsSliding = true;
            SlideTimeLeft = Config.slideDuration;
            _slideCooldown = Config.slideDuration + Config.slideCooldown;
            SquashCollider();
            GameManager.Instance?.RegisterSlide();
            GameEvents.RaiseSlideStarted();
            return true;
        }

        private void SquashCollider()
        {
            if (bodyCollider == null)
            {
                return;
            }
            bodyCollider.height = _standHeight * 0.45f;
            bodyCollider.center = new Vector3(_standCenter.x, _standHeight * 0.225f, _standCenter.z);
        }

        private void RestoreCollider()
        {
            if (bodyCollider == null)
            {
                return;
            }
            bodyCollider.height = _standHeight;
            bodyCollider.center = _standCenter;
        }

        // ------------------------------------------------------------------
        // Stumble / jetpack
        // ------------------------------------------------------------------

        public void ApplyStumble()
        {
            if (Config == null)
            {
                return;
            }
            IsStumbling = true;
            _stumbleTimer = Config.stumbleDuration;
            SpeedMultiplier = Config.stumbleSpeedFactor;
        }

        public void ActivateJetpack(float duration, float height)
        {
            JetpackActive = true;
            _jetpackTimer = duration;
            _jetpackHeight = height;
            _jetpackLerp = 0f;

            // Cancel slide/jump state while flying.
            if (IsSliding)
            {
                IsSliding = false;
                SlideTimeLeft = 0f;
                RestoreCollider();
                GameEvents.RaiseSlideEnded();
            }
            IsJumping = false;
            IsGrounded = false;
            VerticalVelocity = 0f;
        }

        public void DeactivateJetpack()
        {
            JetpackActive = false;
            _jetpackTimer = 0f;
        }

        // ------------------------------------------------------------------
        // Per-frame movement (driven by PlayerController)
        // ------------------------------------------------------------------

        /// <summary>
        /// Advances movement by one frame. Called by PlayerController so that
        /// ordering vs. input and collision stays deterministic.
        /// </summary>
        public void Tick(float forwardSpeed, float deltaTime)
        {
            TickLane(deltaTime);
            TickVertical(deltaTime);
            TickTimers(deltaTime);

            // Forward motion along +Z.
            Vector3 position = transform.position;
            position.z += forwardSpeed * SpeedMultiplier * deltaTime;
            transform.position = position;

            // Lean factor for the animator (-1..1).
            LeanFactor = Mathf.Lerp(LeanFactor,
                IsChangingLane ? Mathf.Sign(_laneToX - _laneFromX) * -1f : 0f,
                10f * deltaTime);
        }

        private void TickLane(float dt)
        {
            if (!IsChangingLane)
            {
                LateralVelocity = 0f;
                return;
            }

            if (Config == null)
            {
                IsChangingLane = false;
                return;
            }

            _laneChangeTime += dt;
            float t = Mathf.Clamp01(_laneChangeTime / Mathf.Max(0.01f, Config.laneChangeDuration));

            // Ease-in-out curve for buttery lane changes (no snapping).
            float eased = t * t * (3f - 2f * t);

            float previousX = transform.position.x;
            float newX = Mathf.Lerp(_laneFromX, _laneToX, eased);
            Vector3 position = transform.position;
            position.x = newX;
            transform.position = position;

            LateralVelocity = dt > 0f ? (newX - previousX) / dt : 0f;
            LaneChangeBlend = eased;

            if (t >= 1f)
            {
                IsChangingLane = false;
                CurrentLane = TargetLane;
                LaneChangeBlend = 1f;
            }
        }

        private void TickVertical(float dt)
        {
            Vector3 position = transform.position;

            if (JetpackActive)
            {
                _jetpackLerp = Mathf.MoveTowards(_jetpackLerp, 1f, dt * 1.6f);
                float targetY = GroundY + _jetpackHeight;
                position.y = Mathf.Lerp(position.y, targetY, _jetpackLerp * dt * 4f);
                if (Mathf.Abs(position.y - targetY) < 0.05f)
                {
                    position.y = targetY;
                }
                transform.position = position;
                return;
            }

            if (!IsGrounded || VerticalVelocity > 0f)
            {
                VerticalVelocity += Config != null ? Config.gravity * dt : -22f * dt;
                position.y += VerticalVelocity * dt;

                if (position.y <= GroundY)
                {
                    // Landing.
                    position.y = GroundY;
                    float impactSpeed = Mathf.Abs(VerticalVelocity);
                    VerticalVelocity = 0f;
                    if (!IsGrounded)
                    {
                        IsGrounded = true;
                        bool wasJumping = IsJumping;
                        IsJumping = false;
                        if (wasJumping)
                        {
                            GameEvents.RaiseLanded();
                            LandingImpact?.Invoke(impactSpeed);
                        }
                    }
                }
                transform.position = position;
            }
            else
            {
                position.y = GroundY;
                transform.position = position;
                IsGrounded = true;
            }
        }

        /// <summary>Raised with impact speed when the player lands from a jump.</summary>
        public System.Action<float> LandingImpact;

        private void TickTimers(float dt)
        {
            if (IsSliding)
            {
                SlideTimeLeft -= dt;
                if (SlideTimeLeft <= 0f)
                {
                    IsSliding = false;
                    RestoreCollider();
                    GameEvents.RaiseSlideEnded();
                }
            }

            if (_slideCooldown > 0f)
            {
                _slideCooldown -= dt;
            }

            if (IsStumbling)
            {
                _stumbleTimer -= dt;
                SpeedMultiplier = Mathf.Lerp(SpeedMultiplier, 1f, dt * 2f);
                if (_stumbleTimer <= 0f)
                {
                    IsStumbling = false;
                    SpeedMultiplier = 1f;
                }
            }

            if (JetpackActive)
            {
                _jetpackTimer -= dt;
                if (_jetpackTimer <= 0f)
                {
                    DeactivateJetpack();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Ground detection for platforms of varying height (chunk tops).
            if (other.CompareTag("Ground") && other.bounds.max.y <= transform.position.y + 0.2f)
            {
                GroundY = other.bounds.max.y;
            }
        }
    }
}
