using System.Collections.Generic;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Generic object pool with prewarm support. Zero allocation during steady
    /// state - all spawning/recycling goes through cached lists.
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private class Pool
        {
            public GameObject Prefab;
            public readonly Stack<GameObject> Instances = new Stack<GameObject>(64);
            public readonly HashSet<GameObject> All = new HashSet<GameObject>();
            public Transform Root;
        }

        private readonly Dictionary<int, Pool> _pools = new Dictionary<int, Pool>(32);
        private Transform _poolRoot;

        protected override void OnSingletonAwake()
        {
            _poolRoot = new GameObject("PoolRoot").transform;
            _poolRoot.SetParent(transform);
        }

        /// <summary>Pre-creates instances so the first spawns never hitch.</summary>
        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null)
            {
                return;
            }
            Pool pool = GetOrCreatePool(prefab);
            for (int i = 0; i < count; i++)
            {
                GameObject instance = CreateInstance(pool);
                instance.SetActive(false);
                pool.Instances.Push(instance);
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                return null;
            }

            Pool pool = GetOrCreatePool(prefab);
            GameObject instance = pool.Instances.Count > 0 ? pool.Instances.Pop() : CreateInstance(pool);

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);

            if (instance.TryGetComponent(out IPooled pooled))
            {
                pooled.OnSpawned();
            }
            return instance;
        }

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            GameObject instance = Spawn(prefab.gameObject, position, rotation);
            return instance != null ? instance.GetComponent<T>() : null;
        }

        public void Recycle(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            IPooled pooled = instance.GetComponent<IPooled>();
            pooled?.OnRecycled();

            instance.SetActive(false);
            instance.transform.SetParent(_poolRoot, false);

            // Return to the pool that owns it.
            foreach (var kvp in _pools)
            {
                if (kvp.Value.All.Contains(instance))
                {
                    kvp.Value.Instances.Push(instance);
                    return;
                }
            }

            // Unknown instance - just destroy it to be safe.
            Destroy(instance);
        }

        private Pool GetOrCreatePool(GameObject prefab)
        {
            int key = prefab.GetInstanceID();
            if (!_pools.TryGetValue(key, out Pool pool))
            {
                pool = new Pool
                {
                    Prefab = prefab,
                    Root = new GameObject($"Pool_{prefab.name}").transform
                };
                pool.Root.SetParent(_poolRoot, false);
                _pools[key] = pool;
            }
            return pool;
        }

        private GameObject CreateInstance(Pool pool)
        {
            GameObject instance = Instantiate(pool.Prefab, pool.Root);
            instance.name = pool.Prefab.name;
            pool.All.Add(instance);
            return instance;
        }

        /// <summary>Destroys every pooled instance (used between scene loads).</summary>
        public void ClearAll()
        {
            foreach (var pool in _pools.Values)
            {
                foreach (var instance in pool.All)
                {
                    if (instance != null)
                    {
                        Destroy(instance);
                    }
                }
                pool.All.Clear();
                pool.Instances.Clear();
            }
            _pools.Clear();
        }
    }

    /// <summary>Optional lifecycle hooks for pooled objects.</summary>
    public interface IPooled
    {
        void OnSpawned();
        void OnRecycled();
    }
}
