using System;
using System.IO;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// JSON save/load with an in-memory cache. Writes are flushed on pause,
    /// game over and quit. Tolerates corrupted files by starting fresh.
    /// </summary>
    public class SaveSystem : Singleton<SaveSystem>
    {
        private const string FileName = "save.json";
        private SaveData _data;
        private bool _dirty;

        public SaveData Data
        {
            get
            {
                if (_data == null)
                {
                    Load();
                }
                return _data;
            }
        }

        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        protected override void OnSingletonAwake()
        {
            Load();
            Application.quitting += Save;
        }

        protected override void OnDestroy()
        {
            Application.quitting -= Save;
            base.OnDestroy();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                Save();
            }
        }

        /// <summary>Loads the save file (or creates a fresh one).</summary>
        public void Load()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    string json = File.ReadAllText(SavePath);
                    _data = JsonUtility.FromJson<SaveData>(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[EndlessRails] Failed to load save data, starting fresh: {e.Message}");
            }

            if (_data == null)
            {
                _data = new SaveData();
            }
            _dirty = false;
        }

        /// <summary>Writes the current save data to disk if anything changed.</summary>
        public void Save()
        {
            if (_data == null || !_dirty)
            {
                // First write always happens (e.g. immediately after a fresh install).
                if (_data != null && File.Exists(SavePath))
                {
                    return;
                }
            }

            try
            {
                string json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(SavePath, json);
                _dirty = false;
            }
            catch (Exception e)
            {
                Debug.LogError($"[EndlessRails] Failed to write save data: {e.Message}");
            }
        }

        public void MarkDirty() => _dirty = true;

        // --- Convenience mutators (all mark dirty + raise events) ---

        public void AddCoins(int amount)
        {
            Data.coinsBalance = Mathf.Max(0, Data.coinsBalance + amount);
            _dirty = true;
            GameEvents.RaiseCoinsBalanceChanged(Data.coinsBalance);
        }

        public bool TrySpendCoins(int amount)
        {
            if (Data.coinsBalance < amount)
            {
                return false;
            }
            Data.coinsBalance -= amount;
            _dirty = true;
            GameEvents.RaiseCoinsBalanceChanged(Data.coinsBalance);
            return true;
        }

        public void RecordRun(int score, float distance, int coins, int jumps, int slides, int pickups)
        {
            Data.totalRuns++;
            Data.totalCoinsCollected += coins;
            Data.totalJumps += jumps;
            Data.totalSlides += slides;
            Data.totalPickups += pickups;
            if (score > Data.highScore)
            {
                Data.highScore = score;
            }
            if (distance > Data.bestDistance)
            {
                Data.bestDistance = distance;
            }
            _dirty = true;
            Save();
        }

        public void SetSetting<T>(Func<SaveData, T> getter, Action<SaveData, T> setter, T value)
        {
            setter(Data, value);
            _dirty = true;
        }

        /// <summary>Deletes the save file and reloads defaults (used by settings "reset").</summary>
        public void ResetAll()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[EndlessRails] Could not delete save file: {e.Message}");
            }
            _data = new SaveData();
            _dirty = true;
            Save();
            GameEvents.RaiseCoinsBalanceChanged(_data.coinsBalance);
        }
    }
}
