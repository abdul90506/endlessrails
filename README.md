# Endless Rails — 3D Endless Runner for Android (Unity 6 / URP)

A complete, original 3D endless runner in the style of classic subway-running games — built from scratch with **no copyrighted characters, names, logos, or assets**. Run down an endless urban railway, dodge trains and barriers, collect coins and power-ups, and stay ahead of the security officer on your tail.

![status](https://img.shields.io/badge/engine-Unity%206%20(URP)-blue) ![platform](https://img.shields.io/badge/platform-Android%20API%2024%2B%20arm64-green) ![license](https://img.shields.io/badge/code-MIT-blue)

---

## ⚡ Quick Start (5 minutes)

1. **Install Unity 6** (6000.0 LTS or newer) via Unity Hub, with **Android Build Support + IL2CPP + Android SDK & NDK**.
2. Open this folder (`EndlessRails/`) as a Unity project (Unity Hub → *Add* → select folder → *Open*).
   - If prompted about the input backend, keep **Active Input Handling = Input Manager (old)** — the project uses the built-in Input class (already set in `ProjectSettings.asset`).
3. In Unity, run **in order** (one menu click each — everything is generated in code):
   - `Tools → Endless Rails → 1. Setup Project (do this first)`
   - `Tools → Endless Rails → 2. Generate Materials, Prefabs & Configs`
   - `Tools → Endless Rails → 3. Build Main Scene`
4. Open `Assets/Scenes/Main.unity` (if not already open) and press **Play**.

That's it — the game runs end-to-end: Main Menu → Tutorial (first launch) → Play → Game Over → Restart/Menu, with audio, coins, power-ups, missions and persistent saves.

## 📱 Build the APK / AAB

### In the Editor
- `Tools → Endless Rails → 4. Build Android APK (release)` → `Builds/Android/EndlessRails.apk`
- `Tools → Endless Rails → 5. Build Android AAB (Play Store)` → `Builds/Android/EndlessRails.aab`

A development keystore is included (`Tools/keystore/endlessrails-release.keystore`, password `endlessrails2026`). **Replace it with your own keystore before publishing** (see *Signing* below).

### From the command line
```bash
UNITY_PATH="$HOME/Unity/Hub/Editor/6000.0.32f1/Editor/Unity" ./Tools/build_android.sh
```

### On Unity Cloud Build Automation (free tier — no local Unity needed)
1. Push this project to a GitHub repo (include `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/keystore/`).
2. [cloud.unity.com](https://cloud.unity.com) → **DevOps → Build Automation** → connect your GitHub repo (Settings → Source control).
3. **Configurations** → new target → Platform **Android**, branch `main`, Unity version auto-detected from `ProjectVersion.txt`.
4. Leave custom/pre-export method fields **empty** — `CloudBuildGuard` (in `Assets/Editor/`) automatically generates all materials, prefabs, configs and the Main scene on the build machine before the export step, and registers the scene in EditorBuildSettings. (Optional: set the *Pre-export method* to `EndlessRails.EditorTools.BuildScript.BuildAndroidCloud` for explicit preparation; it does not build, so it is safe.)
5. **Save → Build now**. Download the APK from Build history when it finishes.
6. Free allowance (renews monthly): 200 Windows build minutes + 100 Mac/Linux minutes, 25 GB storage, 100 GB egress — an Android build of this project typically uses 15–30 minutes.

### On CI (GitHub Actions)
`.github/workflows/android-build.yml` builds a signed APK on every push using [game-ci](https://game.ci). Add your Unity license and keystore as repo secrets (documented in the workflow file).

### Install on a device
```bash
adb install -r Builds/Android/EndlessRails.apk
```

---

## 🎮 Controls

| Input | Action |
|---|---|
| Swipe ◀ / ▶ | Change lane (smooth eased interpolation) |
| Swipe ▲ | Jump |
| Swipe ▼ | Slide / roll (also fast-falls into a slide) |
| Tap | Pause |
| Back / Esc | Pause (Android back button supported) |
| Keyboard (editor) | A/D or ◀/▶ lanes · W/Space/▲ jump · S/▼ slide · Esc pause |

## ✨ Features

**Gameplay**
- 3-lane endless auto-runner with procedural, chunk-based track generation (patterns + weighted difficulty — never ends, never repeats identically)
- Obstacles: stationary & moving trains, jump barriers, overhead slide beams, cones (stumble), construction blocks, low walls, crossing vehicles
- Difficulty ramp: speed ↑, spawn density ↑, reaction windows ↓
- Crash = slow-motion hit moment + physics ragdoll
- Coins (spinning, magnet-able) and power-ups: **Magnet**, **Jetpack**, **Shield**, **Score Multiplier**, each with HUD timer rings
- Chasing security officer: gains ground when you stumble or run slow, falls behind at speed, distance-based footsteps and shouts; catching you ends the run
- Score = distance + coins; persistent high score and coin wallet

**Character & animation**
- Male/female runner options + unlockable variants (purchasable with coins)
- Built-in procedural mannequin with code-driven animation: run cycle, lane lean, jump, slide, stumble, ragdoll on crash, ponytail physics
- `PlayerAnimatorDriver` accepts any rigged humanoid (e.g. Mixamo) — drop in a model + Animator Controller and it takes over automatically (blend-tree parameter contract documented in the script header)
- Footstep emission synced to run phase / animation events

**Environment**
- Urban railway setting: ballast, rails, sleepers, platforms, fences, graffiti walls, lamp posts, puddles, signage
- Procedurally generated PBR materials (albedo + normal + roughness/AO/metallic mask)
- Dynamic time of day (sunrise→dusk), biome rotation (Day / Sunset / Night), exponential fog
- Weather variants: clear, overcast, light rain (particle rain + wet-surface smoothness response)
- Real-time soft shadows, ambient trilight, subtle FOV kick with speed, trauma camera shake, optional first-person mode

**Audio**
- Synthesized SFX set: footsteps per surface (concrete/gravel/metal/wet), jump whoosh, land thud, slide scrape, coin clink (pitch randomization), crash, stumble, train horn/rumble, power-up/down, shield break, chaser shout/steps, UI clicks
- Ambient city loop, wind, rain ambience, original looping background music
- 3D spatial one-shots via a pooled AudioSources (zero GC), bus-style volume control (Music / SFX / Ambient) in Settings; optional AudioMixer routing hooks included

**UI / UX**
- Main Menu, Character Select, Settings (volume, graphics quality, input sensitivity, first-person, FPS counter), High Scores / lifetime stats, Pause, Game Over (with ad-placeholder Revive), first-launch tutorial
- CanvasScaler-based UI that scales correctly across 16:9, 19.5:9 and tablets

**Technical**
- Object pooling everywhere (obstacles, coins, pickups, track segments, SFX sources) — no runtime allocation in steady state, no GC spikes
- Modular systems: Input, Player, World, Enemy, Audio, Environment, UI, Save, Progression — communicate through a static event hub (`GameEvents`)
- ScriptableObject-driven config: `GameConfig`, `PatternDefinition`, `ObstacleDefinition`, `CharacterDefinition`, `BiomeDefinition`, `MissionDefinition`, `AudioLibrary`
- JSON save system at `Application.persistentDataPath/save.json` (coins, high score, unlocks, missions, settings)
- Missions / daily challenges with 24h reset and coin rewards
- Android: min API 24, **arm64-v8a only**, IL2CPP, landscape, linear color space, zero unnecessary permissions (no INTERNET even)

---

## 🗂 Project Structure

```
EndlessRails/
├── Assets/
│   ├── Editor/                  # One-click setup & build tools
│   │   ├── ProjectSetup.cs      #   Android/URP/quality configuration
│   │   ├── AssetFactory.cs      #   Generates materials, prefabs, ScriptableObject configs
│   │   ├── SceneBuilder.cs      #   Generates the complete Main scene + UI
│   │   └── BuildScript.cs       #   APK/AAB build entry points (menu + CLI)
│   ├── Scripts/
│   │   ├── Core/                # GameManager, events, RunStarter, ScriptableObjects
│   │   ├── Input/               # Swipe/keyboard input
│   │   ├── Player/              # Motor, controller, procedural rig, collisions, powerups
│   │   ├── Camera/              # Follow camera + shake
│   │   ├── World/               # Pooling, procedural track, coins, pickups, obstacles
│   │   ├── Enemy/               # Chasing officer AI
│   │   ├── Audio/               # Audio service + event relay
│   │   ├── Environment/         # Time of day, biomes, weather
│   │   ├── UI/                  # All screens
│   │   ├── Save/                # JSON persistence
│   │   └── Progression/         # Missions & dailies
│   ├── Textures/Generated/      # Procedural placeholder PBR textures (CC0)
│   ├── Audio/Generated/         # Synthesized placeholder SFX & music (CC0)
│   ├── Prefabs/                 # Generated by AssetFactory
│   ├── Materials/               # Generated by AssetFactory
│   ├── ScriptableObjects/       # Generated configs
│   └── Scenes/Main.unity        # Generated by SceneBuilder
├── ProjectSettings/             # Player settings (Android 24+, ARM64, IL2CPP)
├── Packages/manifest.json       # URP 17.x + uGUI + core modules
├── Tools/
│   ├── generate_placeholder_assets.py  # Regenerates all textures & audio
│   ├── build_android.sh        # CLI APK+AAB build
│   ├── generate_keystore.sh    # Development keystore generator
│   ├── check_sources.py        # Static source consistency checker
│   ├── compile_check/          # Offline compile test (dotnet, no Unity needed)
│   └── keystore/               # Development signing keystore
├── .github/workflows/          # game-ci Android build
├── README.md
├── LICENSES.md
└── TESTING_CHECKLIST.md
```

---

## 🧩 Extending the Game

### Add a new obstacle
1. Build a prefab under `Assets/Prefabs` (any visuals), add an `Obstacle` component and a **trigger** BoxCollider covering its footprint.
2. Tag it `Obstacle` (or `Train`/`Vehicle`/`Stumble`) and set `impact` (Lethal/Stumble), `canJumpOver`, `mustSlideUnder`.
3. Add a `CellBinding` entry in `WorldManager` mapping a `TrackCell` enum value to the prefab (or extend `TrackCell` in `Enums.cs`).
4. Reference the new cell in one or more `PatternDefinition` assets. Done — the procedural generator will use it.

### Add a new character
1. Create `Create → Endless Rails → Character Definition`.
2. Optionally assign a rigged humanoid `modelPrefab` + `animatorController` (Mixamo works well: export FBX with skin, rig = Humanoid, use the default run/jump animations or replace clips).
   - Expected Animator parameters: `Speed` (float), `Grounded` (bool), `Lean` (float), `Jetpack` (bool), triggers `Jump`, `Slide`, `Stumble`, `Hit`.
3. Set colors, price and id. The character appears in Character Select automatically once added to `CharacterSelectMenu.characters` (or the scene builder's list).

### Add a new track pattern
`Create → Endless Rails → Track Pattern`. Fill rows with `TrackCell` values; leave ≥1 lane passable. Weights: `baseWeight` (always), `difficultyWeightBonus` (grows with difficulty), `minDifficulty` (gate).

### Swap placeholder assets for realistic ones
The game is asset-agnostic — any PBR textures/audio can replace the generated set:
1. Drop textures into `Assets/Textures/Generated` (same `*_albedo/*_normal/*_mask` naming) — or re-point materials in `Assets/Materials`.
2. Drop audio into `Assets/Audio/Generated` with the same file names.
3. Re-run `Tools → Endless Rails → 2. Generate Materials, Prefabs & Configs`.
See `LICENSES.md` for recommended CC0 sources (Poly Haven, ambientCG, Mixamo, Freesound).

### Post-processing (optional)
To enable subtle bloom/DOF: install URP's Volume system → add a **Global Volume** with a Bloom intensity ≤ 0.35 and mild Tonemapping. The game runs correctly without it.

---

## 🔏 Signing

Development keystore (bundled):

| Field | Value |
|---|---|
| Path | `Tools/keystore/endlessrails-release.keystore` |
| Store password | `endlessrails2026` |
| Key alias | `endlessrails` |
| Key password | `endlessrails2026` |

**For production:** generate your own keystore and never commit it:
```bash
keytool -genkeypair -v -keystore release.keystore -alias youralias \
  -keyalg RSA -keysize 2048 -validity 10000 -storepass <pass> -keypass <pass>
```
Then update `BuildScript.cs` constants (or Edit → Project Settings → Player → Publishing Settings) and rotate the `bundleVersionCode` on each upload.

---

## ⚙️ Build Settings (already configured)

| Setting | Value |
|---|---|
| Min Android API | 24 (Android 7.0) |
| Target architectures | arm64-v8a (IL2CPP) |
| Orientation | Landscape (left+right) |
| Color space | Linear |
| Graphics API | Vulkan + GLES3 fallback |
| Permissions | none (empty custom manifest) |
| Input | Built-in Input Manager (legacy) |
| Quality tiers | Low / Medium / High (mapped in Settings) |

---

## 🧪 Testing

See `TESTING_CHECKLIST.md` for the full verification checklist (menu→gameplay→restart loop, collision accuracy, save persistence, memory stability over 10-minute sessions, frame-rate targets, device install).

Editor testing tips:
- Mouse drag simulates swipes; keyboard shortcuts listed above.
- FPS counter: Settings → *Show FPS counter*.
- To re-test the tutorial: delete `Application.persistentDataPath/save.json` (or Settings → Reset Progress).

---

## 📄 Licenses

- **Code**: MIT (see `LICENSES.md`)
- **Generated textures/audio**: CC0 (created procedurally by this project's tooling)
- **Third-party recommendations**: all CC0/free — listed in `LICENSES.md` with license terms

No Subway Surfers or other game IP is used anywhere — all names, visuals, code and audio are original to this project.
