using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Weather FX controller: rain particle volumes that follow the camera,
    /// wet-surface smoothness boost, and weather-specific ambience.
    /// </summary>
    public class WeatherController : Singleton<WeatherController>
    {
        [Header("Rain FX")]
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private ParticleSystem splashParticles;
        [SerializeField] private Transform followTarget;

        [Header("Wet Look")]
        [Tooltip("Materials whose smoothness rises when wet (wet shader response).")]
        [SerializeField] private Renderer[] wettableRenderers = new Renderer[0];
        [SerializeField, Range(0f, 1f)] private float wetSmoothness = 0.82f;
        [SerializeField, Range(0f, 1f)] private float drySmoothness = 0.35f;

        [Header("Wind")]
        [SerializeField] private AudioSource windSource;

        public WeatherState CurrentState { get; private set; } = WeatherState.Clear;

        private float _smoothnessLerp;

        public void SetFollowTarget(Transform target) => followTarget = target;

        public void ApplyWeather(WeatherState state)
        {
            CurrentState = state;

            bool rain = state == WeatherState.LightRain;
            if (rainParticles != null)
            {
                if (rain && !rainParticles.isPlaying) rainParticles.Play();
                if (!rain && rainParticles.isPlaying) rainParticles.Stop();
            }
            if (splashParticles != null)
            {
                if (rain && !splashParticles.isPlaying) splashParticles.Play();
                if (!rain && splashParticles.isPlaying) splashParticles.Stop();
            }

            if (windSource != null)
            {
                windSource.volume = state == WeatherState.Clear ? 0.12f : 0.3f;
            }
        }

        private void Update()
        {
            // Keep rain volumes on the camera so they always cover the view.
            if (followTarget == null)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    followTarget = cam.transform;
                }
                if (followTarget == null)
                {
                    return;
                }
            }

            if (rainParticles != null && rainParticles.isPlaying)
            {
                var shape = rainParticles.shape;
                shape.position = followTarget.position + Vector3.up * 8f;
                if (splashParticles != null && splashParticles.isPlaying)
                {
                    var splashShape = splashParticles.shape;
                    splashShape.position = followTarget.position;
                }
            }

            // Wet material response.
            float target = CurrentState == WeatherState.LightRain ? wetSmoothness : drySmoothness;
            _smoothnessLerp = Mathf.Lerp(_smoothnessLerp, target, Time.deltaTime * 0.25f);
            foreach (var renderer in wettableRenderers)
            {
                if (renderer == null)
                {
                    continue;
                }
                Material material = renderer.material;
                if (material.HasProperty("_Smoothness"))
                {
                    material.SetFloat("_Smoothness", _smoothnessLerp);
                }
            }
        }
    }
}
