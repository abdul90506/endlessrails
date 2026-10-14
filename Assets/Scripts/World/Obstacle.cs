using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Base behaviour for all track obstacles. Configured by an
    /// <see cref="ObstacleDefinition"/> asset; supports optional lateral motion
    /// for passing trains and crossing vehicles. Fully poolable.
    /// </summary>
    public class Obstacle : MonoBehaviour, IPooled
    {
        [SerializeField] private ObstacleDefinition definition;

        // Fallback values when no definition asset is assigned.
        [SerializeField] private ObstacleImpact impact = ObstacleImpact.Lethal;
        [SerializeField] private bool mustSlideUnder;
        [SerializeField] private bool canJumpOver = true;
        [SerializeField] private float clearanceHeight = 1.2f;
        [SerializeField] private bool moves;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float moveRange = 60f;
        [SerializeField] private Vector3 moveDirection = Vector3.right;
        [SerializeField] private AudioSource loopSource;

        public ObstacleImpact Impact => definition != null ? definition.impact : impact;
        public bool MustSlideUnder => definition != null ? definition.mustSlideUnder : mustSlideUnder;
        public bool CanJumpOver => definition != null ? definition.canJumpOver : canJumpOver;
        public float ClearanceHeight => clearanceHeight;

        private Vector3 _origin;
        private float _moved;
        private bool _active;

        private void Awake()
        {
            if (definition != null)
            {
                impact = definition.impact;
                mustSlideUnder = definition.mustSlideUnder;
                canJumpOver = definition.canJumpOver;
                moves = definition.moves;
                moveSpeed = definition.moveSpeed;
                moveRange = definition.moveRange;
            }
        }

        public void Configure(ObstacleDefinition def)
        {
            definition = def;
            if (def != null)
            {
                impact = def.impact;
                mustSlideUnder = def.mustSlideUnder;
                canJumpOver = def.canJumpOver;
                moves = def.moves;
                moveSpeed = def.moveSpeed;
                moveRange = def.moveRange;
            }
        }

        public void OnSpawned()
        {
            _origin = transform.position;
            _moved = 0f;
            _active = true;

            if (moves && loopSource != null && definition != null && definition.loopClip != null)
            {
                loopSource.clip = definition.loopClip;
                loopSource.volume = definition.loopVolume;
                loopSource.loop = true;
                loopSource.spatialBlend = 1f;
                loopSource.Play();
            }
        }

        public void OnRecycled()
        {
            _active = false;
            if (loopSource != null && loopSource.isPlaying)
            {
                loopSource.Stop();
            }
        }

        private void Update()
        {
            if (!_active || !moves)
            {
                return;
            }

            float step = moveSpeed * Time.deltaTime;
            transform.Translate(moveDirection.normalized * step, Space.World);
            _moved += step;

            if (_moved >= moveRange)
            {
                // Trains/vehicles that complete their pass despawn cleanly.
                if (PoolManager.Exists)
                {
                    PoolManager.Instance.Recycle(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>Visual/audio response when a shield absorbs a hit with this obstacle.</summary>
        public void NotifyShieldedHit()
        {
            // Brief flash so the player understands the shield saved them.
            if (TryGetComponent(out Renderer renderer))
            {
                StartCoroutine(FlashRoutine(renderer));
            }
        }

        private System.Collections.IEnumerator FlashRoutine(Renderer renderer)
        {
            Material mat = renderer.material;
            Color original = mat.color;
            mat.color = Color.white;
            yield return new WaitForSeconds(0.12f);
            if (mat != null)
            {
                mat.color = original;
            }
        }

        /// <summary>Approach audio cue (train horn) triggered by the world spawner.</summary>
        public void PlayApproachCue()
        {
            if (definition != null && definition.approachClip != null)
            {
                AudioManager.Instance?.PlayClipAt(definition.approachClip, transform.position, 1f);
            }
        }
    }
}
