using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Power-up pickup: magnet, jetpack, shield or score multiplier.
    /// Applies its effect through PlayerPowerups and recycles itself.
    /// </summary>
    public class Pickup : MonoBehaviour, IPooled
    {
        [SerializeField] private PickupType type = PickupType.Magnet;
        [SerializeField] private float spinSpeed = 85f;
        [SerializeField] private float floatAmplitude = 0.14f;

        public PickupType Type => type;

        private Vector3 _basePosition;
        private bool _collected;

        public void Configure(PickupType pickupType)
        {
            type = pickupType;
        }

        public void OnSpawned()
        {
            _collected = false;
            _basePosition = transform.position;
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
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            Vector3 pos = _basePosition;
            pos.y += Mathf.Sin(Time.time * 2.1f) * floatAmplitude;
            transform.position = pos;
        }

        public void Collect(PlayerController player)
        {
            if (_collected)
            {
                return;
            }
            _collected = true;

            player?.Powerups?.Activate(type);
            AudioManager.Instance?.PlayPowerup(transform.position);

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
