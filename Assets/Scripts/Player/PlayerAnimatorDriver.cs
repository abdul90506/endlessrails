using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Drives a Unity Animator when a rigged humanoid model is assigned via
    /// CharacterDefinition. Expected animator parameters:
    ///   Speed (float 0..1), Grounded (bool), Jump (trigger), Slide (trigger),
    ///   Stumble (trigger), Hit (trigger), Lean (float -1..1), Jetpack (bool).
    /// Uses blend trees for run/idle; safe no-op when no Animator is present.
    /// </summary>
    public class PlayerAnimatorDriver : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int GroundedParam = Animator.StringToHash("Grounded");
        private static readonly int JumpParam = Animator.StringToHash("Jump");
        private static readonly int SlideParam = Animator.StringToHash("Slide");
        private static readonly int StumbleParam = Animator.StringToHash("Stumble");
        private static readonly int HitParam = Animator.StringToHash("Hit");
        private static readonly int LeanParam = Animator.StringToHash("Lean");
        private static readonly int JetpackParam = Animator.StringToHash("Jetpack");

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        public void SetAnimator(Animator newAnimator)
        {
            animator = newAnimator;
        }

        public void SetLocomotion(float speed01, float lean)
        {
            if (animator == null || !animator.isActiveAndEnabled)
            {
                return;
            }
            animator.SetFloat(SpeedParam, speed01, 0.08f, Time.deltaTime);
            animator.SetFloat(LeanParam, lean, 0.1f, Time.deltaTime);
        }

        public void SetGrounded(bool grounded)
        {
            if (animator == null || !animator.isActiveAndEnabled)
            {
                return;
            }
            animator.SetBool(GroundedParam, grounded);
        }

        public void SetSliding(bool sliding)
        {
            if (animator == null || !animator.isActiveAndEnabled)
            {
                return;
            }
            animator.SetBool("Sliding", sliding);
        }

        public void SetJetpack(bool active)
        {
            if (animator == null || !animator.isActiveAndEnabled)
            {
                return;
            }
            animator.SetBool(JetpackParam, active);
        }

        public void TriggerJump() => FireTrigger(JumpParam);
        public void TriggerSlide() => FireTrigger(SlideParam);
        public void TriggerStumble() => FireTrigger(StumbleParam);
        public void TriggerHit() => FireTrigger(HitParam);

        private void FireTrigger(int id)
        {
            if (animator == null || !animator.isActiveAndEnabled)
            {
                return;
            }
            animator.SetTrigger(id);
        }
    }
}
