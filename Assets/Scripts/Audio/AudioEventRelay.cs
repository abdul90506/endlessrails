using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Subscribes to GameEvents and routes them to AudioManager so every player
    /// action produces sound without gameplay code knowing about audio at all.
    /// </summary>
    public class AudioEventRelay : MonoBehaviour
    {
        [SerializeField] private Transform player;

        private void Awake()
        {
            if (player == null)
            {
                var controller = FindFirstObjectByType<PlayerController>();
                if (controller != null)
                {
                    player = controller.transform;
                }
            }
        }

        private void OnEnable()
        {
            GameEvents.OnJumped += HandleJumped;
            GameEvents.OnLanded += HandleLanded;
            GameEvents.OnSlideStarted += HandleSlideStarted;
            GameEvents.OnLaneChanged += HandleLaneChanged;
            GameEvents.OnStumbled += HandleStumbled;
        }

        private void OnDisable()
        {
            GameEvents.OnJumped -= HandleJumped;
            GameEvents.OnLanded -= HandleLanded;
            GameEvents.OnSlideStarted -= HandleSlideStarted;
            GameEvents.OnLaneChanged -= HandleLaneChanged;
            GameEvents.OnStumbled -= HandleStumbled;
        }

        private Vector3 PlayerPosition => player != null ? player.position : Vector3.zero;

        private void HandleJumped() => AudioManager.Instance?.PlayJump(PlayerPosition);
        private void HandleLanded() => AudioManager.Instance?.PlayLand(PlayerPosition, 0.7f);
        private void HandleSlideStarted() => AudioManager.Instance?.PlaySlide(PlayerPosition);
        private void HandleLaneChanged(int lane) => AudioManager.Instance?.PlayLaneChange();

        private void HandleStumbled()
        {
            // Stumble sound is also played by the collision handler with impact context.
        }
    }
}
