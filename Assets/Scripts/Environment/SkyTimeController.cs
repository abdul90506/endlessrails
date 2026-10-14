using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Dynamic time-of-day and biome lighting controller. Progresses the sun
    /// from sunrise to dusk as the run continues, blends sky/fog colors and
    /// rotates through configured biomes for variety.
    /// </summary>
    public class SkyTimeController : Singleton<SkyTimeController>
    {
        [Header("References")]
        [SerializeField] private Light sun;
        [SerializeField] private Material skyboxMaterial;

        [Header("Time of Day")]
        [Tooltip("Seconds of real time for a full sunrise-to-dusk cycle.")]
        [SerializeField] private float dayLengthSeconds = 420f;
        [SerializeField] private Gradient sunColorOverDay;
        [SerializeField] private AnimationCurve sunIntensityOverDay = AnimationCurve.EaseInOut(0f, 0.4f, 1f, 1.4f);
        [SerializeField] private AnimationCurve fogDensityOverDay = AnimationCurve.EaseInOut(0f, 0.02f, 1f, 0.008f);

        [Header("Biomes")]
        [SerializeField] private BiomeDefinition[] biomes = new BiomeDefinition[0];
        [SerializeField] private float biomeLengthMeters = 900f;

        /// <summary>0 = sunrise, 1 = dusk.</summary>
        public float TimeOfDay01 { get; private set; }
        public BiomeDefinition CurrentBiome { get; private set; }
        public WeatherState CurrentWeather { get; private set; } = WeatherState.Clear;

        private System.Random _weatherRng;
        private float _weatherTimer;
        private int _biomeIndex = -1;

        protected override void OnSingletonAwake()
        {
            _weatherRng = new System.Random(System.Environment.TickCount);
            if (sunColorOverDay == null || sunColorOverDay.colorKeys.Length == 0)
            {
                sunColorOverDay = new Gradient
                {
                    colorKeys = new[]
                    {
                        new GradientColorKey(new Color(1f, 0.72f, 0.5f), 0f),
                        new GradientColorKey(new Color(1f, 0.96f, 0.88f), 0.5f),
                        new GradientColorKey(new Color(1f, 0.55f, 0.35f), 1f)
                    }
                };
            }
            if (biomes.Length > 0)
            {
                ApplyBiome(0);
            }
        }

        private void Start()
        {
            GameEvents.OnDistanceChanged += HandleDistanceChanged;
        }

        private void OnDestroy()
        {
            GameEvents.OnDistanceChanged -= HandleDistanceChanged;
            base.OnDestroy();
        }

        private void HandleDistanceChanged(float meters)
        {
            if (biomes == null || biomes.Length == 0)
            {
                return;
            }
            int index = Mathf.FloorToInt(meters / Mathf.Max(100f, biomeLengthMeters)) % biomes.Length;
            if (index != _biomeIndex)
            {
                ApplyBiome(index);
            }
        }

        private void ApplyBiome(int index)
        {
            _biomeIndex = index;
            CurrentBiome = biomes[index];
            RollWeather();

            if (CurrentBiome == null)
            {
                return;
            }

            if (sun != null)
            {
                sun.color = CurrentBiome.sunColor;
                sun.intensity = CurrentBiome.sunIntensity;
            }
            RenderSettings.ambientSkyColor = CurrentBiome.ambientSkyColor;
            RenderSettings.ambientEquatorColor = CurrentBiome.ambientEquatorColor;
            RenderSettings.ambientGroundColor = CurrentBiome.ambientGroundColor;
        }

        private void RollWeather()
        {
            if (CurrentBiome == null)
            {
                return;
            }
            WeatherState previous = CurrentWeather;
            CurrentWeather = CurrentBiome.RollWeather(_weatherRng);
            if (CurrentWeather != previous)
            {
                GameEvents.RaiseWeatherChanged(CurrentWeather);
                AudioManager.Instance?.SetRainActive(CurrentWeather == WeatherState.LightRain);
                WeatherController.Instance?.ApplyWeather(CurrentWeather);
            }
            _weatherTimer = Random.Range(75f, 140f);
        }

        private void Update()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || gm.State != GameState.Playing)
            {
                return;
            }

            // --- Time of day ---
            TimeOfDay01 = Mathf.PingPong(Time.time / Mathf.Max(30f, dayLengthSeconds) + gm.Distance * 0.00004f, 1f);

            if (sun != null)
            {
                float sunAngle = Mathf.Lerp(-18f, 198f, TimeOfDay01);
                sun.transform.rotation = Quaternion.Euler(sunAngle, 32f, 0f);
                if (sunColorOverDay != null)
                {
                    sun.color = Color.Lerp(sun.color, sunColorOverDay.Evaluate(TimeOfDay01), Time.deltaTime * 0.6f);
                }
                sun.intensity = Mathf.Lerp(sun.intensity, sunIntensityOverDay.Evaluate(TimeOfDay01), Time.deltaTime * 0.6f);
            }

            // --- Fog ---
            float targetFog = fogDensityOverDay.Evaluate(TimeOfDay01);
            if (CurrentBiome != null)
            {
                targetFog += CurrentBiome.fogDensity * 0.5f;
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, CurrentBiome.fogColor, Time.deltaTime * 0.35f);
            }
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fog = true;
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, targetFog, Time.deltaTime * 0.4f);

            // --- Skybox tint ---
            if (skyboxMaterial != null && CurrentBiome != null)
            {
                if (skyboxMaterial.HasProperty("_Exposure"))
                {
                    skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(skyboxMaterial.GetFloat("_Exposure"), CurrentBiome.skyExposure, Time.deltaTime * 0.3f));
                }
                if (skyboxMaterial.HasProperty("_SkyTint"))
                {
                    skyboxMaterial.SetColor("_SkyTint", Color.Lerp(skyboxMaterial.GetColor("_SkyTint"), CurrentBiome.skyTint, Time.deltaTime * 0.3f));
                }
            }

            // --- Weather cycling ---
            _weatherTimer -= Time.deltaTime;
            if (_weatherTimer <= 0f)
            {
                RollWeather();
            }
        }
    }
}
