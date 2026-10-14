using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Configuration for a single obstacle prefab: impact type, size hints and
    /// optional visual/audio overrides. Attach to obstacle prefab roots.
    /// </summary>
    [CreateAssetMenu(menuName = "Endless Rails/Obstacle Definition", fileName = "Obstacle")]
    public class ObstacleDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string id = "obstacle";
        public string displayName = "Obstacle";

        [Header("Gameplay")]
        [Tooltip("Lethal = game over on hit. Stumble = player recovers but loses ground.")]
        public ObstacleImpact impact = ObstacleImpact.Lethal;

        [Tooltip("World-space footprint length along the track (meters). Trains are long.")]
        [Range(0.3f, 60f)] public float length = 1.2f;

        [Tooltip("Height in meters - used to decide if the player can jump over.")]
        [Range(0.1f, 6f)] public float height = 1.1f;

        [Tooltip("If true, the obstacle has a clear zone underneath and the player must slide.")]
        public bool mustSlideUnder = false;

        [Tooltip("If true, the obstacle can be jumped over.")]
        public bool canJumpOver = true;

        [Header("Movement (trains / vehicles)")]
        public bool moves = false;
        [Tooltip("Lateral speed for crossing vehicles (m/s).")]
        [Range(0f, 25f)] public float moveSpeed = 6f;
        [Tooltip("Distance a moving obstacle travels before despawning (meters).")]
        [Range(5f, 200f)] public float moveRange = 60f;

        [Header("Audio")]
        public AudioClip loopClip;      // e.g. train rumble
        public AudioClip approachClip;  // e.g. train horn
        [Range(0f, 1f)] public float loopVolume = 0.6f;
    }
}
