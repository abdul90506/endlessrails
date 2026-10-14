using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Global tuning data for gameplay. Editable as a project asset
    /// (Create &gt; Endless Rails &gt; Game Config) - no code changes needed for balancing.
    /// </summary>
    [CreateAssetMenu(menuName = "Endless Rails/Game Config", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Speed")]
        [Tooltip("Forward speed at run start (m/s).")]
        [Range(4f, 30f)] public float startSpeed = 9f;
        [Tooltip("Maximum forward speed (m/s).")]
        [Range(8f, 45f)] public float maxSpeed = 26f;
        [Tooltip("Speed gained per second of play.")]
        [Range(0.02f, 1.5f)] public float speedRampPerSecond = 0.16f;
        [Tooltip("Multiplier applied to speed while airborne.")]
        [Range(0.8f, 1.4f)] public float jumpSpeedFactor = 1.02f;

        [Header("Lanes")]
        [Tooltip("World-space distance between lane centers (meters).")]
        [Range(1.4f, 4f)] public float laneWidth = 2.2f;
        [Tooltip("Seconds for a full lane change.")]
        [Range(0.05f, 0.5f)] public float laneChangeDuration = 0.16f;
        [Tooltip("Maximum degrees the body leans during a lane change.")]
        [Range(0f, 40f)] public float laneChangeLeanAngle = 16f;

        [Header("Jump / Slide")]
        [Tooltip("Initial upward velocity for a jump (m/s).")]
        [Range(3f, 14f)] public float jumpVelocity = 7.4f;
        [Tooltip("Gravity applied while jumping (m/s^2, negative).")]
        [Range(-40f, -8f)] public float gravity = -22f;
        [Tooltip("How long a slide lasts (seconds).")]
        [Range(0.25f, 2f)] public float slideDuration = 0.85f;
        [Tooltip("Cooldown before another slide can start.")]
        [Range(0f, 1f)] public float slideCooldown = 0.15f;

        [Header("Stumble / Crash")]
        [Tooltip("Speed multiplier right after a stumble.")]
        [Range(0.1f, 0.9f)] public float stumbleSpeedFactor = 0.45f;
        [Tooltip("How long the stumble slow-down lasts (seconds).")]
        [Range(0.2f, 2f)] public float stumbleDuration = 0.8f;
        [Tooltip("Seconds of slow-motion on crash.")]
        [Range(0.2f, 2f)] public float crashSlowMoDuration = 0.9f;
        [Tooltip("Time scale during the crash slow-motion.")]
        [Range(0.05f, 0.6f)] public float crashSlowMoScale = 0.28f;

        [Header("Scoring")]
        [Tooltip("Score awarded per meter travelled.")]
        [Range(0.1f, 5f)] public float scorePerMeter = 1f;
        [Tooltip("Score awarded per coin.")]
        [Range(1, 100)] public int scorePerCoin = 10;
        [Tooltip("Coin value added to the wallet per pickup.")]
        [Range(1, 20)] public int coinValue = 1;

        [Header("Powerups")]
        [Range(1f, 30f)] public float magnetDuration = 8f;
        [Range(1f, 12f)] public float magnetRadius = 6.5f;
        [Range(1f, 30f)] public float jetpackDuration = 8f;
        [Range(1f, 20f)] public float jetpackHeight = 6.5f;
        [Range(1f, 30f)] public float shieldDuration = 12f;
        [Range(1f, 30f)] public float multiplierDuration = 10f;
        [Range(1, 5)] public int multiplierValue = 2;

        [Header("Chaser")]
        [Tooltip("Distance behind the player when the run starts (meters).")]
        [Range(2f, 30f)] public float chaserStartDistance = 11f;
        [Tooltip("Closest the chaser can get without catching the player.")]
        [Range(0.5f, 5f)] public float chaserMinDistance = 1.6f;
        [Tooltip("Meters the chaser gains when the player stumbles.")]
        [Range(0.5f, 10f)] public float chaserGainOnStumble = 3.5f;
        [Tooltip("Meters per second the chaser falls behind when the player runs fast.")]
        [Range(0.05f, 2f)] public float chaserFallBehindRate = 0.28f;

        [Header("World Generation")]
        [Tooltip("Length of each generated track chunk (meters).")]
        [Range(8f, 80f)] public float chunkLength = 24f;
        [Tooltip("How many chunks stay active ahead of the player.")]
        [Range(2, 16)] public int chunksAhead = 7;
        [Tooltip("How many chunks stay active behind the player before recycling.")]
        [Range(0, 6)] public int chunksBehind = 2;
        [Tooltip("Spacing between obstacle rows inside a chunk (meters).")]
        [Range(3f, 20f)] public float rowSpacing = 7.5f;
        [Tooltip("Probability a row is skipped entirely (breathing room).")]
        [Range(0f, 0.8f)] public float emptyRowChance = 0.22f;
        [Tooltip("How fast spawn density ramps with time (0..1 difficulty per 60s).")]
        [Range(0.001f, 0.05f)] public float difficultyRampPerSecond = 0.0075f;

        [Header("Camera")]
        public float cameraDistance = 6.2f;
        public float cameraHeight = 3.1f;
        public float cameraLookAhead = 4.5f;
        [Range(0f, 1f)] public float cameraFollowSmoothing = 0.12f;
        [Range(40f, 95f)] public float baseFieldOfView = 62f;
        [Range(40f, 95f)] public float maxFieldOfView = 74f;
        [Range(0f, 1f)] public float fovSpeedContribution = 0.55f;

        [Header("Difficulty")]
        [Tooltip("Extra obstacle rows per chunk at maximum difficulty.")]
        [Range(0f, 1f)] public float maxDensityBonus = 0.45f;

        /// <summary>0..1 difficulty factor for the given elapsed run time.</summary>
        public float GetDifficulty(float elapsedSeconds)
        {
            return Mathf.Clamp01(elapsedSeconds * difficultyRampPerSecond);
        }

        /// <summary>Current forward speed for the given elapsed run time.</summary>
        public float GetSpeed(float elapsedSeconds)
        {
            return Mathf.Min(maxSpeed, startSpeed + elapsedSeconds * speedRampPerSecond);
        }
    }
}
