using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Third-person follow camera with smooth damping, speed-driven FOV,
    /// landing shake, and an optional first-person mode. Also lerps the
    /// look target toward a point ahead of the player for that classic
    /// runner framing.
    /// </summary>
    public class FollowCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 followOffset = new Vector3(0f, 3.1f, -6.2f);
        [SerializeField] private float lookAhead = 4.5f;
        [SerializeField, Range(0f, 1f)] private float positionSmoothing = 0.12f;
        [SerializeField, Range(0f, 1f)] private float rotationSmoothing = 0.16f;

        [Header("FOV")]
        [SerializeField] private float baseFieldOfView = 62f;
        [SerializeField] private float maxFieldOfView = 74f;
        [SerializeField, Range(0f, 1f)] private float fovSpeedContribution = 0.55f;

        [Header("First Person")]
        [SerializeField] private bool firstPersonEnabled;
        [SerializeField] private Vector3 firstPersonOffset = new Vector3(0f, 1.62f, 0.12f);

        private Camera _camera;
        private Vector3 _currentVelocity;
        private Vector3 _currentLookVelocity;
        private float _currentFov;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            _currentFov = baseFieldOfView;
        }

        public void SetTarget(Transform newTarget) => target = newTarget;

        public void SetFirstPerson(bool enabled)
        {
            firstPersonEnabled = enabled;
        }

        public bool IsFirstPerson => firstPersonEnabled;

        private void LateUpdate()
        {
            GameManager gm = GameManager.Instance;
            if (target == null || gm == null)
            {
                return;
            }
            if (gm.State != GameState.Playing && gm.State != GameState.GameOver && gm.State != GameState.Paused)
            {
                return;
            }

            GameConfig config = gm.Config;
            float dt = Time.unscaledDeltaTime;

            if (config != null)
            {
                followOffset = new Vector3(0f, config.cameraHeight, -config.cameraDistance);
                lookAhead = config.cameraLookAhead;
                baseFieldOfView = config.baseFieldOfView;
                maxFieldOfView = config.maxFieldOfView;
                fovSpeedContribution = config.fovSpeedContribution;
                positionSmoothing = config.cameraFollowSmoothing;
            }

            // --- Position ---
            Vector3 desiredPosition;
            Quaternion desiredRotation;

            if (firstPersonEnabled)
            {
                desiredPosition = target.position + target.TransformVector(firstPersonOffset);
                desiredRotation = Quaternion.LookRotation(target.forward + Vector3.up * 0.05f, Vector3.up);
                transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, 0.05f),
                    Quaternion.Slerp(transform.rotation, desiredRotation, 12f * dt));
            }
            else
            {
                desiredPosition = target.position + followOffset;
                // Keep the camera's height stable over platforms.
                desiredPosition.y = Mathf.Max(desiredPosition.y, target.position.y + followOffset.y);

                Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, positionSmoothing);
                transform.position = smoothedPosition;

                Vector3 lookTarget = target.position + Vector3.up * 1.4f + target.forward * lookAhead;
                Quaternion lookRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSmoothing * 60f * dt);
            }

            // --- FOV kick with speed ---
            if (_camera != null)
            {
                float speed01 = config != null
                    ? Mathf.InverseLerp(config.startSpeed, config.maxSpeed, gm.CurrentSpeed)
                    : 0.5f;
                float targetFov = Mathf.Lerp(baseFieldOfView, maxFieldOfView, speed01 * fovSpeedContribution);
                _currentFov = Mathf.Lerp(_currentFov, targetFov, 3f * dt);
                _camera.fieldOfView = _currentFov;
            }

            // --- Shake (additive) ---
            if (CameraShake.Exists)
            {
                Vector3 position = transform.position;
                Quaternion rotation = transform.rotation;
                CameraShake.Instance.ApplyShake(ref position, ref rotation);
                transform.SetPositionAndRotation(position, rotation);
            }
        }

        private void OnEnable()
        {
            GameEvents.OnLanded += HandleLanded;
        }

        private void OnDisable()
        {
            GameEvents.OnLanded -= HandleLanded;
        }

        private void HandleLanded()
        {
            CameraShake.Instance?.Shake(0.12f, 0.12f);
        }
    }
}
