using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Security officer / dog chaser that follows the player from behind.
    /// Closes in when the player stumbles or runs slowly, falls behind at high
    /// speed, and triggers game over if it ever catches up. Distance drives
    /// footstep/breathing/shout audio volume.
    /// </summary>
    public class ChaserAI : Singleton<ChaserAI>
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Animator animator;              // optional rigged chaser model
        [SerializeField] private ProceduralRunner proceduralRig; // fallback mannequin

        [Header("Audio")]
        [SerializeField] private AudioSource voiceSource;        // shouts
        [SerializeField] private AudioSource footstepSource;     // running steps
        [SerializeField] private float footstepInterval = 0.34f;

        [Header("Tuning")]
        [SerializeField] private float catchDistance = 1.35f;
        [SerializeField] private float lateralFollowSpeed = 3.5f;

        /// <summary>Current gap behind the player in meters (positive = behind).</summary>
        public float DistanceBehind { get; private set; } = 11f;

        public bool IsChasing { get; private set; }

        private float _footstepTimer;
        private float _shoutTimer = 6f;
        private Vector3 _velocity;
        private bool _wasCaught;

        private GameConfig Config => GameManager.Instance != null ? GameManager.Instance.Config : null;

        private void Start()
        {
            if (player == null && PlayerExists())
            {
                player = FindFirstObjectByType<PlayerController>()?.transform;
            }
            if (proceduralRig == null)
            {
                proceduralRig = GetComponentInChildren<ProceduralRunner>();
            }
        }

        private static bool PlayerExists()
        {
            return FindFirstObjectByType<PlayerController>() != null;
        }

        public void SetPlayer(Transform target) => player = target;

        /// <summary>Resets the chaser for a new run.</summary>
        public void ResetForRun()
        {
            DistanceBehind = Config != null ? Config.chaserStartDistance : 11f;
            IsChasing = true;
            _wasCaught = false;
            _shoutTimer = Random.Range(4f, 8f);
            SnapBehindPlayer();
        }

        /// <summary>Called when the player stumbles - the chaser gains ground.</summary>
        public void NotifyPlayerStumbled()
        {
            if (Config == null)
            {
                return;
            }
            DistanceBehind = Mathf.Max(catchDistance + 0.2f, DistanceBehind - Config.chaserGainOnStumble);
            TriggerShout();
        }

        private void SnapBehindPlayer()
        {
            if (player == null)
            {
                return;
            }
            transform.position = player.position + Vector3.back * DistanceBehind + Vector3.up * 0.0f;
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }

        private void Update()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || player == null)
            {
                return;
            }

            if (gm.State == GameState.Playing)
            {
                TickChase(gm);
            }
            else if (gm.State == GameState.GameOver)
            {
                IsChasing = false;
            }
        }

        private void TickChase(GameManager gm)
        {
            GameConfig config = Config;
            float dt = Time.deltaTime;

            // --- Distance model ---
            // The chaser runs slightly faster than the player's slow speed and
            // slightly slower than max speed, so mistakes matter but speed helps.
            float playerSpeed = gm.CurrentSpeed * (1f - 0.35f * (1f - Mathf.InverseLerp(0.3f, 1f, gm.Difficulty)));
            float chaserSpeed = Mathf.Lerp(playerSpeed * 1.02f, playerSpeed * 0.96f, Mathf.InverseLerp(0.3f, 1f, gm.Difficulty));
            float relative = playerSpeed - chaserSpeed;   // >0 => chaser falls behind

            DistanceBehind += relative * dt * 0.8f;

            // Slight creep toward the player over long runs unless the player is fast.
            if (config != null)
            {
                float pressure = Mathf.Lerp(0.22f, -config.chaserFallBehindRate, Mathf.InverseLerp(0.2f, 1f, gm.Difficulty));
                DistanceBehind += pressure * dt;
            }

            DistanceBehind = Mathf.Clamp(DistanceBehind, catchDistance, 28f);

            // --- Caught? ---
            if (DistanceBehind <= catchDistance && !_wasCaught && gm.IsPlaying)
            {
                _wasCaught = true;
                IsChasing = false;
                TriggerShout();
                gm.TriggerGameOver();
                return;
            }

            // --- Position: follow the player's lane with damping ---
            Vector3 targetPosition = player.position + Vector3.back * DistanceBehind;
            targetPosition.y = 0f;
            Vector3 next = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 0.25f);
            next.x = Mathf.Lerp(transform.position.x, player.position.x, dt * lateralFollowSpeed);
            transform.position = next;

            if ((player.position - transform.position).sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), dt * 8f);
            }

            // --- Animation + audio ---
            float speed01 = config != null
                ? Mathf.InverseLerp(config.startSpeed, config.maxSpeed, gm.CurrentSpeed)
                : 0.5f;
            proceduralRig?.SetLocomotion(speed01, 0f);
            animator?.SetFloat("Speed", speed01);

            TickAudio(dt);
        }

        private void TickAudio(float dt)
        {
            // Footsteps scale with proximity (louder when close).
            _footstepTimer -= dt;
            if (_footstepTimer <= 0f)
            {
                _footstepTimer = footstepInterval;
                if (footstepSource != null && AudioManager.Exists)
                {
                    float volume = Mathf.Lerp(0.15f, 0.85f, 1f - Mathf.InverseLerp(1.5f, 16f, DistanceBehind));
                    AudioManager.Instance.PlayClipAt(AudioManager.Instance.Library.chaserSteps, transform.position, volume);
                }
            }

            // Occasional shouts when the player is within earshot.
            _shoutTimer -= dt;
            if (_shoutTimer <= 0f)
            {
                _shoutTimer = Random.Range(6f, 13f);
                if (DistanceBehind < 11f)
                {
                    TriggerShout();
                }
            }
        }

        private void TriggerShout()
        {
            if (AudioManager.Exists)
            {
                AudioManager.Instance.PlayChaserShout(transform.position);
            }
            if (voiceSource != null && AudioManager.Exists && AudioManager.Instance.Library != null)
            {
                // Local voice source keeps shouts spatialized on the chaser.
                voiceSource.pitch = Random.Range(0.94f, 1.08f);
                voiceSource.PlayOneShot(AudioManager.Instance.Library.chaserShout);
            }
        }
    }
}
