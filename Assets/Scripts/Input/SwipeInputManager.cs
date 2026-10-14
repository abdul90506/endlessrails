using System;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Unified touch/keyboard input for the runner.
    /// Swipe left/right = lane change, swipe up = jump, swipe down = slide,
    /// tap = pause. Keyboard is supported for quick editor testing
    /// (arrows / WASD, Space = jump, Down = slide, Escape = pause).
    /// </summary>
    public class SwipeInputManager : Singleton<SwipeInputManager>
    {
        [Header("Swipe Tuning")]
        [Tooltip("Minimum drag distance in inches-equivalent screen units to count as a swipe.")]
        [SerializeField] private float swipeThresholdPixels = 48f;

        [Tooltip("Maximum finger travel before a 'tap' is cancelled.")]
        [SerializeField] private float tapMaxPixels = 24f;

        [Tooltip("Longest press duration that still counts as a tap (seconds).")]
        [SerializeField] private float tapMaxDuration = 0.3f;

        [Tooltip("Sensitivity multiplier applied to the swipe threshold (settings menu).")]
        [SerializeField, Range(0.5f, 2f)] private float sensitivity = 1f;

        // --- Events (consumed by PlayerController / GameManager / UI) ---
        public static event Action OnSwipeLeft;
        public static event Action OnSwipeRight;
        public static event Action OnSwipeUp;
        public static event Action OnSwipeDown;
        public static event Action OnTap;
        public static event Action OnPausePressed;

        private Vector2 _pressPosition;
        private float _pressTime;
        private bool _pressActive;
        private bool _swipedThisPress;

        // DPI-aware threshold so swipes feel identical on phones and tablets.
        private float EffectiveThreshold => swipeThresholdPixels * sensitivity * (Screen.dpi > 100f ? Screen.dpi / 160f : 1f) * 0.65f;

        public void SetSensitivity(float value)
        {
            sensitivity = Mathf.Clamp(value, 0.5f, 2f);
        }

        public float GetSensitivity() => sensitivity;

        private void Update()
        {
            // Global pause key works in any state.
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                OnPausePressed?.Invoke();
            }

            HandleTouch();
            HandleKeyboard();
        }

        private void HandleTouch()
        {
            // Mouse in the editor simulates one finger.
            if (Input.GetMouseButtonDown(0))
            {
                BeginPress(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0) && _pressActive && !_swipedThisPress)
            {
                CheckSwipe((Vector2)Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0) && _pressActive)
            {
                EndPress((Vector2)Input.mousePosition);
            }

            // Real device touch path (legacy input manager - enabled project-wide).
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        BeginPress(touch.position);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (_pressActive && !_swipedThisPress)
                        {
                            CheckSwipe(touch.position);
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (_pressActive)
                        {
                            EndPress(touch.position);
                        }
                        break;
                }
            }
        }

        private void BeginPress(Vector2 position)
        {
            _pressPosition = position;
            _pressTime = Time.unscaledTime;
            _pressActive = true;
            _swipedThisPress = false;
        }

        private void CheckSwipe(Vector2 current)
        {
            Vector2 delta = current - _pressPosition;
            if (delta.sqrMagnitude < EffectiveThreshold * EffectiveThreshold)
            {
                return;
            }

            _swipedThisPress = true;
            _pressActive = false;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0f) OnSwipeRight?.Invoke();
                else OnSwipeLeft?.Invoke();
            }
            else
            {
                if (delta.y > 0f) OnSwipeUp?.Invoke();
                else OnSwipeDown?.Invoke();
            }
        }

        private void EndPress(Vector2 current)
        {
            _pressActive = false;
            if (_swipedThisPress)
            {
                return;
            }

            float duration = Time.unscaledTime - _pressTime;
            Vector2 delta = current - _pressPosition;
            if (duration <= tapMaxDuration && delta.magnitude <= tapMaxPixels)
            {
                OnTap?.Invoke();
            }
        }

        private void HandleKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                OnSwipeLeft?.Invoke();
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                OnSwipeRight?.Invoke();
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
                OnSwipeUp?.Invoke();
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                OnSwipeDown?.Invoke();
        }
    }
}
