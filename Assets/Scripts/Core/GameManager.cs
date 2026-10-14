using System.Collections;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Run lifecycle owner: state machine, scoring, speed/difficulty ramp and
    /// the crash slow-motion sequence. Also persists run stats on game over.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private AudioLibrary audioLibrary;

        public GameConfig Config => config;
        public AudioLibrary Audio => audioLibrary;

        // --- Run state ---
        public GameState State { get; private set; } = GameState.MainMenu;
        public int Score { get; private set; }
        public int RunCoins { get; private set; }
        public float Distance { get; private set; }
        public float ElapsedRunTime { get; private set; }
        public float CurrentSpeed { get; private set; }
        public float Difficulty { get; private set; }
        public int JumpsThisRun { get; private set; }
        public int SlidesThisRun { get; private set; }
        public int PickupsThisRun { get; private set; }
        public int ScoreMultiplier { get; private set; } = 1;

        public bool IsPlaying => State == GameState.Playing;
        public bool ShieldActive { get; private set; }

        private Coroutine _crashRoutine;
        private bool _initialized;
        private int _cashedCoins;

        /// <summary>Coins earned this run that have not yet been added to the wallet.</summary>
        public int UncashedCoins => Mathf.Max(0, RunCoins - _cashedCoins);

        /// <summary>Moves run coins into the wallet exactly once (survives revives).</summary>
        public int CashOutRunCoins()
        {
            int amount = UncashedCoins;
            _cashedCoins = RunCoins;
            return amount;
        }

        public void SetConfig(GameConfig newConfig, AudioLibrary newAudio)
        {
            if (newConfig != null) config = newConfig;
            if (newAudio != null) audioLibrary = newAudio;
            _initialized = config != null;
        }

        protected override void OnSingletonAwake()
        {
            if (config == null)
            {
                Debug.LogError("[EndlessRails] GameManager has no GameConfig assigned!");
            }
            CurrentSpeed = config != null ? config.startSpeed : 9f;
        }

        private void Start()
        {
            SetState(GameState.MainMenu);
        }

        private void Update()
        {
            if (State != GameState.Playing)
            {
                return;
            }

            ElapsedRunTime += Time.deltaTime;

            // Speed + difficulty ramp.
            float previousSpeed = CurrentSpeed;
            CurrentSpeed = config.GetSpeed(ElapsedRunTime);
            Difficulty = config.GetDifficulty(ElapsedRunTime);
            if (!Mathf.Approximately(previousSpeed, CurrentSpeed))
            {
                GameEvents.RaiseSpeedChanged(CurrentSpeed);
            }

            // Score accrues with distance travelled.
            float deltaMeters = CurrentSpeed * Time.deltaTime;
            Distance += deltaMeters;
            int deltaScore = Mathf.RoundToInt(deltaMeters * config.scorePerMeter * ScoreMultiplier);
            if (deltaScore > 0)
            {
                Score += deltaScore;
                GameEvents.RaiseScoreChanged(Score);
            }
            GameEvents.RaiseDistanceChanged(Distance);
        }

        // --- Public run control ---

        public void StartRun()
        {
            Score = 0;
            RunCoins = 0;
            Distance = 0f;
            ElapsedRunTime = 0f;
            JumpsThisRun = 0;
            SlidesThisRun = 0;
            PickupsThisRun = 0;
            _cashedCoins = 0;
            ScoreMultiplier = 1;
            ShieldActive = false;
            CurrentSpeed = config.startSpeed;
            Difficulty = 0f;
            Time.timeScale = 1f;

            SetState(GameState.Playing);
            GameEvents.RaiseRunStarted();
            GameEvents.RaiseScoreChanged(Score);
            GameEvents.RaiseCoinsChanged(RunCoins);
            GameEvents.RaiseSpeedChanged(CurrentSpeed);
        }

        public void PauseGame()
        {
            if (State != GameState.Playing)
            {
                return;
            }
            Time.timeScale = 0f;
            SetState(GameState.Paused);
            SaveSystem.Instance?.Save();
        }

        public void ResumeGame()
        {
            if (State != GameState.Paused)
            {
                return;
            }
            Time.timeScale = 1f;
            SetState(GameState.Playing);
        }

        public void RestartRun()
        {
            Time.timeScale = 1f;
            StartRun();
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            SetState(GameState.MainMenu);
            SaveSystem.Instance?.Save();
        }

        /// <summary>Ends the run (crash or caught). Starts slow-motion crash moment.</summary>
        public void TriggerGameOver()
        {
            if (State != GameState.Playing)
            {
                return;
            }

            SetState(GameState.GameOver);
            GameEvents.RaisePlayerCrashed();

            if (_crashRoutine != null)
            {
                StopCoroutine(_crashRoutine);
            }
            _crashRoutine = StartCoroutine(CrashSequence());
        }

        /// <summary>Continues after watching an ad placeholder.</summary>
        public void Revive()
        {
            if (State != GameState.GameOver)
            {
                return;
            }
            Time.timeScale = 1f;
            SetState(GameState.Playing);
            GameEvents.RaiseRunRevived();
        }

        private IEnumerator CrashSequence()
        {
            // Short slow-motion crash moment, then persist stats and raise the UI event.
            Time.timeScale = config.crashSlowMoScale;
            yield return new WaitForSecondsRealtime(config.crashSlowMoDuration);
            Time.timeScale = 1f;

            // Persist stats + wallet exactly once per crash (revive-safe).
            int coinsToCash = CashOutRunCoins();
            SaveSystem.Instance?.AddCoins(coinsToCash);
            SaveSystem.Instance?.RecordRun(
                Score, Distance, coinsToCash, JumpsThisRun, SlidesThisRun, PickupsThisRun);

            GameEvents.RaiseGameOver(Score);
            _crashRoutine = null;
        }

        // --- Mutation helpers (called by gameplay systems) ---

        public void AddCoin()
        {
            RunCoins += config.coinValue;
            Score += config.scorePerCoin * ScoreMultiplier;
            GameEvents.RaiseCoinsChanged(RunCoins);
            GameEvents.RaiseScoreChanged(Score);
        }

        public void RegisterJump() => JumpsThisRun++;
        public void RegisterSlide() => SlidesThisRun++;

        public void RegisterPickup()
        {
            PickupsThisRun++;
        }

        public void SetScoreMultiplier(int value)
        {
            ScoreMultiplier = Mathf.Max(1, value);
        }

        public void SetShieldActive(bool active)
        {
            ShieldActive = active;
        }

        public bool ConsumeShield()
        {
            if (!ShieldActive)
            {
                return false;
            }
            ShieldActive = false;
            GameEvents.RaiseShieldBroken();
            return true;
        }

        private void SetState(GameState newState)
        {
            State = newState;
            GameEvents.RaiseGameStateChanged(newState);
        }
    }
}
