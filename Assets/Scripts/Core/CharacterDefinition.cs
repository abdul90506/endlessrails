using System;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Data for a playable runner: model prefab, animation set, unlock cost and
    /// cosmetic options. Create via Create &gt; Endless Rails &gt; Character.
    /// </summary>
    [CreateAssetMenu(menuName = "Endless Rails/Character Definition", fileName = "Character")]
    public class CharacterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string id = "runner_male_01";
        public string displayName = "Runner";
        public RunnerGender gender = RunnerGender.Male;

        [Header("Model")]
        [Tooltip("Optional rigged humanoid prefab. Leave empty to use the built-in procedural mannequin.")]
        public GameObject modelPrefab;

        [Tooltip("Animator Controller with the standard runner parameters. " +
                 "Leave empty when using the procedural mannequin.")]
        public RuntimeAnimatorController animatorController;

        [Header("Cosmetics")]
        public Color primaryColor = new Color(0.18f, 0.32f, 0.55f);
        public Color secondaryColor = new Color(0.85f, 0.62f, 0.18f);
        public Color skinTone = new Color(0.78f, 0.58f, 0.45f);
        public bool ponytail = false;

        [Header("Unlock")]
        [Tooltip("Cost in coins. 0 = unlocked from the start.")]
        public int coinPrice = 0;
        [Tooltip("Optional price in real-money IAP placeholder (unused).")]
        public bool featured = false;

        [Header("Audio")]
        public AudioClip[] maleFootstepOverride;   // optional per-character footsteps
        public AudioClip[] femaleFootstepOverride;
    }

    /// <summary>Serializable record of which characters the player owns.</summary>
    [Serializable]
    public class CharacterUnlockRecord
    {
        public string characterId;
        public bool unlocked;
    }
}
