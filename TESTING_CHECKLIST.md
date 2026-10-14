# Quality Verification Checklist — Endless Rails

Run through this list before shipping. Target device class: Snapdragon 720G-class
mid-range Android (e.g. Redmi Note 9 Pro / Galaxy A52), 60 Hz display.

## 0. UI Smoke Test (run this first — every clickable element)
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 0.1 | Tutorial overlay dismisses | Tap anywhere on it, or press START | ☐ |
| 0.2 | Main menu buttons all respond | PLAY, CHARACTER SELECT, SETTINGS, HIGH SCORES, QUIT | ☐ |
| 0.3 | Character select cards are tappable (buy/select) | Tap each card + BACK | ☐ |
| 0.4 | Settings sliders drag smoothly with a finger | Drag each of the 4 sliders | ☐ |
| 0.5 | Settings toggles + quality dropdown respond | Tap each row, open the dropdown | ☐ |
| 0.6 | Pause button (top-right) pauses and resumes | Tap it twice mid-run | ☐ |
| 0.7 | Pause menu buttons work | RESUME, RESTART, SETTINGS, MAIN MENU | ☐ |
| 0.8 | Game over buttons work | REVIVE, RESTART, MAIN MENU | ☐ |
| 0.9 | BACK buttons on Settings / Characters / High Scores | Tap BACK on each screen | ☐ |

Notes: click handlers are *persistent* listeners stored in the scene
(`SceneBuilder.WireButton` -> `UnityEventTools.AddPersistentListener`), so they
survive into the APK. `Tools/check_sources.py` fails the build check if editor
code ever uses runtime-only `onClick.AddListener()` again, and the scene builder
logs a wiring summary (`VerifyButtonWiring`).

## 1. Core Loop
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 1.1 | Main menu → Play → Game Over → Restart works with zero errors | Full loop ×3, watch Console (editor) and `adb logcat` (device) | ☐ |
| 1.2 | Pause / Resume / Main Menu from pause works | Tap pause mid-run | ☐ |
| 1.3 | Game Over screen shows score, best, coins, distance | Force a crash | ☐ |
| 1.4 | Revive (ad placeholder) resumes the run | Tap REVIVE after game over | ☐ |

## 2. Controls & Feel
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 2.1 | Swipe left/right changes lane with eased motion (no snapping) | Slow lane changes back-to-back | ☐ |
| 2.2 | Swipe up jumps over barriers | Jump over `Obstacle_BarrierJump` | ☐ |
| 2.3 | Swipe down slides under beams | Slide under `Obstacle_BarrierSlide` | ☐ |
| 2.4 | Double-queued swipes register (fast play) | Rapid double lane changes | ☐ |
| 2.5 | Tap pauses the game | Tap empty screen area | ☐ |
| 2.6 | Android back button pauses | Hardware back during run | ☐ |

## 3. Collisions
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 3.1 | Lethal obstacles end the run | Hit train/block/wall/vehicle | ☐ |
| 3.2 | Cones cause stumble (not death), chaser gains ground | Hit a cone | ☐ |
| 3.3 | Jumping over a barrier never false-positives | Clean jump arc over barrier | ☐ |
| 3.4 | Sliding under a beam never false-positives | Clean slide | ☐ |
| 3.5 | Coins collect on contact, magnet pulls coins through obstacles | Grab magnet, run near coins | ☐ |
| 3.6 | Shield absorbs exactly one lethal hit | Pick up shield, crash once, crash again | ☐ |
| 3.7 | Jetpack flies over everything for its duration | Grab jetpack | ☐ |
| 3.8 | No missed hits when clipping obstacle corners | Graze each obstacle type at speed | ☐ |

## 4. Persistence
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 4.1 | Coin wallet persists across app restarts | Earn coins, force-stop, relaunch | ☐ |
| 4.2 | High score persists | Beat best score, relaunch | ☐ |
| 4.3 | Settings (volume/sensitivity/quality) persist | Change, relaunch | ☐ |
| 4.4 | Character unlocks persist | Buy character, relaunch | ☐ |
| 4.5 | Tutorial shows only once | Clear data → launch → dismiss → relaunch | ☐ |
| 4.6 | Mission progress accumulates across runs | Collect coins over 2 runs | ☐ |
| 4.7 | Corrupt save file recovers gracefully | Truncate save.json, launch | ☐ |

## 5. Memory & Performance
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 5.1 | 10-minute session with no memory leak / GC spikes | Android Profiler: total alloc ≈ 0 B/frame in steady state | ☐ |
| 5.2 | Object counts stay flat over 10 minutes | Profiler → Hierarchy → total object count | ☐ |
| 5.3 | Stable 60 FPS on target device (High quality) | FPS counter on device, worst-case dense spawns | ☐ |
| 5.4 | No hitches when chunks recycle | Frame debugger / profiler timeline | ☐ |
| 5.5 | Low/Medium quality tiers hold 60 FPS on weaker hardware | Settings → Quality | ☐ |

## 6. Audio
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 6.1 | Every action plays its sound (jump, land, slide, lane, coin, crash, stumble) | Perform each action | ☐ |
| 6.2 | Footsteps sync with run cycle | Visual/audio sync at speed | ☐ |
| 6.3 | Train horn/rumble attenuates with distance | Approach moving train | ☐ |
| 6.4 | Chaser footsteps/shouts scale with distance | Stumble vs. fast run | ☐ |
| 6.5 | Music/SFX/Ambient sliders work independently | Settings sliders | ☐ |
| 6.6 | Rain ambience switches with weather | Run until rain weather rolls | ☐ |
| 6.7 | No audio clicks/pops when pooling sources | Rapid coin pickups | ☐ |

## 7. Visuals & Camera
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 7.1 | Camera follows smoothly with damping; no jitter at speed | Watch corners/landings | ☐ |
| 7.2 | FOV widens subtly with speed | Compare start vs. max speed | ☐ |
| 7.3 | Camera shake on landings/crashes | Jump from jetpack height, crash | ☐ |
| 7.4 | First-person toggle works and persists | Settings toggle | ☐ |
| 7.5 | Time of day shifts sun/sky/fog over a long run | 5+ minute run | ☐ |
| 7.6 | Night biome street lamps read well | Run ~2700 m | ☐ |
| 7.7 | Rain particles follow the camera; wet materials gloss up | Rain weather | ☐ |
| 7.8 | UI scales correctly on 16:9, 19.5:9, and a tablet | Multiple devices/resolutions | ☐ |

## 8. Devices & Build
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 8.1 | Release APK installs on a physical device | `adb install -r Builds/Android/EndlessRails.apk` | ☐ |
| 8.2 | Release build runs without the Unity profiler/editor attached | Cold start from launcher | ☐ |
| 8.3 | AAB passes bundle validation | `bundletool build-apks` / Play Console upload | ☐ |
| 8.4 | arm64-only, API 24 min — installs on Android 7 and Android 14+ | Two devices/emulators | ☐ |
| 8.5 | App requests **no** unnecessary permissions | Settings → App → Permissions | ☐ |
| 8.6 | Cold start < 5 s on target device | Stopwatch from icon tap | ☐ |
| 8.7 | Process death + resume recovers (no black screen) | Background, kill, relaunch | ☐ |

## 9. Content Safety
| # | Check | How to verify | Pass? |
|---|---|---|---|
| 9.1 | No third-party trademarks/copyrighted assets present | Audit `Assets/` + this README | ☐ |
| 9.2 | All shipped audio/textures are project-generated CC0 | `Tools/generate_placeholder_assets.py` is the only source | ☐ |
| 9.3 | LICENSES.md accompanies any distribution | Include in zip/AAB listing | ☐ |

---

### Automated assistance
- `python3 Tools/check_sources.py` — static source consistency checks (brace balance, event/API cross-references, tag validity). Run after any code change.
- `Tools/build_android.sh` — full clean build pipeline; a non-zero exit fails the checklist.
