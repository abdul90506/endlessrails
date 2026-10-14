using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Spinning collectible coin with magnet attraction and pooled lifecycle.
    /// Visuals: metallic disc + optional glint particle on pickup.
    /// </summary>
    public class Coin : MonoBehaviour, IPooled
    {
        [SerializeField] private float spinSpeed = 130f;          // degrees/sec around Y
        [SerializeField] private float bobAmplitude = 0.09f;
        [SerializeField] private float bobFrequency = 1.7f;
        [SerializeField] private ParticleSystem pickupSparkle;     // optional prefab-based FX

        private Vector3 _basePosition;
        private float _phase;
        private bool _collected;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _phase = Random.value * Mathf.PI * 2f;
        }

        public void OnSpawned()
        {
            _collected = false;
            _basePosition = _transform.position;
        }

        public void OnRecycled()
        {
            _collected = false;
        }

        private void Update()
        {
            if (_collected)
            {
                return;
            }

            // Spin + gentle bob (time-based so pause freezes it naturally).
            float time = Time.time;
            _transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            Vector3 pos = _basePosition;
            pos.y += Mathf.Sin(time * bobFrequency + _phase) * bobAmplitude;
            _transform.position = pos;
        }

        /// <summary>Magnet pull: glides toward the player.</summary>
        public void PullToward(Vector3 target, float deltaTime)
        {
            if (_collected)
            {
                return;
            }
            _transform.position = Vector3.Lerp(_transform.position, target, deltaTime * 7f);
            _basePosition = _transform.position;
        }

        /// <summary>Called by PlayerCollisionHandler on trigger contact.</summary>
        public void Collect()
        {
            if (_collected)
            {
                return;
            }
            _collected = true;

            GameManager.Instance?.AddCoin();
            GameEvents.RaiseCoinCollected(GameManager.Instance != null ? GameManager.Instance.Config.coinValue : 1);
            AudioManager.Instance?.PlayCoin(transform.position);

            if (pickupSparkle != null)
            {
                // FX instances are pooled separately so coins never allocate.
                ParticleSystem fx = Instantiate(pickupSparkle, transform.position, Quaternion.identity);
                fx.Play();
                Destroy(fx.gameObject, 1.2f);
            }

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
}
