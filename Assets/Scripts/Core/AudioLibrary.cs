using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Central audio clip database. Every sound the game plays is referenced here,
    /// so audio can be swapped without touching code.
    /// </summary>
    [CreateAssetMenu(menuName = "Endless Rails/Audio Library", fileName = "AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        [Header("Footsteps")]
        public AudioClip[] footstepConcrete;
        public AudioClip[] footstepGravel;
        public AudioClip[] footstepMetal;
        public AudioClip[] footstepWet;

        [Header("Movement")]
        public AudioClip jumpWhoosh;
        public AudioClip landThud;
        public AudioClip slideScrape;
        public AudioClip stumble;
        public AudioClip crash;
        public AudioClip laneWhoosh;

        [Header("Coins & Pickups")]
        public AudioClip coinPickup;
        public AudioClip coinMulti;
        public AudioClip powerup;
        public AudioClip powerdown;
        public AudioClip shieldBreak;

        [Header("World")]
        public AudioClip trainHorn;
        public AudioClip trainRumble;
        public AudioClip vehiclePass;
        public AudioClip chaserSteps;
        public AudioClip chaserShout;

        [Header("Ambience")]
        public AudioClip windAmbient;
        public AudioClip rainAmbient;
        public AudioClip cityAmbient;

        [Header("Music")]
        public AudioClip musicLoop;

        [Header("UI")]
        public AudioClip buttonClick;
        public AudioClip uiSwoosh;
        public AudioClip gameOverSting;

        /// <summary>Returns the right footstep array for a surface, with fallbacks.</summary>
        public AudioClip[] GetFootsteps(SurfaceType surface)
        {
            return surface switch
            {
                SurfaceType.Gravel => (footstepGravel is { Length: > 0 }) ? footstepGravel : footstepConcrete,
                SurfaceType.Metal => (footstepMetal is { Length: > 0 }) ? footstepMetal : footstepConcrete,
                SurfaceType.Wet => (footstepWet is { Length: > 0 }) ? footstepWet : footstepConcrete,
                _ => (footstepConcrete is { Length: > 0 }) ? footstepConcrete : null
            };
        }
    }
}
