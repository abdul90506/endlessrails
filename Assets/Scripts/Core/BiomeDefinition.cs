using System;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Environment theme data: sky/lighting colors, fog, weather odds and
    /// material tinting for one stretch of city. Biomes cycle as the run progresses.
    /// </summary>
    [CreateAssetMenu(menuName = "Endless Rails/Biome Definition", fileName = "Biome")]
    public class BiomeDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string id = "city_day";
        public string displayName = "City Line";

        [Header("Lighting")]
        public Color sunColor = new Color(1f, 0.96f, 0.88f);
        [Range(0f, 4f)] public float sunIntensity = 1.25f;
        public Color ambientSkyColor = new Color(0.52f, 0.62f, 0.78f);
        public Color ambientEquatorColor = new Color(0.42f, 0.44f, 0.48f);
        public Color ambientGroundColor = new Color(0.18f, 0.17f, 0.16f);

        [Header("Sky & Fog")]
        [Tooltip("Skybox tint multiplier.")]
        public Color skyTint = new Color(0.55f, 0.7f, 1f);
        [Range(0f, 4f)] public float skyExposure = 1.1f;
        public Color fogColor = new Color(0.72f, 0.78f, 0.85f);
        [Range(0f, 0.1f)] public float fogDensity = 0.012f;

        [Header("Weather")]
        [Tooltip("Weighted odds for each weather state in this biome.")]
        public WeatherWeight[] weatherWeights =
        {
            new WeatherWeight { state = WeatherState.Clear, weight = 0.6f },
            new WeatherWeight { state = WeatherState.Overcast, weight = 0.25f },
            new WeatherWeight { state = WeatherState.LightRain, weight = 0.15f }
        };

        [Header("Environment")]
        [Tooltip("Randomly chosen for track props when this biome is active.")]
        public Color[] graffitiTints = { new Color(0.8f, 0.3f, 0.3f), new Color(0.3f, 0.6f, 0.9f) };
        [Range(0f, 1f)] public float wetSurfaceChance = 0.15f;

        [Serializable]
        public class WeatherWeight
        {
            public WeatherState state = WeatherState.Clear;
            [Range(0f, 10f)] public float weight = 1f;
        }

        public WeatherState RollWeather(System.Random rng)
        {
            float total = 0f;
            foreach (var w in weatherWeights) total += Mathf.Max(0f, w.weight);
            if (total <= 0f) return WeatherState.Clear;
            float roll = (float)rng.NextDouble() * total;
            foreach (var w in weatherWeights)
            {
                roll -= Mathf.Max(0f, w.weight);
                if (roll <= 0f) return w.state;
            }
            return WeatherState.Clear;
        }
    }
}
