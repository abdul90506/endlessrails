using System;
using System.Collections.Generic;

namespace EndlessRails
{
    /// <summary>
    /// Everything that persists between sessions. Serialized to JSON at
    /// Application.persistentDataPath/save.json via JsonUtility.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int version = 1;

        // --- Lifetime stats / economy ---
        public int coinsBalance = 0;
        public int highScore = 0;
        public float bestDistance = 0f;
        public int totalCoinsCollected = 0;
        public int totalRuns = 0;
        public int totalJumps = 0;
        public int totalSlides = 0;
        public int totalPickups = 0;

        // --- Settings ---
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;
        public float ambientVolume = 0.7f;
        public float inputSensitivity = 1f;
        public int graphicsQuality = 2;        // 0 Low, 1 Medium, 2 High
        public bool firstPersonCamera = false;
        public bool showFps = false;
        public bool tutorialShown = false;

        // --- Unlocks ---
        public string selectedCharacterId = "runner_male_01";
        public List<CharacterUnlockRecord> characters = new List<CharacterUnlockRecord>();

        // --- Missions ---
        public List<MissionProgress> missions = new List<MissionProgress>();
        public long lastDailyResetUtc = 0;

        public bool IsCharacterUnlocked(string id)
        {
            foreach (var record in characters)
            {
                if (record.characterId == id)
                {
                    return record.unlocked;
                }
            }
            // Unknown records default to "locked unless price is zero" handled by caller.
            return false;
        }

        public void SetCharacterUnlocked(string id, bool unlocked)
        {
            foreach (var record in characters)
            {
                if (record.characterId == id)
                {
                    record.unlocked = unlocked;
                    return;
                }
            }
            characters.Add(new CharacterUnlockRecord { characterId = id, unlocked = unlocked });
        }
    }
}
