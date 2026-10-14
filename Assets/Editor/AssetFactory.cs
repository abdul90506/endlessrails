using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EndlessRails.EditorTools
{
    /// <summary>
    /// Builds every material, prefab and ScriptableObject configuration asset
    /// the game needs - entirely from code, using the generated placeholder
    /// textures/audio. Run:  Tools &gt; Endless Rails &gt; 2. Generate Assets
    /// </summary>
    public static class AssetFactory
    {
        private const string MenuRoot = "Tools/Endless Rails/";
        private const string MatDir = "Assets/Materials";
        private const string PrefabDir = "Assets/Prefabs";
        private const string SoDir = "Assets/ScriptableObjects";
        private const string TexDir = "Assets/Textures/Generated";
        private const string AudDir = "Assets/Audio/Generated";

        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        [MenuItem(MenuRoot + "2. Generate Materials, Prefabs & Configs")]
        public static void GenerateAll()
        {
            EnsureFolders();
            BuildMaterials();
            BuildPrefabs();
            BuildConfigs();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[EndlessRails] All materials, prefabs and ScriptableObject configs generated.");
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder(MatDir)) AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder(PrefabDir)) AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder(SoDir)) AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }

        // ==================================================================
        // Materials
        // ==================================================================

        private static void BuildMaterials()
        {
            CreatePbrMaterial("M_Concrete", "concrete", 0.85f);
            CreatePbrMaterial("M_Gravel", "gravel", 0.95f);
            CreatePbrMaterial("M_Ballast", "ballast", 0.92f);
            CreatePbrMaterial("M_Metal", "metal_brushed", 0.38f);
            CreatePbrMaterial("M_Rust", "rust", 0.9f);
            CreatePbrMaterial("M_Asphalt", "asphalt", 0.88f);
            CreatePbrMaterial("M_Graffiti", "graffiti", 0.75f);
            CreatePbrMaterial("M_Wood", "wood", 0.8f);
            CreatePbrMaterial("M_PlatformTile", "platform_tile", 0.82f);
            CreatePbrMaterial("M_TrainPaint", "train_paint", 0.42f);

            CreateFlatMaterial("M_CoinGold", new Color(1f, 0.78f, 0.22f), 0.25f, 1f);
            CreateFlatMaterial("M_RailSteel", new Color(0.55f, 0.56f, 0.6f), 0.32f, 1f);
            CreateFlatMaterial("M_ConeOrange", new Color(1f, 0.42f, 0.1f), 0.6f, 0f);
            CreateFlatMaterial("M_BarrierRed", new Color(0.8f, 0.12f, 0.1f), 0.55f, 0.1f);
            CreateFlatMaterial("M_BarrierWhite", new Color(0.92f, 0.92f, 0.9f), 0.55f, 0.1f);
            CreateFlatMaterial("M_ShieldBubble", new Color(0.35f, 0.75f, 1f, 0.28f), 0.1f, 0f, transparent: true);
            CreateFlatMaterial("M_Puddle", new Color(0.35f, 0.4f, 0.45f, 0.55f), 0.05f, 0f, transparent: true);
            CreateFlatMaterial("M_Skin", new Color(0.78f, 0.58f, 0.45f), 0.6f, 0f);
            CreateFlatMaterial("M_Cloth", new Color(0.18f, 0.32f, 0.55f), 0.8f, 0f);
            CreateFlatMaterial("M_Hair", new Color(0.16f, 0.12f, 0.1f), 0.75f, 0f);
            CreateFlatMaterial("M_Glass", new Color(0.7f, 0.85f, 0.95f, 0.25f), 0.06f, 0f, transparent: true);
            CreateFlatMaterial("M_VehicleBody", new Color(0.15f, 0.45f, 0.25f), 0.35f, 0.4f);
            CreateFlatMaterial("M_LampGlow", new Color(1f, 0.85f, 0.55f), 0.2f, 0f);
            CreateFlatMaterial("M_Magnet", new Color(0.85f, 0.15f, 0.15f), 0.35f, 0.6f);
            CreateFlatMaterial("M_Jetpack", new Color(0.95f, 0.6f, 0.12f), 0.35f, 0.5f);
            CreateFlatMaterial("M_Multiplier", new Color(0.2f, 0.75f, 0.35f), 0.35f, 0.2f);
        }

        private static Material CreatePbrMaterial(string name, string texBase, float smoothness)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                ?? Shader.Find("Standard");
            Material mat = new Material(shader);

            Texture2D albedo = AssetDatabase.LoadAssetAtPath<Texture2D>($"{TexDir}/{texBase}_albedo.png");
            Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{TexDir}/{texBase}_normal.png");
            Texture2D mask = AssetDatabase.LoadAssetAtPath<Texture2D>($"{TexDir}/{texBase}_mask.png");

            if (albedo != null)
            {
                SetTexture(mat, "_BaseMap", albedo);
                SetTexture(mat, "_MainTex", albedo);
            }
            if (normal != null)
            {
                SetTexture(mat, "_BumpMap", normal);
                if (mat.HasProperty("_NormalScale"))
                {
                    mat.SetFloat("_NormalScale", 1.1f);
                }
                mat.EnableKeyword("_NORMALMAP");
            }
            if (mask != null && mat.HasProperty("_MaskMap"))
            {
                SetTexture(mat, "_MaskMap", mask);
            }
            SetFloat(mat, "_Smoothness", smoothness);
            SetFloat(mat, "_Glossiness", smoothness);

            return SaveMaterial(mat, name);
        }

        private static Material CreateFlatMaterial(string name, Color color, float smoothness, float metallic, bool transparent = false)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                ?? Shader.Find("Standard");
            Material mat = new Material(shader);
            SetColor(mat, "_BaseColor", color);
            SetColor(mat, "_Color", color);
            SetFloat(mat, "_Smoothness", smoothness);
            SetFloat(mat, "_Glossiness", smoothness);
            SetFloat(mat, "_Metallic", metallic);

            if (transparent)
            {
                // URP Lit transparent setup.
                mat.SetFloat("_Surface", 1f);
                mat.SetFloat("_Blend", 0f);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                if (mat.HasProperty("_AlphaClip")) mat.SetFloat("_AlphaClip", 0f);
            }

            return SaveMaterial(mat, name);
        }

        private static Material SaveMaterial(Material mat, string name)
        {
            string path = $"{MatDir}/{name}.mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(mat, existing);
                Materials[name] = existing;
                Object.DestroyImmediate(mat);
                return existing;
            }
            AssetDatabase.CreateAsset(mat, path);
            Materials[name] = mat;
            return mat;
        }

        private static void SetTexture(Material mat, string prop, Texture tex)
        {
            if (mat.HasProperty(prop))
            {
                mat.SetTexture(prop, tex);
            }
        }

        private static void SetColor(Material mat, string prop, Color color)
        {
            if (mat.HasProperty(prop))
            {
                mat.SetColor(prop, color);
            }
        }

        private static void SetFloat(Material mat, string prop, float value)
        {
            if (mat.HasProperty(prop))
            {
                mat.SetFloat(prop, value);
            }
        }

        private static Material Mat(string name) => Materials.ContainsKey(name) ? Materials[name] : null;

        // ==================================================================
        // Prefabs
        // ==================================================================

        private static void BuildPrefabs()
        {
            BuildCoin();
            BuildPickups();
            BuildObstacles();
            BuildTrackSegment();
            BuildPlayer();
            BuildChaser();
            BuildRainFX();
        }

        private static GameObject SavePrefab(GameObject go, string name)
        {
            string path = $"{PrefabDir}/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        private static GameObject Primitive(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            if (mat != null)
            {
                go.GetComponent<Renderer>().sharedMaterial = mat;
            }
            return go;
        }

        private static void BuildCoin()
        {
            GameObject coin = new GameObject("Coin");
            GameObject disc = Primitive(PrimitiveType.Cylinder, "Disc", coin.transform, Vector3.zero,
                new Vector3(0.52f, 0.05f, 0.52f), Mat("M_CoinGold"));
            disc.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Object.DestroyImmediate(disc.GetComponent<Collider>());

            SphereCollider trigger = coin.AddComponent<SphereCollider>();
            trigger.radius = 0.45f;
            trigger.isTrigger = true;
            coin.tag = "Coin";
            coin.AddComponent<Coin>();
            SavePrefab(coin, "Coin");
        }

        private static void BuildPickups()
        {
            BuildPickup("Pickup_Magnet", PickupType.Magnet, "M_Magnet", PrimitiveType.Cylinder);
            BuildPickup("Pickup_Jetpack", PickupType.Jetpack, "M_Jetpack", PrimitiveType.Cube);
            BuildPickup("Pickup_Shield", PickupType.Shield, "M_ShieldBubble", PrimitiveType.Sphere);
            BuildPickup("Pickup_Multiplier", PickupType.Multiplier, "M_Multiplier", PrimitiveType.Cube);
        }

        private static void BuildPickup(string name, PickupType type, string materialName, PrimitiveType shape)
        {
            GameObject pickup = new GameObject(name);
            GameObject body = Primitive(shape, "Body", pickup.transform, Vector3.zero, Vector3.one * 0.7f, Mat(materialName));
            Object.DestroyImmediate(body.GetComponent<Collider>());

            // Simple spinning ring for readability.
            GameObject ring = Primitive(PrimitiveType.Cylinder, "Ring", pickup.transform, Vector3.zero,
                new Vector3(1f, 0.04f, 1f), Mat("M_BarrierWhite"));
            ring.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Object.DestroyImmediate(ring.GetComponent<Collider>());

            SphereCollider trigger = pickup.AddComponent<SphereCollider>();
            trigger.radius = 0.55f;
            trigger.isTrigger = true;
            pickup.tag = "Pickup";

            Pickup pickupComponent = pickup.AddComponent<Pickup>();
            var so = new SerializedObject(pickupComponent);
            so.FindProperty("type").enumValueIndex = (int)type;
            so.ApplyModifiedPropertiesWithoutUndo();

            SavePrefab(pickup, name);
        }

        private static void BuildObstacles()
        {
            // --- Low barrier (jump over) ---
            GameObject barrier = new GameObject("Obstacle_BarrierJump");
            Primitive(PrimitiveType.Cube, "Plank", barrier.transform, new Vector3(0f, 0.55f, 0f),
                new Vector3(2f, 0.22f, 0.12f), Mat("M_BarrierRed"));
            Primitive(PrimitiveType.Cube, "PlankWhite", barrier.transform, new Vector3(0f, 0.85f, 0f),
                new Vector3(2f, 0.18f, 0.1f), Mat("M_BarrierWhite"));
            Primitive(PrimitiveType.Cube, "LegL", barrier.transform, new Vector3(-0.85f, 0.35f, 0f),
                new Vector3(0.12f, 0.7f, 0.12f), Mat("M_Metal"));
            Primitive(PrimitiveType.Cube, "LegR", barrier.transform, new Vector3(0.85f, 0.35f, 0f),
                new Vector3(0.12f, 0.7f, 0.12f), Mat("M_Metal"));
            AddObstacleShape(barrier, new Vector3(0f, 0.6f, 0f), new Vector3(2f, 1.2f, 0.35f), ObstacleImpact.Lethal, canJumpOver: true);
            SavePrefab(barrier, "Obstacle_BarrierJump");

            // --- Overhead beam (slide under) ---
            GameObject beam = new GameObject("Obstacle_BarrierSlide");
            Primitive(PrimitiveType.Cube, "Beam", beam.transform, new Vector3(0f, 1.75f, 0f),
                new Vector3(2.1f, 0.32f, 0.32f), Mat("M_TrainPaint"));
            Primitive(PrimitiveType.Cube, "PostL", beam.transform, new Vector3(-1.0f, 1.0f, 0f),
                new Vector3(0.18f, 2f, 0.18f), Mat("M_Metal"));
            Primitive(PrimitiveType.Cube, "PostR", beam.transform, new Vector3(1.0f, 1.0f, 0f),
                new Vector3(0.18f, 2f, 0.18f), Mat("M_Metal"));
            AddObstacleShape(beam, new Vector3(0f, 1.6f, 0f), new Vector3(2.1f, 0.9f, 0.4f), ObstacleImpact.Lethal, mustSlideUnder: true);
            SavePrefab(beam, "Obstacle_BarrierSlide");

            // --- Traffic cone (stumble) ---
            GameObject cone = new GameObject("Obstacle_Cone");
            GameObject body = Primitive(PrimitiveType.Cylinder, "ConeBody", cone.transform, new Vector3(0f, 0.32f, 0f),
                new Vector3(0.42f, 0.34f, 0.42f), Mat("M_ConeOrange"));
            body.transform.localRotation = Quaternion.identity;
            Primitive(PrimitiveType.Cube, "ConeBase", cone.transform, new Vector3(0f, 0.04f, 0f),
                new Vector3(0.62f, 0.08f, 0.62f), Mat("M_ConeOrange"));
            AddObstacleShape(cone, new Vector3(0f, 0.32f, 0f), new Vector3(0.62f, 0.66f, 0.62f), ObstacleImpact.Stumble, canJumpOver: true);
            SavePrefab(cone, "Obstacle_Cone");

            // --- Construction block (full block) ---
            GameObject block = new GameObject("Obstacle_Block");
            Primitive(PrimitiveType.Cube, "BlockBody", block.transform, new Vector3(0f, 0.62f, 0f),
                new Vector3(1.9f, 1.25f, 1.3f), Mat("M_Concrete"));
            Primitive(PrimitiveType.Cube, "Stripe", block.transform, new Vector3(0f, 1.0f, 0.66f),
                new Vector3(1.92f, 0.22f, 0.02f), Mat("M_BarrierWhite"));
            AddObstacleShape(block, new Vector3(0f, 0.62f, 0f), new Vector3(1.9f, 1.25f, 1.3f), ObstacleImpact.Lethal, canJumpOver: false);
            SavePrefab(block, "Obstacle_Block");

            // --- Low wall (jump) ---
            GameObject wall = new GameObject("Obstacle_LowWall");
            Primitive(PrimitiveType.Cube, "WallBody", wall.transform, new Vector3(0f, 0.52f, 0f),
                new Vector3(2f, 1.05f, 0.35f), Mat("M_Graffiti"));
            AddObstacleShape(wall, new Vector3(0f, 0.52f, 0f), new Vector3(2f, 1.05f, 0.35f), ObstacleImpact.Lethal, canJumpOver: true);
            SavePrefab(wall, "Obstacle_LowWall");

            // --- Train (stationary, long) ---
            BuildTrain("Obstacle_Train", moving: false);
            BuildTrain("Obstacle_TrainMoving", moving: true);

            // --- Vehicle (rail crossing) ---
            GameObject vehicle = new GameObject("Obstacle_Vehicle");
            Primitive(PrimitiveType.Cube, "Body", vehicle.transform, new Vector3(0f, 0.72f, 0f),
                new Vector3(1.9f, 0.85f, 4.1f), Mat("M_VehicleBody"));
            Primitive(PrimitiveType.Cube, "Cabin", vehicle.transform, new Vector3(0f, 1.35f, -0.3f),
                new Vector3(1.75f, 0.7f, 2f), Mat("M_Glass"));
            Primitive(PrimitiveType.Cylinder, "WheelFL", vehicle.transform, new Vector3(-0.95f, 0.35f, 1.35f),
                new Vector3(0.7f, 0.18f, 0.7f), Mat("M_Rust")).transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            Primitive(PrimitiveType.Cylinder, "WheelFR", vehicle.transform, new Vector3(0.95f, 0.35f, 1.35f),
                new Vector3(0.7f, 0.18f, 0.7f), Mat("M_Rust")).transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            Primitive(PrimitiveType.Cylinder, "WheelBL", vehicle.transform, new Vector3(-0.95f, 0.35f, -1.35f),
                new Vector3(0.7f, 0.18f, 0.7f), Mat("M_Rust")).transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            Primitive(PrimitiveType.Cylinder, "WheelBR", vehicle.transform, new Vector3(0.95f, 0.35f, -1.35f),
                new Vector3(0.7f, 0.18f, 0.7f), Mat("M_Rust")).transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            GameObject vehicleShape = AddObstacleShape(vehicle, new Vector3(0f, 0.9f, 0f), new Vector3(1.95f, 1.8f, 4.2f), ObstacleImpact.Lethal, canJumpOver: false);
            var vehicleObstacle = vehicleShape.GetComponent<Obstacle>();
            var vehicleSo = new SerializedObject(vehicleObstacle);
            vehicleSo.FindProperty("moves").boolValue = true;
            vehicleSo.FindProperty("moveSpeed").floatValue = 9f;
            vehicleSo.FindProperty("moveRange").floatValue = 55f;
            vehicleSo.FindProperty("moveDirection").vector3Value = Vector3.right;
            vehicleSo.ApplyModifiedPropertiesWithoutUndo();
            vehicle.tag = "Vehicle";
            SavePrefab(vehicle, "Obstacle_Vehicle");
        }

        private static void BuildTrain(string name, bool moving)
        {
            GameObject train = new GameObject(name);
            float length = 17f;

            Primitive(PrimitiveType.Cube, "Carriage", train.transform, new Vector3(0f, 1.85f, 0f),
                new Vector3(2.15f, 2.7f, length), Mat("M_TrainPaint"));
            Primitive(PrimitiveType.Cube, "Roof", train.transform, new Vector3(0f, 3.32f, 0f),
                new Vector3(2.05f, 0.28f, length * 0.98f), Mat("M_Metal"));
            Primitive(PrimitiveType.Cube, "WindowStripL", train.transform, new Vector3(-1.09f, 2.35f, 0f),
                new Vector3(0.04f, 0.72f, length * 0.86f), Mat("M_Glass"));
            Primitive(PrimitiveType.Cube, "WindowStripR", train.transform, new Vector3(1.09f, 2.35f, 0f),
                new Vector3(0.04f, 0.72f, length * 0.86f), Mat("M_Glass"));
            Primitive(PrimitiveType.Cube, "Undercarriage", train.transform, new Vector3(0f, 0.35f, 0f),
                new Vector3(1.9f, 0.5f, length * 0.94f), Mat("M_Rust"));
            Primitive(PrimitiveType.Cube, "Nose", train.transform, new Vector3(0f, 1.85f, length * 0.5f + 0.2f),
                new Vector3(2.05f, 2.4f, 0.5f), Mat("M_TrainPaint"));

            GameObject shape = AddObstacleShape(train, new Vector3(0f, 1.8f, 0f), new Vector3(2.3f, 3.6f, length),
                ObstacleImpact.Lethal, canJumpOver: false);

            Obstacle obstacle = shape.GetComponent<Obstacle>();
            var so = new SerializedObject(obstacle);
            so.FindProperty("moves").boolValue = moving;
            if (moving)
            {
                so.FindProperty("moveSpeed").floatValue = 7f;
                so.FindProperty("moveRange").floatValue = 90f;
                so.FindProperty("moveDirection").vector3Value = Vector3.forward;
            }
            so.ApplyModifiedPropertiesWithoutUndo();

            AudioSource loop = train.AddComponent<AudioSource>();
            loop.playOnAwake = false;
            loop.loop = true;
            loop.spatialBlend = 1f;
            loop.rolloffMode = AudioRolloffMode.Linear;
            loop.maxDistance = 80f;
            var so2 = new SerializedObject(obstacle);
            so2.FindProperty("loopSource").objectReferenceValue = loop;
            so2.ApplyModifiedPropertiesWithoutUndo();

            train.tag = moving ? "Train" : "Obstacle";
            SavePrefab(train, name);
        }

        /// <summary>Adds the root Obstacle component + trigger box to a prefab.</summary>
        private static GameObject AddObstacleShape(GameObject root, Vector3 center, Vector3 size, ObstacleImpact impact,
            bool canJumpOver = true, bool mustSlideUnder = false)
        {
            BoxCollider trigger = root.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center = center;
            trigger.size = size;
            root.tag = root.CompareTag("Untagged") ? "Obstacle" : root.tag;

            Obstacle obstacle = root.AddComponent<Obstacle>();
            var so = new SerializedObject(obstacle);
            so.FindProperty("impact").enumValueIndex = (int)impact;
            so.FindProperty("canJumpOver").boolValue = canJumpOver;
            so.FindProperty("mustSlideUnder").boolValue = mustSlideUnder;
            so.FindProperty("clearanceHeight").floatValue = size.y * 0.5f;
            so.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static void BuildTrackSegment()
        {
            GameObject segment = new GameObject("TrackSegment");
            float length = 24f;

            // Ballast bed.
            Primitive(PrimitiveType.Cube, "Ballast", segment.transform, new Vector3(0f, -0.12f, 0f),
                new Vector3(9f, 0.24f, length), Mat("M_Ballast"));

            // Rails (x = ±1.1 keeps them inside the center lane span).
            Primitive(PrimitiveType.Cube, "RailL", segment.transform, new Vector3(-1.1f, 0.06f, 0f),
                new Vector3(0.12f, 0.12f, length), Mat("M_RailSteel"));
            Primitive(PrimitiveType.Cube, "RailR", segment.transform, new Vector3(1.1f, 0.06f, 0f),
                new Vector3(0.12f, 0.12f, length), Mat("M_RailSteel"));

            // Sleepers.
            for (int i = -4; i <= 4; i++)
            {
                Primitive(PrimitiveType.Cube, $"Sleeper_{i + 4}", segment.transform, new Vector3(0f, 0.02f, i * 2.6f),
                    new Vector3(2.6f, 0.08f, 0.32f), Mat("M_Wood"));
            }

            // Side platforms (ride height for visual depth).
            Primitive(PrimitiveType.Cube, "PlatformL", segment.transform, new Vector3(-5.6f, 0.55f, 0f),
                new Vector3(3.4f, 1.1f, length), Mat("M_PlatformTile"));
            Primitive(PrimitiveType.Cube, "PlatformR", segment.transform, new Vector3(5.6f, 0.55f, 0f),
                new Vector3(3.4f, 1.1f, length), Mat("M_PlatformTile"));

            // Fences + graffiti walls behind the platforms.
            Primitive(PrimitiveType.Cube, "WallL", segment.transform, new Vector3(-8.1f, 1.6f, 0f),
                new Vector3(0.25f, 3.2f, length), Mat("M_Graffiti"));
            Primitive(PrimitiveType.Cube, "WallR", segment.transform, new Vector3(8.1f, 1.6f, 0f),
                new Vector3(0.25f, 3.2f, length), Mat("M_Graffiti"));

            for (int i = -2; i <= 2; i++)
            {
                Primitive(PrimitiveType.Cube, $"FencePostL_{i + 2}", segment.transform, new Vector3(-4.0f, 1.35f, i * 5f),
                    new Vector3(0.1f, 1.5f, 0.1f), Mat("M_Metal"));
                Primitive(PrimitiveType.Cube, $"FencePostR_{i + 2}", segment.transform, new Vector3(4.0f, 1.35f, i * 5f),
                    new Vector3(0.1f, 1.5f, 0.1f), Mat("M_Metal"));
            }

            // Lamp posts with glow heads.
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject lampPost = Primitive(PrimitiveType.Cylinder, $"LampPost_{i}", segment.transform,
                    new Vector3(i * 6.9f, 2.6f, i * 6f), new Vector3(0.14f, 2.6f, 0.14f), Mat("M_Metal"));
                lampPost.transform.localRotation = Quaternion.identity;
                Primitive(PrimitiveType.Cube, $"LampHead_{i}", segment.transform,
                    new Vector3(i * 6.9f, 5.2f, i * 6f), new Vector3(0.55f, 0.18f, 0.9f), Mat("M_LampGlow"));
            }

            // Puddles (flat transparent quads).
            for (int i = -1; i <= 1; i++)
            {
                GameObject puddle = Primitive(PrimitiveType.Cube, $"Puddle_{i + 1}", segment.transform,
                    new Vector3(i * 2.2f, 0.02f, i * 7f), new Vector3(1.6f, 0.015f, 2.6f), Mat("M_Puddle"));
                puddle.isStatic = true;
            }

            // Track-side signage.
            GameObject sign = Primitive(PrimitiveType.Cube, "Sign", segment.transform,
                new Vector3(4.2f, 2.4f, 3f), new Vector3(0.08f, 1f, 1.5f), Mat("M_BarrierWhite"));
            sign.transform.localRotation = Quaternion.Euler(0f, 12f, 0f);

            segment.layer = LayerMask.NameToLayer("Default");
            SavePrefab(segment, "TrackSegment");
        }

        private static void BuildPlayer()
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";

            CapsuleCollider capsule = player.AddComponent<CapsuleCollider>();
            capsule.center = new Vector3(0f, 0.95f, 0f);
            capsule.radius = 0.32f;
            capsule.height = 1.9f;
            capsule.isTrigger = true;

            Rigidbody body = player.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            player.AddComponent<PlayerMotor>();
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerCollisionHandler>();
            player.AddComponent<PlayerPowerups>();
            player.AddComponent<FootstepEmitter>();

            GameObject rigHost = new GameObject("CharacterRig");
            rigHost.transform.SetParent(player.transform, false);
            rigHost.AddComponent<ProceduralRunner>();
            rigHost.AddComponent<PlayerAnimatorDriver>();

            // Shield visual bubble (disabled by default).
            GameObject shield = Primitive(PrimitiveType.Sphere, "ShieldVisual", player.transform,
                new Vector3(0f, 1f, 0f), Vector3.one * 1.6f, Mat("M_ShieldBubble"));
            Object.DestroyImmediate(shield.GetComponent<Collider>());
            shield.SetActive(false);

            var powerups = player.GetComponent<PlayerPowerups>();
            var so = new SerializedObject(powerups);
            so.FindProperty("shieldVisual").objectReferenceValue = shield;
            so.ApplyModifiedPropertiesWithoutUndo();

            SavePrefab(player, "Player");
        }

        private static void BuildChaser()
        {
            GameObject chaser = new GameObject("Chaser");
            GameObject rigHost = new GameObject("CharacterRig");
            rigHost.transform.SetParent(chaser.transform, false);
            var rig = rigHost.AddComponent<ProceduralRunner>();

            // Distinct uniform colours for the officer.
            var appearance = ScriptableObject.CreateInstance<CharacterDefinition>();
            appearance.primaryColor = new Color(0.12f, 0.16f, 0.28f);
            appearance.secondaryColor = new Color(0.75f, 0.6f, 0.15f);
            appearance.skinTone = new Color(0.72f, 0.52f, 0.4f);
            string appearancePath = $"{SoDir}/Character_Chaser.asset";
            AssetDatabase.CreateAsset(appearance, appearancePath);

            var soRig = new SerializedObject(rig);
            soRig.FindProperty("appearance").objectReferenceValue = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(appearancePath);
            soRig.ApplyModifiedPropertiesWithoutUndo();

            chaser.AddComponent<ChaserAI>();
            SavePrefab(chaser, "Chaser");
        }

        private static void BuildRainFX()
        {
            GameObject rain = new GameObject("RainFX");
            ParticleSystem ps = rain.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.loop = true;
            main.startLifetime = 1.1f;
            main.startSpeed = 14f;
            main.startSize = 0.045f;
            main.maxParticles = 900;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = 0.35f;

            var emission = ps.emission;
            emission.rateOverTime = 420f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(26f, 0.2f, 26f);

            var renderer = rain.GetComponent<ParticleSystemRenderer>();
            Shader particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                ?? Shader.Find("Particles/Standard Unlit")
                ?? Shader.Find("Sprites/Default");
            Material rainMat = new Material(particleShader);
            if (rainMat.HasProperty("_BaseColor"))
            {
                rainMat.SetColor("_BaseColor", new Color(0.65f, 0.72f, 0.85f, 0.42f));
            }
            renderer.sharedMaterial = rainMat;
            renderer.renderMode = ParticleSystemRenderMode.Stretch;

            SavePrefab(rain, "RainFX");
        }

        // ==================================================================
        // ScriptableObject configs
        // ==================================================================

        private static void BuildConfigs()
        {
            BuildGameConfig();
            BuildAudioLibrary();
            BuildPatterns();
            BuildCharacters();
            BuildBiomes();
            BuildMissions();
        }

        private static T GetOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }
            return asset;
        }

        private static void BuildGameConfig()
        {
            var config = GetOrCreate<GameConfig>($"{SoDir}/GameConfig.asset");
            EditorUtility.SetDirty(config);
        }

        private static void BuildAudioLibrary()
        {
            var library = GetOrCreate<AudioLibrary>($"{SoDir}/AudioLibrary.asset");

            library.footstepConcrete = LoadClips("footstep_concrete");
            library.footstepGravel = LoadClips("footstep_gravel");
            library.footstepMetal = LoadClips("footstep_metal");
            library.footstepWet = LoadClips("footstep_wet");
            library.jumpWhoosh = LoadClip("jump_whoosh");
            library.landThud = LoadClip("land_thud");
            library.slideScrape = LoadClip("slide_scrape");
            library.stumble = LoadClip("stumble");
            library.crash = LoadClip("crash");
            library.laneWhoosh = LoadClip("ui_swoosh");
            library.coinPickup = LoadClip("coin_pickup");
            library.coinMulti = LoadClip("coin_multi");
            library.powerup = LoadClip("powerup");
            library.powerdown = LoadClip("powerdown");
            library.shieldBreak = LoadClip("shield_break");
            library.trainHorn = LoadClip("train_horn");
            library.trainRumble = LoadClip("train_rumble");
            library.chaserSteps = LoadClip("chaser_steps");
            library.chaserShout = LoadClip("shout");
            library.windAmbient = LoadClip("wind_ambient");
            library.rainAmbient = LoadClip("rain_ambient");
            library.cityAmbient = LoadClip("city_ambient");
            library.musicLoop = LoadClip("music_loop");
            library.buttonClick = LoadClip("button_click");
            library.uiSwoosh = LoadClip("ui_swoosh");
            library.gameOverSting = LoadClip("game_over");

            EditorUtility.SetDirty(library);
        }

        private static AudioClip LoadClip(string name) =>
            AssetDatabase.LoadAssetAtPath<AudioClip>($"{AudDir}/{name}.wav");

        private static AudioClip[] LoadClips(string name)
        {
            AudioClip clip = LoadClip(name);
            return clip != null ? new[] { clip } : new AudioClip[0];
        }

        private static void BuildPatterns()
        {
            // A small library of reusable layouts. All leave ≥1 lane passable.
            CreatePattern("Pattern_EasyCoins", 1.2f, 0f, 0f,
                Row(TrackCell.Coin, TrackCell.Empty, TrackCell.Empty),
                Row(TrackCell.Empty, TrackCell.Coin, TrackCell.Empty),
                Row(TrackCell.Empty, TrackCell.Empty, TrackCell.Coin));

            CreatePattern("Pattern_SingleBarrier", 1f, 0.2f, 0.1f,
                Row(TrackCell.BarrierJump, TrackCell.Empty, TrackCell.Coin),
                Row(TrackCell.Coin, TrackCell.Empty, TrackCell.Empty));

            CreatePattern("Pattern_SlideGate", 0.9f, 0.3f, 0.15f,
                Row(TrackCell.BarrierSlide, TrackCell.Empty, TrackCell.Coin),
                Row(TrackCell.Empty, TrackCell.Coin, TrackCell.Empty));

            CreatePattern("Pattern_TwoLaneBlock", 0.8f, 0.5f, 0.25f,
                Row(TrackCell.Block, TrackCell.Empty, TrackCell.Block),
                Row(TrackCell.Coin, TrackCell.Coin, TrackCell.Coin));

            CreatePattern("Pattern_ConeField", 1f, 0.25f, 0.1f,
                Row(TrackCell.Cone, TrackCell.Cone, TrackCell.Empty),
                Row(TrackCell.Empty, TrackCell.Coin, TrackCell.Coin));

            CreatePattern("Pattern_WallJump", 0.9f, 0.35f, 0.2f,
                Row(TrackCell.LowWall, TrackCell.LowWall, TrackCell.Empty),
                Row(TrackCell.Coin, TrackCell.Empty, TrackCell.Empty));

            CreatePattern("Pattern_TrainSide", 0.55f, 0.8f, 0.3f,
                Row(TrackCell.Train, TrackCell.Empty, TrackCell.Coin),
                Row(TrackCell.Train, TrackCell.Coin, TrackCell.Empty),
                Row(TrackCell.Train, TrackCell.Empty, TrackCell.Coin),
                Row(TrackCell.Coin, TrackCell.Coin, TrackCell.Coin));

            CreatePattern("Pattern_MovingTrain", 0.35f, 1.1f, 0.45f, true,
                Row(TrackCell.TrainMoving, TrackCell.Empty, TrackCell.Empty),
                Row(TrackCell.Coin, TrackCell.Coin, TrackCell.Empty));

            CreatePattern("Pattern_VehicleCrossing", 0.3f, 1f, 0.4f,
                Row(TrackCell.Empty, TrackCell.Vehicle, TrackCell.Empty),
                Row(TrackCell.Coin, TrackCell.Empty, TrackCell.Coin));

            CreatePattern("Pattern_MixedAdvanced", 0.3f, 1.2f, 0.5f,
                Row(TrackCell.BarrierJump, TrackCell.BarrierSlide, TrackCell.Block),
                Row(TrackCell.Coin, TrackCell.Empty, TrackCell.Coin),
                Row(TrackCell.Cone, TrackCell.Cone, TrackCell.Empty));

            CreatePattern("Pattern_PickupLane", 0.25f, 0.1f, 0f,
                Row(TrackCell.PickupMagnet, TrackCell.Empty, TrackCell.Coin),
                Row(TrackCell.Coin, TrackCell.Empty, TrackCell.Empty));

            CreatePattern("Pattern_DenseHard", 0.2f, 1.4f, 0.6f,
                Row(TrackCell.BarrierSlide, TrackCell.Block, TrackCell.BarrierJump),
                Row(TrackCell.Empty, TrackCell.Coin, TrackCell.Empty),
                Row(TrackCell.LowWall, TrackCell.Train, TrackCell.Cone));
        }

        private static TrackRow Row(TrackCell left, TrackCell center, TrackCell right)
        {
            return new TrackRow { left = left, center = center, right = right };
        }

        private static void CreatePattern(string name, float baseWeight, float difficultyBonus, float minDifficulty,
            params TrackRow[] rows)
        {
            CreatePattern(name, baseWeight, difficultyBonus, minDifficulty, false, rows);
        }

        private static void CreatePattern(string name, float baseWeight, float difficultyBonus, float minDifficulty,
            bool containsTrains, params TrackRow[] rows)
        {
            var pattern = GetOrCreate<PatternDefinition>($"{SoDir}/{name}.asset");
            pattern.rows = rows;
            pattern.baseWeight = baseWeight;
            pattern.difficultyWeightBonus = difficultyBonus;
            pattern.minDifficulty = minDifficulty;
            pattern.containsTrains = containsTrains;
            EditorUtility.SetDirty(pattern);
        }

        private static void BuildCharacters()
        {
            var male = GetOrCreate<CharacterDefinition>($"{SoDir}/Character_Male.asset");
            male.id = "runner_male_01";
            male.displayName = "Runner (M)";
            male.gender = RunnerGender.Male;
            male.primaryColor = new Color(0.18f, 0.32f, 0.55f);
            male.secondaryColor = new Color(0.85f, 0.62f, 0.18f);
            male.skinTone = new Color(0.78f, 0.58f, 0.45f);
            male.ponytail = false;
            male.coinPrice = 0;
            EditorUtility.SetDirty(male);

            var female = GetOrCreate<CharacterDefinition>($"{SoDir}/Character_Female.asset");
            female.id = "runner_female_01";
            female.displayName = "Runner (F)";
            female.gender = RunnerGender.Female;
            female.primaryColor = new Color(0.62f, 0.18f, 0.3f);
            female.secondaryColor = new Color(0.9f, 0.75f, 0.2f);
            female.skinTone = new Color(0.82f, 0.62f, 0.5f);
            female.ponytail = true;
            female.coinPrice = 250;
            EditorUtility.SetDirty(female);

            var maleAlt = GetOrCreate<CharacterDefinition>($"{SoDir}/Character_MaleAlt.asset");
            maleAlt.id = "runner_male_02";
            maleAlt.displayName = "Runner (M) 'Night'";
            maleAlt.gender = RunnerGender.Male;
            maleAlt.primaryColor = new Color(0.1f, 0.1f, 0.12f);
            maleAlt.secondaryColor = new Color(0.2f, 0.8f, 0.7f);
            maleAlt.skinTone = new Color(0.7f, 0.5f, 0.38f);
            maleAlt.ponytail = false;
            maleAlt.coinPrice = 750;
            EditorUtility.SetDirty(maleAlt);
        }

        private static void BuildBiomes()
        {
            var day = GetOrCreate<BiomeDefinition>($"{SoDir}/Biome_Day.asset");
            day.id = "city_day";
            day.displayName = "City Line - Day";
            day.sunColor = new Color(1f, 0.96f, 0.88f);
            day.sunIntensity = 1.3f;
            day.skyTint = new Color(0.55f, 0.7f, 1f);
            day.fogColor = new Color(0.72f, 0.78f, 0.85f);
            EditorUtility.SetDirty(day);

            var dusk = GetOrCreate<BiomeDefinition>($"{SoDir}/Biome_Dusk.asset");
            dusk.id = "city_dusk";
            dusk.displayName = "City Line - Sunset";
            dusk.sunColor = new Color(1f, 0.62f, 0.38f);
            dusk.sunIntensity = 1.05f;
            dusk.ambientSkyColor = new Color(0.55f, 0.38f, 0.35f);
            dusk.skyTint = new Color(1f, 0.62f, 0.42f);
            dusk.fogColor = new Color(0.72f, 0.52f, 0.42f);
            EditorUtility.SetDirty(dusk);

            var night = GetOrCreate<BiomeDefinition>($"{SoDir}/Biome_Night.asset");
            night.id = "city_night";
            night.displayName = "City Line - Night";
            night.sunColor = new Color(0.55f, 0.62f, 0.92f);
            night.sunIntensity = 0.5f;
            night.ambientSkyColor = new Color(0.12f, 0.15f, 0.28f);
            night.ambientEquatorColor = new Color(0.08f, 0.1f, 0.16f);
            night.skyTint = new Color(0.12f, 0.16f, 0.32f);
            night.skyExposure = 0.55f;
            night.fogColor = new Color(0.12f, 0.14f, 0.22f);
            night.fogDensity = 0.016f;
            night.wetSurfaceChance = 0.3f;
            EditorUtility.SetDirty(night);
        }

        private static void BuildMissions()
        {
            CreateMission("Mission_Coins500", "Coin Collector", "Collect coins in total.",
                MissionType.CollectCoins, 500, 150, dailyOnly: false);
            CreateMission("Mission_Distance2000", "Marathon", "Run a total distance of",
                MissionType.RunDistance, 2000, 200, dailyOnly: false);
            CreateMission("Mission_Jumps30", "Air Time", "Perform jumps in total.",
                MissionType.JumpCount, 30, 100, dailyOnly: true);
            CreateMission("Mission_Slides20", "Under the Radar", "Perform slides in total.",
                MissionType.SlideCount, 20, 100, dailyOnly: true);
            CreateMission("Mission_Pickups10", "Powered Up", "Collect power-ups in total.",
                MissionType.CollectPickups, 10, 120, dailyOnly: true);
            CreateMission("Mission_Runs5", "Regular", "Finish runs in total.",
                MissionType.PlayRuns, 5, 80, dailyOnly: true);
            CreateMission("Mission_Coins2000", "Treasure Hunter", "Collect coins in total.",
                MissionType.CollectCoins, 2000, 500, dailyOnly: false);
            CreateMission("Mission_Distance10000", "Long Hauler", "Run a total distance of",
                MissionType.RunDistance, 10000, 900, dailyOnly: false);
        }

        private static void CreateMission(string name, string displayName, string description,
            MissionType type, int target, int reward, bool dailyOnly)
        {
            var mission = GetOrCreate<MissionDefinition>($"{SoDir}/{name}.asset");
            mission.id = name.ToLowerInvariant();
            mission.displayName = displayName;
            mission.description = description;
            mission.type = type;
            mission.target = target;
            mission.coinReward = reward;
            mission.dailyOnly = dailyOnly;
            EditorUtility.SetDirty(mission);
        }
    }
}
