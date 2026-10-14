using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EndlessRails.EditorTools
{
    /// <summary>
    /// Builds the complete Main scene from generated prefabs: systems, lighting,
    /// player, chaser, environment and the full UGUI interface. Run after
    /// Generate Assets:  Tools &gt; Endless Rails &gt; 3. Build Main Scene
    /// </summary>
    public static class SceneBuilder
    {
        private const string MenuRoot = "Tools/Endless Rails/";
        private const string ScenePath = "Assets/Scenes/Main.unity";
        private const string PrefabDir = "Assets/Prefabs";
        private const string SoDir = "Assets/ScriptableObjects";

        [MenuItem(MenuRoot + "3. Build Main Scene")]
        public static void BuildScene()
        {
            // In batch/cloud mode there is no user dialog - always proceed.
            if (!Application.isBatchMode && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject systems = BuildSystems();
            BuildLighting();
            GameObject camera = BuildCamera();
            GameObject player = BuildPlayer();
            BuildChaser(player.transform);
            BuildEnvironment();
            BuildCanvas();

            // Final wiring that needs cross-references.
            WireReferences(systems, camera, player);

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
            EditorSceneManager.SaveScene(scene, ScenePath);

            // Register in build settings.
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

            AssetDatabase.SaveAssets();
            Debug.Log("[EndlessRails] Main scene built at " + ScenePath + ". Press Play to run!");
        }

        // ==================================================================
        // Systems
        // ==================================================================

        private static GameObject BuildSystems()
        {
            GameObject systems = new GameObject("Systems");

            GameManager gameManager = systems.AddComponent<GameManager>();
            var gmSo = new SerializedObject(gameManager);
            gmSo.FindProperty("config").objectReferenceValue = LoadSo<GameConfig>("GameConfig");
            gmSo.FindProperty("audioLibrary").objectReferenceValue = LoadSo<AudioLibrary>("AudioLibrary");
            gmSo.ApplyModifiedPropertiesWithoutUndo();

            systems.AddComponent<SwipeInputManager>();
            systems.AddComponent<PoolManager>();

            AudioManager audio = systems.AddComponent<AudioManager>();
            var audioSo = new SerializedObject(audio);
            audioSo.FindProperty("library").objectReferenceValue = LoadSo<AudioLibrary>("AudioLibrary");
            audioSo.ApplyModifiedPropertiesWithoutUndo();

            systems.AddComponent<SaveSystem>();
            systems.AddComponent<WorldManager>();
            systems.AddComponent<MissionManager>();
            systems.AddComponent<UIManager>();
            systems.AddComponent<AudioEventRelay>();

            // Sky + weather.
            GameObject sky = new GameObject("SkyTime");
            sky.transform.SetParent(systems.transform, false);
            sky.AddComponent<SkyTimeController>();
            sky.AddComponent<WeatherController>();

            // Mission pool.
            var missionManager = systems.GetComponent<MissionManager>();
            var missionSo = new SerializedObject(missionManager);
            SerializedProperty missionPool = missionSo.FindProperty("missionPool");
            missionPool.arraySize = 0;
            foreach (string missionName in new[]
                     {
                         "Mission_Coins500", "Mission_Distance2000", "Mission_Jumps30", "Mission_Slides20",
                         "Mission_Pickups10", "Mission_Runs5", "Mission_Coins2000", "Mission_Distance10000"
                     })
            {
                var mission = LoadSo<MissionDefinition>(missionName);
                if (mission == null)
                {
                    continue;
                }
                missionPool.arraySize++;
                missionPool.GetArrayElementAtIndex(missionPool.arraySize - 1).objectReferenceValue = mission;
            }
            missionSo.ApplyModifiedPropertiesWithoutUndo();

            // World pattern library + prefab bindings.
            var worldManager = systems.GetComponent<WorldManager>();
            var worldSo = new SerializedObject(worldManager);
            SerializedProperty patterns = worldSo.FindProperty("patterns");
            List<PatternDefinition> patternAssets = LoadAllSo<PatternDefinition>();
            patterns.arraySize = patternAssets.Count;
            for (int i = 0; i < patternAssets.Count; i++)
            {
                patterns.GetArrayElementAtIndex(i).objectReferenceValue = patternAssets[i];
            }

            worldSo.FindProperty("coinPrefab").objectReferenceValue = LoadPrefab("Coin");
            worldSo.FindProperty("pickupMagnetPrefab").objectReferenceValue = LoadPrefab("Pickup_Magnet");
            worldSo.FindProperty("pickupJetpackPrefab").objectReferenceValue = LoadPrefab("Pickup_Jetpack");
            worldSo.FindProperty("pickupShieldPrefab").objectReferenceValue = LoadPrefab("Pickup_Shield");
            worldSo.FindProperty("pickupMultiplierPrefab").objectReferenceValue = LoadPrefab("Pickup_Multiplier");
            worldSo.FindProperty("trackSegmentPrefab").objectReferenceValue = LoadPrefab("TrackSegment");

            SerializedProperty bindings = worldSo.FindProperty("obstacleBindings");
            var cellMap = new (TrackCell cell, string prefab)[]
            {
                (TrackCell.BarrierJump, "Obstacle_BarrierJump"),
                (TrackCell.BarrierSlide, "Obstacle_BarrierSlide"),
                (TrackCell.Cone, "Obstacle_Cone"),
                (TrackCell.Block, "Obstacle_Block"),
                (TrackCell.LowWall, "Obstacle_LowWall"),
                (TrackCell.Train, "Obstacle_Train"),
                (TrackCell.TrainMoving, "Obstacle_TrainMoving"),
                (TrackCell.Vehicle, "Obstacle_Vehicle")
            };
            bindings.arraySize = cellMap.Length;
            for (int i = 0; i < cellMap.Length; i++)
            {
                SerializedProperty element = bindings.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("cell").enumValueIndex = (int)cellMap[i].cell;
                element.FindPropertyRelative("prefab").objectReferenceValue = LoadPrefab(cellMap[i].prefab);
            }
            worldSo.ApplyModifiedPropertiesWithoutUndo();

            // Character select library.
            // (Panel wiring happens in BuildCanvas.)

            return systems;
        }

        // ==================================================================
        // Lighting + camera
        // ==================================================================

        private static void BuildLighting()
        {
            GameObject sun = new GameObject("Sun");
            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.96f, 0.88f);
            light.intensity = 1.25f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.88f;
            sun.transform.rotation = Quaternion.Euler(42f, 32f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.52f, 0.62f, 0.78f);
            RenderSettings.ambientEquatorColor = new Color(0.42f, 0.44f, 0.48f);
            RenderSettings.ambientGroundColor = new Color(0.18f, 0.17f, 0.16f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.012f;
            RenderSettings.fogColor = new Color(0.72f, 0.78f, 0.85f);

            // Procedural gradient skybox so no external HDRI is required.
            Material skybox = RenderSettings.skybox;
            Shader skyShader = Shader.Find("Skybox/Procedural") ?? Shader.Find("Skybox/Panoramic");
            if (skyShader != null)
            {
                skybox = new Material(skyShader);
                if (skybox.HasProperty("_SunSize")) skybox.SetFloat("_SunSize", 0.045f);
                if (skybox.HasProperty("_AtmosphereThickness")) skybox.SetFloat("_AtmosphereThickness", 1.15f);
                if (skybox.HasProperty("_Exposure")) skybox.SetFloat("_Exposure", 1.1f);
                AssetDatabase.CreateAsset(skybox, "Assets/Materials/M_Skybox.mat");
                RenderSettings.skybox = skybox;
            }

            // Wire sky controller references.
            var skyTime = Object.FindFirstObjectByType<SkyTimeController>();
            if (skyTime != null)
            {
                var so = new SerializedObject(skyTime);
                so.FindProperty("sun").objectReferenceValue = light;
                so.FindProperty("skyboxMaterial").objectReferenceValue = skybox;
                SerializedProperty biomes = so.FindProperty("biomes");
                biomes.arraySize = 3;
                biomes.GetArrayElementAtIndex(0).objectReferenceValue = LoadSo<BiomeDefinition>("Biome_Day");
                biomes.GetArrayElementAtIndex(1).objectReferenceValue = LoadSo<BiomeDefinition>("Biome_Dusk");
                biomes.GetArrayElementAtIndex(2).objectReferenceValue = LoadSo<BiomeDefinition>("Biome_Night");
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static GameObject BuildCamera()
        {
            GameObject cam = new GameObject("Main Camera");
            cam.tag = "MainCamera";
            Camera camera = cam.AddComponent<Camera>();
            camera.fieldOfView = 62f;
            camera.nearClipPlane = 0.15f;
            camera.farClipPlane = 320f;
            camera.allowHDR = true;
            camera.allowMSAA = true;
            camera.backgroundColor = new Color(0.45f, 0.55f, 0.68f);
            cam.transform.position = new Vector3(0f, 3.1f, -6.2f);
            cam.AddComponent<AudioListener>();
            cam.AddComponent<FollowCamera>();
            cam.AddComponent<CameraShake>();

            // Depth-of-field-ish look is handled by URP post-processing if added
            // manually (optional); the game runs fine without it.
            return cam;
        }

        // ==================================================================
        // Player + chaser
        // ==================================================================

        private static GameObject BuildPlayer()
        {
            GameObject prefab = LoadPrefab("Player");
            GameObject player = prefab != null
                ? (GameObject)PrefabUtility.InstantiatePrefab(prefab)
                : new GameObject("Player");
            player.transform.position = new Vector3(0f, 0f, 0f);
            return player;
        }

        private static void BuildChaser(Transform player)
        {
            GameObject prefab = LoadPrefab("Chaser");
            GameObject chaser = prefab != null
                ? (GameObject)PrefabUtility.InstantiatePrefab(prefab)
                : new GameObject("Chaser");
            chaser.transform.position = new Vector3(0f, 0f, -11f);

            var ai = chaser.GetComponent<ChaserAI>();
            if (ai != null)
            {
                var so = new SerializedObject(ai);
                so.FindProperty("player").objectReferenceValue = player;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Footstep audio source on the chaser.
            AudioSource footstepSource = chaser.GetComponent<AudioSource>();
            if (footstepSource == null)
            {
                footstepSource = chaser.AddComponent<AudioSource>();
            }
            footstepSource.playOnAwake = false;
            footstepSource.spatialBlend = 1f;
            footstepSource.maxDistance = 30f;

            if (ai != null)
            {
                var so = new SerializedObject(ai);
                so.FindProperty("footstepSource").objectReferenceValue = footstepSource;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // ==================================================================
        // Environment + weather FX
        // ==================================================================

        private static void BuildEnvironment()
        {
            GameObject prefab = LoadPrefab("RainFX");
            if (prefab == null)
            {
                return;
            }
            GameObject rain = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            rain.name = "RainFX";
            rain.SetActive(false);

            var weather = Object.FindFirstObjectByType<WeatherController>();
            if (weather != null)
            {
                var so = new SerializedObject(weather);
                so.FindProperty("rainParticles").objectReferenceValue = rain.GetComponent<ParticleSystem>();
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // ==================================================================
        // UI
        // ==================================================================

        private static Font _font;

        private static Font UiFont
        {
            get
            {
                if (_font == null)
                {
                    _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    if (_font == null)
                    {
                        _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    }
                }
                return _font;
            }
        }

        private static void BuildCanvas()
        {
            GameObject canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            // Event system.
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            // UIManager (created on the Systems object earlier) - needed for the
            // pause button and for panel registration at the end.
            UIManager uiManager = Object.FindFirstObjectByType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("[EndlessRails][SceneBuilder] UIManager not found - UI buttons cannot be wired!");
            }

            // --- HUD ---
            GameObject hud = BuildPanel(canvasGo.transform, "HUD");
            BuildText(hud.transform, "ScoreText", "0", new Vector2(0f, -46f), new Vector2(420f, 80f), 58, TextAnchor.UpperLeft, FontStyle.Bold);
            BuildText(hud.transform, "CoinText", "0", new Vector2(0f, -120f), new Vector2(420f, 52f), 38, TextAnchor.UpperLeft);
            BuildText(hud.transform, "DistanceText", "0 m", new Vector2(0f, -170f), new Vector2(420f, 44f), 30, TextAnchor.UpperLeft);
            BuildText(hud.transform, "MultiplierText", "x2", new Vector2(0f, -230f), new Vector2(420f, 44f), 34, TextAnchor.UpperLeft, FontStyle.Bold);
            BuildText(hud.transform, "FpsText", "", new Vector2(0f, 40f), new Vector2(220f, 40f), 24, TextAnchor.LowerRight);

            // Powerup rings (timer rings) top-right.
            float ringX = -260f;
            foreach (string ringName in new[] { "Magnet", "Jetpack", "Shield", "Multiplier" })
            {
                GameObject ring = BuildImage(hud.transform, ringName + "Ring", new Vector2(ringX, -70f), new Vector2(84f, 84f), new Color(1f, 1f, 1f, 0.85f));
                Image ringImage = ring.GetComponent<Image>();
                ringImage.type = Image.Type.Filled;
                ringImage.fillMethod = Image.FillMethod.Radial360;
                ringImage.fillAmount = 1f;
                ring.SetActive(false);
                GameObject icon = BuildImage(hud.transform, ringName + "Icon", new Vector2(ringX, -70f), new Vector2(52f, 52f), RingColor(ringName));
                icon.SetActive(false);
                ringX += 100f;
            }

            // Pause button.
            GameObject pauseButton = BuildButton(hud.transform, "PauseButton", "II", new Vector2(-120f, -50f), new Vector2(100f, 100f));
            WireButton(pauseButton, uiManager, nameof(UIManager.OnPausePressed));

            // --- Main menu ---
            GameObject menu = BuildPanel(canvasGo.transform, "MainMenu");
            MainMenu mainMenu = menu.AddComponent<MainMenu>();
            BuildText(menu.transform, "TitleText", "ENDLESS RAILS", new Vector2(0f, 300f), new Vector2(1100f, 140f), 96, TextAnchor.MiddleCenter, FontStyle.Bold);
            BuildText(menu.transform, "SubtitleText", "A procedural urban rail runner", new Vector2(0f, 205f), new Vector2(900f, 60f), 32, TextAnchor.MiddleCenter);

            GameObject play = BuildButton(menu.transform, "PlayButton", "PLAY", new Vector2(0f, 70f), new Vector2(460f, 110f));
            WireButton(play, mainMenu, nameof(MainMenu.OnPlay));
            GameObject characters = BuildButton(menu.transform, "CharactersButton", "CHARACTER SELECT", new Vector2(0f, -55f), new Vector2(460f, 84f));
            WireButton(characters, mainMenu, nameof(MainMenu.OnCharacterSelect));
            GameObject settings = BuildButton(menu.transform, "SettingsButton", "SETTINGS", new Vector2(0f, -155f), new Vector2(460f, 84f));
            WireButton(settings, mainMenu, nameof(MainMenu.OnSettings));
            GameObject scores = BuildButton(menu.transform, "ScoresButton", "HIGH SCORES", new Vector2(0f, -255f), new Vector2(460f, 84f));
            WireButton(scores, mainMenu, nameof(MainMenu.OnHighScores));
            GameObject quit = BuildButton(menu.transform, "QuitButton", "QUIT", new Vector2(0f, -355f), new Vector2(460f, 84f));
            WireButton(quit, mainMenu, nameof(MainMenu.OnQuit));

            BuildText(menu.transform, "BestScoreText", "Best: 0", new Vector2(620f, 70f), new Vector2(420f, 50f), 34, TextAnchor.MiddleLeft);
            BuildText(menu.transform, "CoinsText", "0", new Vector2(620f, 10f), new Vector2(420f, 50f), 34, TextAnchor.MiddleLeft);
            BuildText(menu.transform, "BestDistanceText", "Longest run: 0 m", new Vector2(620f, -50f), new Vector2(420f, 50f), 28, TextAnchor.MiddleLeft);
            BuildText(menu.transform, "MissionText", "", new Vector2(620f, -130f), new Vector2(480f, 90f), 26, TextAnchor.UpperLeft);

            // --- Pause menu ---
            GameObject pause = BuildPanel(canvasGo.transform, "PauseMenu");
            PauseMenu pauseMenu = pause.AddComponent<PauseMenu>();
            BuildText(pause.transform, "PauseTitle", "PAUSED", new Vector2(0f, 240f), new Vector2(700f, 110f), 80, TextAnchor.MiddleCenter, FontStyle.Bold);
            GameObject resume = BuildButton(pause.transform, "ResumeButton", "RESUME", new Vector2(0f, 90f), new Vector2(420f, 96f));
            WireButton(resume, pauseMenu, nameof(PauseMenu.OnResume));
            GameObject restart2 = BuildButton(pause.transform, "RestartButton", "RESTART", new Vector2(0f, -20f), new Vector2(420f, 96f));
            WireButton(restart2, pauseMenu, nameof(PauseMenu.OnRestart));
            GameObject pauseSettings = BuildButton(pause.transform, "SettingsButton", "SETTINGS", new Vector2(0f, -130f), new Vector2(420f, 96f));
            WireButton(pauseSettings, pauseMenu, nameof(PauseMenu.OnSettings));
            GameObject menuBtn = BuildButton(pause.transform, "MainMenuButton", "MAIN MENU", new Vector2(0f, -240f), new Vector2(420f, 96f));
            WireButton(menuBtn, pauseMenu, nameof(PauseMenu.OnMainMenu));

            // --- Game over ---
            GameObject gameover = BuildPanel(canvasGo.transform, "GameOver");
            GameOverScreen gameOverMenu = gameover.AddComponent<GameOverScreen>();
            BuildText(gameover.transform, "GameOverTitle", "CAUGHT!", new Vector2(0f, 300f), new Vector2(900f, 110f), 86, TextAnchor.MiddleCenter, FontStyle.Bold);
            BuildText(gameover.transform, "NewBestLabel", "NEW BEST!", new Vector2(0f, 215f), new Vector2(600f, 60f), 42, TextAnchor.MiddleCenter, FontStyle.Bold);
            BuildText(gameover.transform, "FinalScoreText", "0", new Vector2(0f, 130f), new Vector2(600f, 80f), 62, TextAnchor.MiddleCenter, FontStyle.Bold);
            BuildText(gameover.transform, "BestScoreText", "0", new Vector2(-220f, 30f), new Vector2(360f, 50f), 34, TextAnchor.MiddleCenter);
            BuildText(gameover.transform, "CoinsText", "+0", new Vector2(220f, 30f), new Vector2(360f, 50f), 34, TextAnchor.MiddleCenter);
            BuildText(gameover.transform, "DistanceText", "0 m", new Vector2(0f, -30f), new Vector2(500f, 50f), 32, TextAnchor.MiddleCenter);
            GameObject revive = BuildButton(gameover.transform, "ReviveButton", "REVIVE (AD)", new Vector2(0f, -140f), new Vector2(420f, 92f));
            WireButton(revive, gameOverMenu, nameof(GameOverScreen.OnRevive));
            GameObject goRestart = BuildButton(gameover.transform, "RestartButton", "RESTART", new Vector2(0f, -245f), new Vector2(420f, 92f));
            WireButton(goRestart, gameOverMenu, nameof(GameOverScreen.OnRestart));
            GameObject goMenu = BuildButton(gameover.transform, "MainMenuButton", "MAIN MENU", new Vector2(0f, -350f), new Vector2(420f, 92f));
            WireButton(goMenu, gameOverMenu, nameof(GameOverScreen.OnMainMenu));

            // --- Settings ---
            GameObject settingsPanel = BuildPanel(canvasGo.transform, "Settings");
            SettingsMenu settingsMenu = settingsPanel.AddComponent<SettingsMenu>();
            BuildText(settingsPanel.transform, "SettingsTitle", "SETTINGS", new Vector2(0f, 330f), new Vector2(800f, 100f), 72, TextAnchor.MiddleCenter, FontStyle.Bold);
            BuildSlider(settingsPanel.transform, "MusicSlider", "Music Volume", new Vector2(0f, 200f));
            BuildSlider(settingsPanel.transform, "SfxSlider", "SFX Volume", new Vector2(0f, 90f));
            BuildSlider(settingsPanel.transform, "AmbientSlider", "Ambient Volume", new Vector2(0f, -20f));
            BuildSlider(settingsPanel.transform, "SensitivitySlider", "Input Sensitivity", new Vector2(0f, -130f));

            // Quality dropdown.
            BuildText(settingsPanel.transform, "QualityLabel", "Graphics Quality", new Vector2(-260f, -230f), new Vector2(320f, 46f), 30, TextAnchor.MiddleLeft);
            GameObject dropdownGo = new GameObject("QualityDropdown", typeof(RectTransform));
            dropdownGo.transform.SetParent(settingsPanel.transform, false);
            RectTransform dropdownRect = dropdownGo.GetComponent<RectTransform>();
            dropdownRect.anchoredPosition = new Vector2(160f, -230f);
            dropdownRect.sizeDelta = new Vector2(360f, 60f);
            Image dropdownImage = dropdownGo.AddComponent<Image>();
            dropdownImage.color = new Color(0.16f, 0.17f, 0.22f, 0.95f);
            Dropdown dropdown = dropdownGo.AddComponent<Dropdown>();
            dropdown.options.Add(new Dropdown.OptionData("Low"));
            dropdown.options.Add(new Dropdown.OptionData("Medium"));
            dropdown.options.Add(new Dropdown.OptionData("High"));
            Text dropdownLabel = BuildText(dropdownGo.transform, "Label", "High", Vector2.zero, new Vector2(340f, 56f), 30, TextAnchor.MiddleCenter).GetComponent<Text>();
            dropdown.captionText = dropdownLabel;
            dropdown.value = 2;

            // Toggles.
            BuildToggle(settingsPanel.transform, "FirstPersonToggle", "First-person camera", new Vector2(0f, -310f));
            BuildToggle(settingsPanel.transform, "FpsToggle", "Show FPS counter", new Vector2(0f, -380f));

            GameObject settingsBack = BuildButton(settingsPanel.transform, "BackButton", "BACK", new Vector2(0f, -450f), new Vector2(320f, 84f));
            WireButton(settingsBack, settingsMenu, nameof(SettingsMenu.OnBack));

            // --- Character select ---
            GameObject characterPanel = BuildPanel(canvasGo.transform, "CharacterSelect");
            CharacterSelectMenu characterMenu = characterPanel.AddComponent<CharacterSelectMenu>();
            BuildText(characterPanel.transform, "CharacterTitle", "CHARACTER SELECT", new Vector2(0f, 330f), new Vector2(900f, 100f), 66, TextAnchor.MiddleCenter, FontStyle.Bold);
            BuildText(characterPanel.transform, "CoinsText", "0", new Vector2(600f, 330f), new Vector2(360f, 60f), 36, TextAnchor.MiddleRight);
            BuildText(characterPanel.transform, "StatusText", "", new Vector2(0f, -300f), new Vector2(700f, 50f), 30, TextAnchor.MiddleCenter);

            GameObject listRoot = new GameObject("CardList", typeof(RectTransform));
            listRoot.transform.SetParent(characterPanel.transform, false);
            RectTransform listRect = listRoot.GetComponent<RectTransform>();
            listRect.anchoredPosition = new Vector2(0f, 60f);
            listRect.sizeDelta = new Vector2(1400f, 520f);
            VerticalLayoutGroup layout = listRoot.AddComponent<VerticalLayoutGroup>();
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.spacing = 18f;
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            // Template card (disabled, used by CharacterSelectMenu).
            GameObject cardTemplate = BuildButton(listRoot.transform, "CharacterCard", "Character", Vector2.zero, new Vector2(700f, 110f));
            cardTemplate.SetActive(false);
            var layoutElement = cardTemplate.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 110f;

            GameObject characterBack = BuildButton(characterPanel.transform, "BackButton", "BACK", new Vector2(0f, -420f), new Vector2(320f, 84f));
            WireButton(characterBack, characterMenu, nameof(CharacterSelectMenu.OnBack));

            var charSo = new SerializedObject(characterMenu);
            charSo.FindProperty("characters").arraySize = 3;
            charSo.FindProperty("characters").GetArrayElementAtIndex(0).objectReferenceValue = LoadSo<CharacterDefinition>("Character_Male");
            charSo.FindProperty("characters").GetArrayElementAtIndex(1).objectReferenceValue = LoadSo<CharacterDefinition>("Character_Female");
            charSo.FindProperty("characters").GetArrayElementAtIndex(2).objectReferenceValue = LoadSo<CharacterDefinition>("Character_MaleAlt");
            charSo.FindProperty("listRoot").objectReferenceValue = listRoot.transform;
            charSo.FindProperty("cardPrefab").objectReferenceValue = cardTemplate.GetComponent<Button>();
            charSo.ApplyModifiedPropertiesWithoutUndo();

            // --- High scores ---
            GameObject scoresPanel = BuildPanel(canvasGo.transform, "HighScores");
            HighScoresScreen highScoresMenu = scoresPanel.AddComponent<HighScoresScreen>();
            BuildText(scoresPanel.transform, "HighScoresTitle", "HIGH SCORES", new Vector2(0f, 330f), new Vector2(900f, 100f), 72, TextAnchor.MiddleCenter, FontStyle.Bold);
            BuildText(scoresPanel.transform, "StatsText", "", new Vector2(0f, 20f), new Vector2(800f, 520f), 34, TextAnchor.UpperCenter);
            GameObject scoresBack = BuildButton(scoresPanel.transform, "BackButton", "BACK", new Vector2(0f, -380f), new Vector2(320f, 84f));
            WireButton(scoresBack, highScoresMenu, nameof(HighScoresScreen.OnBack));

            // --- Tutorial overlay ---
            GameObject tutorial = BuildPanel(canvasGo.transform, "Tutorial");
            Image tutorialBg = tutorial.GetComponent<Image>();
            if (tutorialBg != null)
            {
                tutorialBg.color = new Color(0f, 0f, 0f, 0.82f);
            }
            BuildText(tutorial.transform, "TutorialTitle", "HOW TO PLAY", new Vector2(0f, 260f), new Vector2(900f, 100f), 70, TextAnchor.MiddleCenter, FontStyle.Bold);
            BuildText(tutorial.transform, "BodyText", "", new Vector2(0f, -20f), new Vector2(1000f, 480f), 36, TextAnchor.MiddleCenter);
            TutorialOverlay tutorialOverlay = tutorial.AddComponent<TutorialOverlay>();

            // Tap anywhere on the overlay dismisses it (the panel image is the
            // touch target - no visible colour change), plus an explicit button.
            Button tutorialTapArea = tutorial.AddComponent<Button>();
            tutorialTapArea.transition = Selectable.Transition.None;
            WireButton(tutorial, tutorialOverlay, nameof(TutorialOverlay.Dismiss));

            GameObject tutorialStart = BuildButton(tutorial.transform, "StartButton", "START", new Vector2(0f, -350f), new Vector2(400f, 104f));
            WireButton(tutorialStart, tutorialOverlay, nameof(TutorialOverlay.Dismiss));

            // --- Panel registration on UIManager ---
            if (uiManager != null)
            {
                var so = new SerializedObject(uiManager);
                SerializedProperty panels = so.FindProperty("panels");
                string[] names = { "HUD", "MainMenu", "PauseMenu", "GameOver", "Settings", "CharacterSelect", "HighScores", "Tutorial" };
                panels.arraySize = names.Length;
                for (int i = 0; i < names.Length; i++)
                {
                    SerializedProperty entry = panels.GetArrayElementAtIndex(i);
                    entry.FindPropertyRelative("name").stringValue = names[i];
                    entry.FindPropertyRelative("panel").objectReferenceValue = canvasGo.transform.Find(names[i])?.gameObject;
                }
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Final check: every button must carry at least one *persistent* listener,
            // otherwise the click would be dead in the built APK.
            VerifyButtonWiring(canvasGo);
        }

        /// <summary>
        /// Logs how many buttons in the canvas have persistent (serialized) click
        /// handlers. Buttons without one are silently dead in a build, so any
        /// unexpected entry here is a real bug. (The disabled CharacterCard
        /// template is wired at runtime by CharacterSelectMenu - expected.)
        /// </summary>
        private static void VerifyButtonWiring(GameObject canvasGo)
        {
            int wired = 0;
            int expectedRuntime = 0;
            foreach (Button button in canvasGo.GetComponentsInChildren<Button>(true))
            {
                if (button.onClick.GetPersistentEventCount() > 0)
                {
                    wired++;
                }
                else if (button.name == "CharacterCard")
                {
                    expectedRuntime++;
                }
                else
                {
                    Debug.LogError($"[EndlessRails][SceneBuilder] Button '{button.name}' has NO persistent click listener - it would be dead in the build!");
                }
            }
            Debug.Log($"[EndlessRails][SceneBuilder] Button wiring verified: {wired} persistent, {expectedRuntime} runtime-only (card template).");
        }

        private static Color RingColor(string ringName)
        {
            return ringName switch
            {
                "Magnet" => new Color(0.9f, 0.25f, 0.25f),
                "Jetpack" => new Color(0.95f, 0.6f, 0.12f),
                "Shield" => new Color(0.35f, 0.75f, 1f),
                _ => new Color(0.2f, 0.8f, 0.35f)
            };
        }

        // --- UI construction helpers ---

        private static GameObject BuildPanel(Transform parent, string name)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image image = panel.AddComponent<Image>();
            image.color = name == "HUD" ? new Color(0f, 0f, 0f, 0f) : new Color(0.05f, 0.07f, 0.12f, 0.93f);
            // The HUD is a transparent full-screen layer - it must never intercept
            // touches. Every other panel keeps its image as the touch surface.
            image.raycastTarget = name != "HUD";
            return panel;
        }

        private static GameObject BuildText(Transform parent, string name, string content, Vector2 position,
            Vector2 size, int fontSize, TextAnchor anchor, FontStyle style = FontStyle.Normal)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Text text = go.AddComponent<Text>();
            text.font = UiFont;
            text.text = content;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = anchor;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            // Labels are decorative - they must NEVER block raycasts, otherwise a
            // text drawn above a button silently swallows the tap.
            text.raycastTarget = false;
            return go;
        }

        private static GameObject BuildImage(Transform parent, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Image image = go.AddComponent<Image>();
            image.color = color;
            return go;
        }

        private static GameObject BuildButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = go.AddComponent<Image>();
            image.color = new Color(0.16f, 0.32f, 0.52f, 0.95f);

            Button button = go.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(0.22f, 0.42f, 0.65f, 1f);
            colors.pressedColor = new Color(0.1f, 0.22f, 0.38f, 1f);
            button.colors = colors;

            BuildText(go.transform, "Label", label, Vector2.zero, size, Mathf.RoundToInt(size.y * 0.42f), TextAnchor.MiddleCenter, FontStyle.Bold);
            return go;
        }

        private static void BuildSlider(Transform parent, string name, string label, Vector2 position)
        {
            BuildText(parent, name + "Label", label, position + new Vector2(-260f, 0f), new Vector2(320f, 46f), 30, TextAnchor.MiddleLeft);

            GameObject sliderGo = new GameObject(name, typeof(RectTransform));
            sliderGo.transform.SetParent(parent, false);
            RectTransform sliderRect = sliderGo.GetComponent<RectTransform>();
            sliderRect.anchoredPosition = position + new Vector2(160f, 0f);
            sliderRect.sizeDelta = new Vector2(360f, 40f);

            // Background.
            GameObject background = BuildImage(sliderGo.transform, "Background", Vector2.zero, new Vector2(360f, 16f), new Color(0.2f, 0.22f, 0.28f));
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0.5f);
            bgRect.anchorMax = new Vector2(1f, 0.5f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Fill area + fill.
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderGo.transform, false);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.5f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.5f);
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            GameObject fill = BuildImage(fillArea.transform, "Fill", Vector2.zero, new Vector2(360f, 16f), new Color(0.32f, 0.62f, 0.88f));
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0.5f);
            fillRect.anchorMax = new Vector2(1f, 0.5f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            // Handle.
            GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderGo.transform, false);
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = new Vector2(0f, 0f);
            handleAreaRect.anchorMax = new Vector2(1f, 1f);
            handleAreaRect.offsetMin = Vector2.zero;
            handleAreaRect.offsetMax = Vector2.zero;

            GameObject handle = BuildImage(handleArea.transform, "Handle", Vector2.zero, new Vector2(28f, 28f), Color.white);
            RectTransform handleRect = handle.GetComponent<RectTransform>();

            Slider slider = sliderGo.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.direction = Slider.Direction.LeftToRight;
            slider.value = 0.8f;

            // Large invisible touch area (the visible bar is only 16px tall,
            // which is unusable on a phone). Drags bubble up to the Slider.
            BuildImage(sliderGo.transform, "HitArea", Vector2.zero, new Vector2(380f, 72f), new Color(1f, 1f, 1f, 0f));
        }

        private static void BuildToggle(Transform parent, string name, string label, Vector2 position)
        {
            GameObject toggleGo = new GameObject(name, typeof(RectTransform));
            toggleGo.transform.SetParent(parent, false);
            RectTransform rect = toggleGo.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(520f, 54f);

            GameObject background = BuildImage(toggleGo.transform, "Background", new Vector2(-230f, 0f), new Vector2(48f, 48f), new Color(0.2f, 0.22f, 0.28f));
            GameObject checkmark = BuildImage(toggleGo.transform, "Checkmark", new Vector2(-230f, 0f), new Vector2(30f, 30f), new Color(0.32f, 0.62f, 0.88f));

            Text labelText = BuildText(toggleGo.transform, "Label", label, new Vector2(80f, 0f), new Vector2(420f, 50f), 30, TextAnchor.MiddleLeft).GetComponent<Text>();
            labelText.alignment = TextAnchor.MiddleLeft;

            Toggle toggle = toggleGo.AddComponent<Toggle>();
            toggle.targetGraphic = background.GetComponent<Image>();
            toggle.graphic = checkmark.GetComponent<Image>();

            // Whole row is tappable (invisible touch area over box + label).
            BuildImage(toggleGo.transform, "HitArea", Vector2.zero, new Vector2(520f, 54f), new Color(1f, 1f, 1f, 0f));
        }

        /// <summary>
        /// Adds a *persistent* click listener to a button pointing at a component
        /// method. This is the crucial difference from onClick.AddListener():
        /// a runtime AddListener() from an editor script is never serialized into
        /// the scene, so the button is dead in a built player. Persistent listeners
        /// (UnityEventTools) are stored inside the scene asset and work everywhere.
        /// Using nameof() + a component reference means the method name is checked
        /// at compile time - a typo can no longer produce a silent dead button.
        /// </summary>
        private static void WireButton<T>(GameObject buttonGo, T component, string methodName) where T : Component
        {
            Button button = buttonGo != null ? buttonGo.GetComponent<Button>() : null;
            if (button == null)
            {
                Debug.LogError($"[EndlessRails][SceneBuilder] WireButton: no Button on '{(buttonGo != null ? buttonGo.name : "null")}'.");
                return;
            }
            if (component == null)
            {
                Debug.LogError($"[EndlessRails][SceneBuilder] WireButton: component is null for button '{buttonGo.name}' -> {methodName}.");
                return;
            }

            var method = component.GetType().GetMethod(methodName, System.Type.EmptyTypes);
            if (method == null)
            {
                Debug.LogError($"[EndlessRails][SceneBuilder] WireButton: method {component.GetType().Name}.{methodName} not found for button '{buttonGo.name}'.");
                return;
            }

            var action = (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(
                typeof(UnityEngine.Events.UnityAction), component, method);
            UnityEventTools.AddPersistentListener(button.onClick, action);
        }

        // ==================================================================
        // Final wiring
        // ==================================================================

        private static void WireReferences(GameObject systems, GameObject camera, GameObject player)
        {
            // Follow camera target.
            FollowCamera followCamera = camera.GetComponent<FollowCamera>();
            if (followCamera != null)
            {
                var so = new SerializedObject(followCamera);
                so.FindProperty("target").objectReferenceValue = player.transform;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Weather follow target.
            WeatherController weather = systems.GetComponentInChildren<WeatherController>();
            if (weather != null)
            {
                var so = new SerializedObject(weather);
                so.FindProperty("followTarget").objectReferenceValue = camera.transform;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // World manager player reference is passed at run start; also set it here.
            WorldManager world = systems.GetComponent<WorldManager>();
            if (world != null)
            {
                var so = new SerializedObject(world);
                // (player transform is passed via BeginRun - triggered below)
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Audio relay player reference.
            AudioEventRelay relay = systems.GetComponent<AudioEventRelay>();
            if (relay != null)
            {
                var so = new SerializedObject(relay);
                so.FindProperty("player").objectReferenceValue = player.transform;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Run-start hook: auto-start world generation when a run begins.
            RunStarter starter = systems.AddComponent<RunStarter>();
            var starterSo = new SerializedObject(starter);
            starterSo.FindProperty("player").objectReferenceValue = player.transform;
            GameObject tutorialPanel = GameObject.Find("Canvas/Tutorial");
            if (tutorialPanel != null)
            {
                starterSo.FindProperty("tutorialPanel").objectReferenceValue = tutorialPanel;
            }
            starterSo.ApplyModifiedPropertiesWithoutUndo();
        }

        // ==================================================================
        // Asset loading helpers
        // ==================================================================

        private static GameObject LoadPrefab(string name) =>
            AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/{name}.prefab");

        private static T LoadSo<T>(string name) where T : ScriptableObject =>
            AssetDatabase.LoadAssetAtPath<T>($"{SoDir}/{name}.asset");

        private static List<T> LoadAllSo<T>() where T : ScriptableObject
        {
            var result = new List<T>();
            foreach (string guid in AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { SoDir }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    result.Add(asset);
                }
            }
            return result;
        }
    }
}
