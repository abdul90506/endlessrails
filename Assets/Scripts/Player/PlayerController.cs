using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Glue between input, motor, camera-facing behaviour and game state.
    /// Owns the run loop for the player character.
    /// </summary>
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMotor motor;
        [SerializeField] private ProceduralRunner proceduralRig;
        [SerializeField] private PlayerAnimatorDriver animatorDriver;
        [SerializeField] private PlayerPowerups powerups;
        [SerializeField] private PlayerCollisionHandler collisionHandler;

        public PlayerMotor Motor => motor;
        public PlayerPowerups Powerups => powerups;
        public bool IsAlive { get; private set; } = true;

        private void Awake()
        {
            if (motor == null) motor = GetComponent<PlayerMotor>();
            if (proceduralRig == null) proceduralRig = GetComponentInChildren<ProceduralRunner>();
            if (animatorDriver == null) animatorDriver = GetComponentInChildren<PlayerAnimatorDriver>();
            if (powerups == null) powerups = GetComponentInChildren<PlayerPowerups>();
            if (collisionHandler == null) collisionHandler = GetComponentInChildren<PlayerCollisionHandler>();
        }

        private void OnEnable()
        {
            SwipeInputManager.OnSwipeLeft += HandleSwipeLeft;
            SwipeInputManager.OnSwipeRight += HandleSwipeRight;
            SwipeInputManager.OnSwipeUp += HandleSwipeUp;
            SwipeInputManager.OnSwipeDown += HandleSwipeDown;
            GameEvents.OnRunStarted += HandleRunStarted;
            GameEvents.OnRunRevived += HandleRevived;
        }

        private void OnDisable()
        {
            SwipeInputManager.OnSwipeLeft -= HandleSwipeLeft;
            SwipeInputManager.OnSwipeRight -= HandleSwipeRight;
            SwipeInputManager.OnSwipeUp -= HandleSwipeUp;
            SwipeInputManager.OnSwipeDown -= HandleSwipeDown;
            GameEvents.OnRunStarted -= HandleRunStarted;
            GameEvents.OnRunRevived -= HandleRevived;
        }

        private void Start()
        {
            motor.LandingImpact += HandleLandingImpact;
        }

        private void Update()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.IsPlaying || !IsAlive)
            {
                return;
            }

            motor.Tick(gm.CurrentSpeed, Time.deltaTime);

            // Feed the animator / procedural rig every frame.
            float speed01 = gm.Config != null
                ? Mathf.InverseLerp(gm.Config.startSpeed, gm.Config.maxSpeed, gm.CurrentSpeed)
                : 0.5f;
            proceduralRig?.SetLocomotion(speed01, motor.IsChangingLane ? motor.LeanFactor : 0f);
            proceduralRig?.SetAirborne(!motor.IsGrounded && !motor.JetpackActive);
            proceduralRig?.SetSliding(motor.IsSliding);
            proceduralRig?.SetJetpack(motor.JetpackActive);
            animatorDriver?.SetLocomotion(speed01, motor.LeanFactor);
            animatorDriver?.SetGrounded(motor.IsGrounded);
            animatorDriver?.SetSliding(motor.IsSliding);
            animatorDriver?.SetJetpack(motor.JetpackActive);
        }

        // --- Input handlers ---

        private void HandleSwipeLeft()
        {
            if (!CanAct()) return;
            motor.RequestLaneChange(-1);
        }

        private void HandleSwipeRight()
        {
            if (!CanAct()) return;
            motor.RequestLaneChange(1);
        }

        private void HandleSwipeUp()
        {
            if (!CanAct()) return;
            if (motor.TryJump())
            {
                animatorDriver?.TriggerJump();
                proceduralRig?.TriggerJump();
            }
        }

        private void HandleSwipeDown()
        {
            if (!CanAct()) return;
            if (motor.TrySlide())
            {
                animatorDriver?.TriggerSlide();
                proceduralRig?.TriggerSlide();
            }
        }

        private bool CanAct()
        {
            GameManager gm = GameManager.Instance;
            return gm != null && gm.IsPlaying && IsAlive;
        }

        private void HandleRunStarted()
        {
            IsAlive = true;
            motor.ResetForRun();
            collisionHandler?.ResetForRun();
            powerups?.ResetForRun();
            proceduralRig?.ResetPose();
        }

        private void HandleRevived()
        {
            IsAlive = true;
            motor.ResetForRun();
            collisionHandler?.ResetForRun();
        }

        private void HandleLandingImpact(float impactSpeed)
        {
            float strength = Mathf.InverseLerp(2f, 14f, impactSpeed);
            CameraShake.Instance?.Shake(0.18f + strength * 0.22f, 0.22f * strength + 0.08f);
        }

        /// <summary>Called by the collision handler on a lethal hit.</summary>
        public void NotifyCrashed()
        {
            if (!IsAlive)
            {
                return;
            }
            IsAlive = false;
            proceduralRig?.EnableRagdoll();
            animatorDriver?.TriggerHit();
            GameManager.Instance?.TriggerGameOver();
        }

        /// <summary>Called by the collision handler on a stumble hit.</summary>
        public void NotifyStumbled()
        {
            motor.ApplyStumble();
            animatorDriver?.TriggerStumble();
            proceduralRig?.TriggerStumble();
            CameraShake.Instance?.Shake(0.35f, 0.12f);
        }
    }
}
