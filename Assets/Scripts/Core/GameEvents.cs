using System;

namespace EndlessRails
{
    /// <summary>
    /// Central static event hub. Decouples gameplay systems from UI/audio/save
    /// without scene-reference spaghetti. All events are raised on the main thread.
    /// </summary>
    public static class GameEvents
    {
        // --- Run lifecycle ---
        public static event Action OnRunStarted;
        public static event Action<int> OnGameOver;                 // final score
        public static event Action OnRunRevived;
        public static event Action<GameState> OnGameStateChanged;

        // --- Player actions ---
        public static event Action<int> OnLaneChanged;              // new lane index (-1,0,1)
        public static event Action OnJumped;
        public static event Action OnLanded;                        // impact strength via OnLandedHard
        public static event Action OnSlideStarted;
        public static event Action OnSlideEnded;
        public static event Action OnStumbled;
        public static event Action OnPlayerCrashed;

        // --- Score / coins ---
        public static event Action<int> OnScoreChanged;             // total score
        public static event Action<int> OnCoinsChanged;             // run coins
        public static event Action<int> OnCoinCollected;            // value
        public static event Action<float> OnDistanceChanged;        // meters

        // --- Pickups ---
        public static event Action<PickupType> OnPickupCollected;
        public static event Action<PickupType> OnPickupExpired;
        public static event Action OnShieldBroken;

        // --- World ---
        public static event Action<WeatherState> OnWeatherChanged;
        public static event Action<float> OnSpeedChanged;           // current forward speed

        // --- Progression ---
        public static event Action<MissionDefinition> OnMissionCompleted;
        public static event Action<int> OnCoinsBalanceChanged;      // wallet total

        public static void RaiseRunStarted() => OnRunStarted?.Invoke();
        public static void RaiseGameOver(int score) => OnGameOver?.Invoke(score);
        public static void RaiseRunRevived() => OnRunRevived?.Invoke();
        public static void RaiseGameStateChanged(GameState s) => OnGameStateChanged?.Invoke(s);
        public static void RaiseLaneChanged(int lane) => OnLaneChanged?.Invoke(lane);
        public static void RaiseJumped() => OnJumped?.Invoke();
        public static void RaiseLanded() => OnLanded?.Invoke();
        public static void RaiseSlideStarted() => OnSlideStarted?.Invoke();
        public static void RaiseSlideEnded() => OnSlideEnded?.Invoke();
        public static void RaiseStumbled() => OnStumbled?.Invoke();
        public static void RaisePlayerCrashed() => OnPlayerCrashed?.Invoke();
        public static void RaiseScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void RaiseCoinsChanged(int coins) => OnCoinsChanged?.Invoke(coins);
        public static void RaiseCoinCollected(int value) => OnCoinCollected?.Invoke(value);
        public static void RaiseDistanceChanged(float meters) => OnDistanceChanged?.Invoke(meters);
        public static void RaisePickupCollected(PickupType type) => OnPickupCollected?.Invoke(type);
        public static void RaisePickupExpired(PickupType type) => OnPickupExpired?.Invoke(type);
        public static void RaiseShieldBroken() => OnShieldBroken?.Invoke();
        public static void RaiseWeatherChanged(WeatherState w) => OnWeatherChanged?.Invoke(w);
        public static void RaiseSpeedChanged(float speed) => OnSpeedChanged?.Invoke(speed);
        public static void RaiseMissionCompleted(MissionDefinition m) => OnMissionCompleted?.Invoke(m);
        public static void RaiseCoinsBalanceChanged(int total) => OnCoinsBalanceChanged?.Invoke(total);

        /// <summary>Clears all subscribers (used between play sessions in the editor).</summary>
        public static void ClearAll()
        {
            OnRunStarted = null; OnGameOver = null; OnRunRevived = null; OnGameStateChanged = null;
            OnLaneChanged = null; OnJumped = null; OnLanded = null;
            OnSlideStarted = null; OnSlideEnded = null; OnStumbled = null; OnPlayerCrashed = null;
            OnScoreChanged = null; OnCoinsChanged = null; OnCoinCollected = null; OnDistanceChanged = null;
            OnPickupCollected = null; OnPickupExpired = null; OnShieldBroken = null;
            OnWeatherChanged = null; OnSpeedChanged = null;
            OnMissionCompleted = null; OnCoinsBalanceChanged = null;
        }
    }
}
