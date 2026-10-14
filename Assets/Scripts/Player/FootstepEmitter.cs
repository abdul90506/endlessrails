using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Emits footstep audio in sync with the run cycle, with surface-based
    /// variation. When using a rigged model, call <see cref="OnFootstep"/>
    /// from Animation Events; the procedural rig drives footsteps from its
    /// run phase automatically.
    /// </summary>
    public class FootstepEmitter : MonoBehaviour
    {
        [SerializeField] private SurfaceType surface = SurfaceType.Concrete;
        [SerializeField] private float autoStepFrequency = 2.6f;   // steps per second at full speed
        [SerializeField] private ProceduralRunner proceduralRig;

        private float _stepTimer;
        private bool _stepAlternator;
        private PlayerMotor _motor;

        public void SetSurface(SurfaceType newSurface) => surface = newSurface;

        private void Awake()
        {
            if (proceduralRig == null)
            {
                proceduralRig = GetComponentInParent<ProceduralRunner>();
            }
            _motor = GetComponentInParent<PlayerMotor>();
        }

        private void Update()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.IsPlaying || _motor == null)
            {
                return;
            }
            if (!_motor.IsGrounded || _motor.IsSliding || _motor.JetpackActive)
            {
                return;
            }

            // Only auto-step for the procedural rig (rigged models use events).
            if (proceduralRig != null && proceduralRig.enabled == false)
            {
                return;
            }

            float speed01 = gm.Config != null
                ? Mathf.InverseLerp(gm.Config.startSpeed, gm.Config.maxSpeed, gm.CurrentSpeed)
                : 0.5f;

            _stepTimer -= Time.deltaTime * Mathf.Lerp(0.7f, 1.6f, speed01) * autoStepFrequency;
            if (_stepTimer <= 0f)
            {
                _stepTimer = 1f;
                _stepAlternator = !_stepAlternator;
                PlayStep(0.85f + speed01 * 0.25f);
            }
        }

        /// <summary>Animation-event hook for rigged character models.</summary>
        public void OnFootstep()
        {
            PlayStep(1f);
        }

        private void PlayStep(float volumeScale)
        {
            AudioManager.Instance?.PlayFootstep(surface, transform.position, volumeScale, _stepAlternator);
        }
    }
}
