using System;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// A single 3-lane row of track content. Cell values are indices into
    /// <see cref="TrackCell"/>. Guarantees: patterns authored in the editor
    /// always leave at least one lane passable (validated on import).
    /// </summary>
    [Serializable]
    public class TrackRow
    {
        public TrackCell left = TrackCell.Empty;
        public TrackCell center = TrackCell.Empty;
        public TrackCell right = TrackCell.Empty;

        public TrackCell Get(int lane) => lane switch
        {
            -1 => left,
            0 => center,
            _ => right
        };

        /// <summary>True when at least one lane has no lethal/full-block cell.</summary>
        public bool HasPassableLane()
        {
            return IsPassable(left) || IsPassable(center) || IsPassable(right);
        }

        public static bool IsPassable(TrackCell cell)
        {
            return cell == TrackCell.Empty
                || cell == TrackCell.Coin
                || cell == TrackCell.BarrierJump
                || cell == TrackCell.BarrierSlide
                || cell == TrackCell.Cone
                || IsPickup(cell);
        }

        public static bool IsPickup(TrackCell cell)
        {
            return cell == TrackCell.PickupMagnet
                || cell == TrackCell.PickupJetpack
                || cell == TrackCell.PickupShield
                || cell == TrackCell.PickupMultiplier;
        }
    }

    /// <summary>
    /// ScriptableObject describing a reusable obstacle/coin layout snippet.
    /// The chunk spawner stitches these together procedurally so the track
    /// never ends and rarely repeats identically.
    /// </summary>
    [CreateAssetMenu(menuName = "Endless Rails/Track Pattern", fileName = "Pattern")]
    public class PatternDefinition : ScriptableObject
    {
        [Tooltip("Rows are laid out along the track, one per rowSpacing meters.")]
        public TrackRow[] rows = { new TrackRow(), new TrackRow() };

        [Tooltip("Relative spawn weight at low difficulty.")]
        [Range(0f, 10f)] public float baseWeight = 1f;

        [Tooltip("Additional weight per difficulty point (0..1). Lets hard layouts ramp in late.")]
        [Range(0f, 10f)] public float difficultyWeightBonus = 0f;

        [Tooltip("Minimum difficulty (0..1) before this pattern appears at all.")]
        [Range(0f, 1f)] public float minDifficulty = 0f;

        [Tooltip("True if the pattern contains trains / vehicles (used for audio cues).")]
        public bool containsTrains = false;

        public float GetWeight(float difficulty)
        {
            if (difficulty < minDifficulty) return 0f;
            return baseWeight + difficultyWeightBonus * difficulty;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var row in rows)
            {
                if (row != null && !row.HasPassableLane())
                {
                    Debug.LogWarning($"[EndlessRails] Pattern '{name}' has a row that blocks all three lanes. " +
                                     "Fix it so at least one lane is passable.", this);
                }
            }
        }
#endif
    }
}
