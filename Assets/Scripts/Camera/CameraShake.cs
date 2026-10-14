using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Trauma-based camera shake with decaying noise. Additive on top of the
    /// follow camera's position/rotation each frame.
    /// </summary>
    public class CameraShake : Singleton<CameraShake>
    {
        [SerializeField] private float maxPositionalShake = 0.22f;
        [SerializeField] private float maxAngularShake = 2.4f;   // degrees
        [SerializeField] private float frequency = 26f;

        private float _trauma;
        private float _seed;

        protected override void OnSingletonAwake()
        {
            _seed = Random.value * 1000f;
        }

        /// <summary>Triggers a shake impulse. duration in seconds, trauma 0..1.</summary>
        public void Shake(float duration, float trauma)
        {
            _trauma = Mathf.Clamp01(Mathf.Max(_trauma, trauma));
            if (duration > 0.01f)
            {
                CancelInvoke(nameof(Decay));
                InvokeRepeating(nameof(Decay), duration, 0.05f);
            }
        }

        private void Decay()
        {
            _trauma = Mathf.MoveTowards(_trauma, 0f, Time.deltaTime * 3.2f);
            if (_trauma <= 0f)
            {
                CancelInvoke(nameof(Decay));
            }
        }

        /// <summary>Consumed by FollowCamera to offset the final pose.</summary>
        public void ApplyShake(ref Vector3 position, ref Quaternion rotation)
        {
            if (_trauma <= 0f)
            {
                return;
            }

            float t = Time.unscaledTime * frequency;
            float shake = _trauma * _trauma;   // quadratic falloff feels punchier

            Vector3 offset = new Vector3(
                (Mathf.PerlinNoise(_seed, t) * 2f - 1f),
                (Mathf.PerlinNoise(_seed + 1f, t) * 2f - 1f),
                (Mathf.PerlinNoise(_seed + 2f, t) * 2f - 1f)) * (maxPositionalShake * shake);

            position += offset;

            Vector3 angular = new Vector3(
                (Mathf.PerlinNoise(_seed + 3f, t) * 2f - 1f),
                (Mathf.PerlinNoise(_seed + 4f, t) * 2f - 1f),
                (Mathf.PerlinNoise(_seed + 5f, t) * 2f - 1f)) * (maxAngularShake * shake);

            rotation *= Quaternion.Euler(angular);
        }

        private void Update()
        {
            if (_trauma > 0f)
            {
                _trauma = Mathf.MoveTowards(_trauma, 0f, Time.unscaledDeltaTime * 1.4f);
            }
        }
    }
}
