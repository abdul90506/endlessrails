using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Lightweight MonoBehaviour singleton base. Uses lazy instance lookup so
    /// scene-placed managers are found automatically without manual wiring.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        private static bool _quitting;

        public static T Instance
        {
            get
            {
                if (_instance == null && !_quitting)
                {
                    _instance = FindFirstObjectByType<T>();
                }
                return _instance;
            }
        }

        public static bool Exists => _instance != null;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != (T)this)
            {
                Debug.LogWarning($"[EndlessRails] Duplicate {typeof(T).Name} destroyed on '{name}'.");
                Destroy(gameObject);
                return;
            }
            _instance = (T)this;
            OnSingletonAwake();
        }

        /// <summary>Override instead of Awake to keep singleton bookkeeping intact.</summary>
        protected virtual void OnSingletonAwake() { }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == (T)this)
            {
                _instance = null;
            }
        }
    }
}
