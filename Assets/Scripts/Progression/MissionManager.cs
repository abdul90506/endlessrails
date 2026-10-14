using System.Collections.Generic;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Tracks mission progress across runs, persists it via SaveSystem and
    /// grants coin rewards on completion. Daily missions reset every 24h.
    /// </summary>
    public class MissionManager : Singleton<MissionManager>
    {
        [SerializeField] private MissionDefinition[] missionPool = new MissionDefinition[0];
        [SerializeField] private int activeMissionCount = 3;

        public IReadOnlyList<MissionDefinition> ActiveMissions => _activeMissions;

        private readonly List<MissionDefinition> _activeMissions = new List<MissionDefinition>(4);
        private readonly Dictionary<string, MissionDefinition> _byId = new Dictionary<string, MissionDefinition>(16);

        protected override void OnSingletonAwake()
        {
            foreach (var mission in missionPool)
            {
                if (mission != null)
                {
                    _byId[mission.id] = mission;
                }
            }
        }

        private void Start()
        {
            RefreshActiveMissions();
            GameEvents.OnCoinCollected += HandleCoinCollected;
            GameEvents.OnPickupCollected += HandlePickupCollected;
            GameEvents.OnJumped += HandleJumped;
            GameEvents.OnSlideStarted += HandleSlide;
            GameEvents.OnGameOver += HandleGameOver;
        }

        private void OnDestroy()
        {
            GameEvents.OnCoinCollected -= HandleCoinCollected;
            GameEvents.OnPickupCollected -= HandlePickupCollected;
            GameEvents.OnJumped -= HandleJumped;
            GameEvents.OnSlideStarted -= HandleSlide;
            GameEvents.OnGameOver -= HandleGameOver;
            base.OnDestroy();
        }

        /// <summary>Loads persisted progress and picks the active mission set.</summary>
        public void RefreshActiveMissions()
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null)
            {
                return;
            }

            MaybeResetDailies(save);

            _activeMissions.Clear();
            if (save.Data.missions.Count == 0)
            {
                // First launch: take the first N missions from the pool.
                for (int i = 0; i < Mathf.Min(activeMissionCount, missionPool.Length); i++)
                {
                    var progress = new MissionProgress(missionPool[i]);
                    save.Data.missions.Add(progress);
                    _activeMissions.Add(missionPool[i]);
                }
                save.MarkDirty();
            }
            else
            {
                foreach (var progress in save.Data.missions)
                {
                    if (_byId.TryGetValue(progress.missionId, out MissionDefinition mission))
                    {
                        _activeMissions.Add(mission);
                    }
                }
            }
        }

        private void MaybeResetDailies(SaveSystem save)
        {
            long now = System.DateTime.UtcNow.Ticks;
            long oneDay = System.TimeSpan.TicksPerDay;
            if (save.Data.lastDailyResetUtc == 0)
            {
                save.Data.lastDailyResetUtc = now;
                save.MarkDirty();
                return;
            }

            if (now - save.Data.lastDailyResetUtc >= oneDay)
            {
                // Reset daily mission progress; standard missions persist.
                foreach (var progress in save.Data.missions)
                {
                    if (_byId.TryGetValue(progress.missionId, out var def) && def.dailyOnly)
                    {
                        progress.current = 0;
                        progress.completed = false;
                        progress.claimed = false;
                    }
                }
                save.Data.lastDailyResetUtc = now;
                save.MarkDirty();
            }
        }

        // --- Progress events (fired during play) ---

        private void HandleCoinCollected(int amount) => AddProgress(MissionType.CollectCoins, amount);
        private void HandlePickupCollected(PickupType type) => AddProgress(MissionType.CollectPickups, 1);
        private void HandleJumped() => AddProgress(MissionType.JumpCount, 1);
        private void HandleSlide() => AddProgress(MissionType.SlideCount, 1);

        private void HandleGameOver(int finalScore)
        {
            GameManager gm = GameManager.Instance;
            if (gm != null)
            {
                AddProgress(MissionType.RunDistance, Mathf.RoundToInt(gm.Distance));
                AddProgress(MissionType.ReachScore, finalScore);   // cumulative-style: adds up
                AddProgress(MissionType.PlayRuns, 1);
            }
            SaveSystem.Instance?.Save();
        }

        private void AddProgress(MissionType type, int amount)
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null || amount <= 0)
            {
                return;
            }

            foreach (var progress in save.Data.missions)
            {
                if (!_byId.TryGetValue(progress.missionId, out MissionDefinition mission))
                {
                    continue;
                }
                if (mission.type != type || progress.completed)
                {
                    continue;
                }

                progress.current += amount;
                if (progress.current >= mission.target)
                {
                    progress.current = mission.target;
                    progress.completed = true;
                    GrantReward(mission, progress);
                }
                save.MarkDirty();
            }
        }

        private void GrantReward(MissionDefinition mission, MissionProgress progress)
        {
            if (progress.claimed)
            {
                return;
            }
            progress.claimed = true;
            SaveSystem.Instance?.AddCoins(mission.coinReward);
            GameEvents.RaiseMissionCompleted(mission);
        }

        /// <summary>0..1 completion fraction for HUD mission widgets.</summary>
        public float GetProgressFraction(MissionDefinition mission)
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null || mission == null)
            {
                return 0f;
            }
            foreach (var progress in save.Data.missions)
            {
                if (progress.missionId == mission.id)
                {
                    return Mathf.Clamp01((float)progress.current / Mathf.Max(1, mission.target));
                }
            }
            return 0f;
        }

        public int GetProgressCurrent(MissionDefinition mission)
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null || mission == null)
            {
                return 0;
            }
            foreach (var progress in save.Data.missions)
            {
                if (progress.missionId == mission.id)
                {
                    return progress.current;
                }
            }
            return 0;
        }
    }
}
