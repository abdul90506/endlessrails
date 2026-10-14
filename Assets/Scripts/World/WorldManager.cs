using System.Collections.Generic;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Chunk-based procedural track generator. Streams rows of obstacles/coins
    /// ahead of the player, recycles everything behind them through PoolManager,
    /// and stitches endless track visuals (rails, ballast, platforms, fences).
    ///
    /// Layout is data-driven: <see cref="PatternDefinition"/> assets define
    /// reusable 3-lane snippets; difficulty weights decide which appear when.
    /// </summary>
    public class WorldManager : Singleton<WorldManager>
    {
        [System.Serializable]
        public class CellBinding
        {
            public TrackCell cell;
            public GameObject prefab;
        }

        [Header("Prefab Bindings")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private GameObject pickupMagnetPrefab;
        [SerializeField] private GameObject pickupJetpackPrefab;
        [SerializeField] private GameObject pickupShieldPrefab;
        [SerializeField] private GameObject pickupMultiplierPrefab;
        [SerializeField] private GameObject trackSegmentPrefab;
        [SerializeField] private CellBinding[] obstacleBindings = new CellBinding[0];

        [Header("Pattern Library")]
        [SerializeField] private PatternDefinition[] patterns = new PatternDefinition[0];

        [Header("Spawn Tuning")]
        [SerializeField] private float coinRunLength = 4;          // coins per line
        [SerializeField] private float pickupChance = 0.045f;      // per row
        [SerializeField] private float warmupDistance = 60f;       // easy intro
        [SerializeField] private float recycleMargin = 18f;

        private readonly Dictionary<TrackCell, GameObject> _prefabByCell = new Dictionary<TrackCell, GameObject>(16);
        private readonly List<RowInstance> _rows = new List<RowInstance>(128);
        private readonly Stack<RowInstance> _rowPool = new Stack<RowInstance>(64);
        private readonly List<GameObject> _segments = new List<GameObject>(32);

        private float _nextRowZ;
        private float _nextSegmentZ;
        private float _segmentLength = 24f;
        private Transform _player;
        private System.Random _patternRng;
        private readonly float[] _lastTrainZ = { -999f, -999f, -999f };
        private const float TrainMinSpacing = 18f;

        private class RowInstance
        {
            public float z;
            public readonly List<GameObject> objects = new List<GameObject>(8);
            public void Clear() => objects.Clear();
        }

        private GameConfig Config => GameManager.Instance != null ? GameManager.Instance.Config : null;

        protected override void OnSingletonAwake()
        {
            foreach (var binding in obstacleBindings)
            {
                if (binding != null && binding.prefab != null)
                {
                    _prefabByCell[binding.cell] = binding.prefab;
                }
            }
            _patternRng = new System.Random(System.Environment.TickCount);
        }

        /// <summary>Called by GameManager on run start to reset the stream.</summary>
        public void BeginRun(Transform playerTransform)
        {
            _player = playerTransform;
            _nextRowZ = warmupDistance;
            _nextSegmentZ = 0f;
            for (int i = 0; i < _lastTrainZ.Length; i++)
            {
                _lastTrainZ[i] = -999f;
            }
            ClearWorld();
            PrewarmPools();

            // Seed the first few segments so the player never sees empty track.
            while (_nextSegmentZ < 120f)
            {
                SpawnSegment(_nextSegmentZ);
                _nextSegmentZ += _segmentLength;
            }
        }

        public void ClearWorld()
        {
            foreach (var row in _rows)
            {
                RecycleRow(row);
            }
            _rows.Clear();

            foreach (var segment in _segments)
            {
                if (segment != null)
                {
                    if (PoolManager.Exists)
                    {
                        PoolManager.Instance.Recycle(segment);
                    }
                    else
                    {
                        Destroy(segment);
                    }
                }
            }
            _segments.Clear();
        }

        private void PrewarmPools()
        {
            if (!PoolManager.Exists)
            {
                return;
            }
            var pool = PoolManager.Instance;
            pool.Prewarm(coinPrefab, 120);
            pool.Prewarm(pickupMagnetPrefab, 4);
            pool.Prewarm(pickupJetpackPrefab, 4);
            pool.Prewarm(pickupShieldPrefab, 4);
            pool.Prewarm(pickupMultiplierPrefab, 4);
            pool.Prewarm(trackSegmentPrefab, 10);
            foreach (var binding in obstacleBindings)
            {
                if (binding != null && binding.prefab != null)
                {
                    pool.Prewarm(binding.prefab, binding.cell == TrackCell.Train || binding.cell == TrackCell.TrainMoving ? 6 : 16);
                }
            }
        }

        private void Update()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || _player == null)
            {
                return;
            }
            if (gm.State != GameState.Playing && gm.State != GameState.GameOver)
            {
                return;
            }

            float playerZ = _player.position.z;
            float lookAhead = Config != null ? Config.chunkLength * Config.chunksAhead : 140f;

            // Stream new content.
            while (_nextSegmentZ < playerZ + lookAhead)
            {
                SpawnSegment(_nextSegmentZ);
                _nextSegmentZ += _segmentLength;
            }

            while (_nextRowZ < playerZ + lookAhead)
            {
                _nextRowZ += SpawnRow(_nextRowZ, gm.Difficulty);
            }

            // Recycle behind the player.
            RecycleBehind(playerZ);
        }

        // ------------------------------------------------------------------
        // Row spawning
        // ------------------------------------------------------------------

        /// <summary>
        /// Spawns one content unit at z. Returns how far the cursor should advance
        /// (pattern length + gap) so multi-row patterns never overlap the next one.
        /// </summary>
        private float SpawnRow(float z, float difficulty)
        {
            RowInstance row = _rowPool.Count > 0 ? _rowPool.Pop() : new RowInstance();
            row.Clear();
            row.z = z;
            _rows.Add(row);

            bool warmup = z < warmupDistance;
            GameConfig config = Config;
            float rowSpacing = config != null ? config.rowSpacing : 7.5f;

            // Free breathing room between clusters.
            if (!warmup && config != null && _patternRng.NextDouble() < config.emptyRowChance)
            {
                return rowSpacing;
            }

            // Free-lane pickup opportunity.
            if (!warmup && _patternRng.NextDouble() < pickupChance)
            {
                PickupType type = (PickupType)_patternRng.Next(0, 4);
                GameObject prefab = PickupPrefab(type);
                if (prefab != null)
                {
                    int lane = _patternRng.Next(-1, 2);
                    SpawnInto(row, prefab, new Vector3(LaneX(lane), 1.15f, z));
                }
                return rowSpacing;
            }

            if (warmup)
            {
                // Tutorial stretch: coins only.
                if (_patternRng.NextDouble() < 0.55)
                {
                    SpawnCoinRun(row, _patternRng.Next(-1, 2), z);
                }
                return rowSpacing;
            }

            PatternDefinition pattern = PickPattern(difficulty);
            if (pattern == null || pattern.rows == null || pattern.rows.Length == 0)
            {
                SpawnCoinRun(row, _patternRng.Next(-1, 2), z);
                return rowSpacing;
            }

            for (int r = 0; r < pattern.rows.Length; r++)
            {
                float rowZ = z + r * (rowSpacing * 0.9f);
                TrackRow cells = pattern.rows[r];
                if (cells == null)
                {
                    continue;
                }
                SpawnCell(row, cells.left, -1, rowZ);
                SpawnCell(row, cells.center, 0, rowZ);
                SpawnCell(row, cells.right, 1, rowZ);
            }

            // Advance past the whole pattern plus a gap.
            return pattern.rows.Length * rowSpacing * 0.9f + rowSpacing * 0.6f;
        }

        private void SpawnCell(RowInstance row, TrackCell cell, int lane, float z)
        {
            float x = LaneX(lane);

            switch (cell)
            {
                case TrackCell.Empty:
                    return;

                case TrackCell.Coin:
                    SpawnCoinRun(row, lane, z);
                    return;

                case TrackCell.PickupMagnet:
                    SpawnInto(row, PickupPrefab(PickupType.Magnet), new Vector3(x, 1.15f, z));
                    return;
                case TrackCell.PickupJetpack:
                    SpawnInto(row, PickupPrefab(PickupType.Jetpack), new Vector3(x, 1.15f, z));
                    return;
                case TrackCell.PickupShield:
                    SpawnInto(row, PickupPrefab(PickupType.Shield), new Vector3(x, 1.15f, z));
                    return;
                case TrackCell.PickupMultiplier:
                    SpawnInto(row, PickupPrefab(PickupType.Multiplier), new Vector3(x, 1.15f, z));
                    return;
            }

            if (_prefabByCell.TryGetValue(cell, out GameObject prefab) && prefab != null)
            {
                // Trains are long: never stack two within TrainMinSpacing in one lane.
                if (cell == TrackCell.Train || cell == TrackCell.TrainMoving)
                {
                    int laneIndex = lane + 1;
                    if (z - _lastTrainZ[laneIndex] < TrainMinSpacing)
                    {
                        return;
                    }
                    _lastTrainZ[laneIndex] = z;
                }

                Vector3 position = new Vector3(x, 0f, z);
                if (cell == TrackCell.Train || cell == TrackCell.TrainMoving)
                {
                    // Trains are long - keep them centered in the lane.
                    position.y = 0.1f;
                }
                GameObject instance = SpawnInto(row, prefab, position);

                // Play the approach cue when a moving train enters.
                if (cell == TrackCell.TrainMoving && instance != null && instance.TryGetComponent(out Obstacle moving))
                {
                    moving.PlayApproachCue();
                }
            }
        }

        private void SpawnCoinRun(RowInstance row, int lane, float z)
        {
            if (coinPrefab == null)
            {
                return;
            }
            float x = LaneX(lane);
            for (int i = 0; i < coinRunLength; i++)
            {
                SpawnInto(row, coinPrefab, new Vector3(x, 1.0f, z + i * 1.6f));
            }
        }

        private GameObject SpawnInto(RowInstance row, GameObject prefab, Vector3 position)
        {
            if (prefab == null)
            {
                return null;
            }
            GameObject instance = PoolManager.Exists
                ? PoolManager.Instance.Spawn(prefab, position, Quaternion.identity)
                : Instantiate(prefab, position, Quaternion.identity);
            if (instance != null)
            {
                row.objects.Add(instance);
            }
            return instance;
        }

        private GameObject PickupPrefab(PickupType type)
        {
            return type switch
            {
                PickupType.Magnet => pickupMagnetPrefab,
                PickupType.Jetpack => pickupJetpackPrefab,
                PickupType.Shield => pickupShieldPrefab,
                _ => pickupMultiplierPrefab
            };
        }

        private float LaneX(int lane)
        {
            return lane * (Config != null ? Config.laneWidth : 2.2f);
        }

        // ------------------------------------------------------------------
        // Pattern selection
        // ------------------------------------------------------------------

        private PatternDefinition PickPattern(float difficulty)
        {
            float totalWeight = 0f;
            foreach (var pattern in patterns)
            {
                if (pattern == null)
                {
                    continue;
                }
                totalWeight += pattern.GetWeight(difficulty);
            }

            if (totalWeight <= 0f)
            {
                return null;
            }

            float roll = (float)_patternRng.NextDouble() * totalWeight;
            foreach (var pattern in patterns)
            {
                if (pattern == null)
                {
                    continue;
                }
                roll -= pattern.GetWeight(difficulty);
                if (roll <= 0f)
                {
                    return pattern;
                }
            }
            return null;
        }

        // ------------------------------------------------------------------
        // Track visuals + recycling
        // ------------------------------------------------------------------

        private void SpawnSegment(float z)
        {
            if (trackSegmentPrefab == null)
            {
                return;
            }
            GameObject segment = PoolManager.Exists
                ? PoolManager.Instance.Spawn(trackSegmentPrefab, new Vector3(0f, 0f, z), Quaternion.identity)
                : Instantiate(trackSegmentPrefab, new Vector3(0f, 0f, z), Quaternion.identity);
            if (segment != null)
            {
                _segments.Add(segment);
            }
        }

        private void RecycleBehind(float playerZ)
        {
            for (int i = _rows.Count - 1; i >= 0; i--)
            {
                if (_rows[i].z < playerZ - recycleMargin)
                {
                    RecycleRow(_rows[i]);
                    _rowPool.Push(_rows[i]);
                    _rows.RemoveAt(i);
                }
            }

            for (int i = _segments.Count - 1; i >= 0; i--)
            {
                GameObject segment = _segments[i];
                if (segment == null)
                {
                    _segments.RemoveAt(i);
                    continue;
                }
                if (segment.transform.position.z < playerZ - recycleMargin - _segmentLength)
                {
                    if (PoolManager.Exists)
                    {
                        PoolManager.Instance.Recycle(segment);
                    }
                    else
                    {
                        Destroy(segment);
                    }
                    _segments.RemoveAt(i);
                }
            }
        }

        private void RecycleRow(RowInstance row)
        {
            foreach (var instance in row.objects)
            {
                if (instance == null)
                {
                    continue;
                }
                if (PoolManager.Exists)
                {
                    PoolManager.Instance.Recycle(instance);
                }
                else
                {
                    Destroy(instance);
                }
            }
            row.Clear();
        }
    }
}
