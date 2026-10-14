using System;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// A mission / daily challenge definition. Create a set of these and assign
    /// them to <see cref="MissionManager"/>; progress is saved between sessions.
    /// </summary>
    [CreateAssetMenu(menuName = "Endless Rails/Mission", fileName = "Mission")]
    public class MissionDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string id = "collect_500_coins";
        public string displayName = "Coin Collector";
        [TextArea] public string description = "Collect 500 coins in total.";

        [Header("Objective")]
        public MissionType type = MissionType.CollectCoins;
        [Tooltip("Target amount for the objective.")]
        public int target = 500;

        [Header("Reward")]
        public int coinReward = 150;

        [Header("Scheduling")]
        [Tooltip("Mission only appears as a daily challenge.")]
        public bool dailyOnly = false;

        public string GetDescriptionWithTarget()
        {
            return type switch
            {
                MissionType.RunDistance => $"{description} ({target} m)",
                _ => $"{description} ({target})"
            };
        }
    }

    /// <summary>Runtime + persisted progress for one mission.</summary>
    [Serializable]
    public class MissionProgress
    {
        public string missionId;
        public int current;
        public bool completed;
        public bool claimed;
        public long startedUtc;

        public MissionProgress() { }

        public MissionProgress(MissionDefinition def)
        {
            missionId = def.id;
            startedUtc = DateTime.UtcNow.Ticks;
        }
    }
}
