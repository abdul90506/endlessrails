using UnityEngine;
using UnityEngine.UI;

namespace EndlessRails
{
    /// <summary>
    /// Settings screen: music/SFX/ambient volume, graphics quality,
    /// input sensitivity, camera mode and FPS counter toggle.
    /// All values persist through SaveSystem.
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider ambientSlider;
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Dropdown qualityDropdown;
        [SerializeField] private Toggle firstPersonToggle;
        [SerializeField] private Toggle fpsToggle;

        private void Awake()
        {
            if (musicSlider == null) musicSlider = transform.FindDeep<Slider>("MusicSlider");
            if (sfxSlider == null) sfxSlider = transform.FindDeep<Slider>("SfxSlider");
            if (ambientSlider == null) ambientSlider = transform.FindDeep<Slider>("AmbientSlider");
            if (sensitivitySlider == null) sensitivitySlider = transform.FindDeep<Slider>("SensitivitySlider");
            if (qualityDropdown == null) qualityDropdown = transform.FindDeep<Dropdown>("QualityDropdown");
            if (firstPersonToggle == null) firstPersonToggle = transform.FindDeep<Toggle>("FirstPersonToggle");
            if (fpsToggle == null) fpsToggle = transform.FindDeep<Toggle>("FpsToggle");
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null)
            {
                return;
            }

            SetSlider(musicSlider, save.Data.musicVolume, value =>
            {
                save.Data.musicVolume = value;
                AudioManager.Instance?.SetMusicVolume(value);
            });
            SetSlider(sfxSlider, save.Data.sfxVolume, value =>
            {
                save.Data.sfxVolume = value;
                AudioManager.Instance?.SetSfxVolume(value);
            });
            SetSlider(ambientSlider, save.Data.ambientVolume, value =>
            {
                save.Data.ambientVolume = value;
                AudioManager.Instance?.SetAmbientVolume(value);
            });
            SetSlider(sensitivitySlider, Mathf.InverseLerp(0.5f, 2f, save.Data.inputSensitivity), value =>
            {
                float sensitivity = Mathf.Lerp(0.5f, 2f, value);
                save.Data.inputSensitivity = sensitivity;
                SwipeInputManager.Instance?.SetSensitivity(sensitivity);
            });

            if (qualityDropdown != null)
            {
                qualityDropdown.value = Mathf.Clamp(save.Data.graphicsQuality, 0, qualityDropdown.options.Count - 1);
                qualityDropdown.onValueChanged.AddListener(index =>
                {
                    save.Data.graphicsQuality = index;
                    ApplyQuality(index);
                    save.MarkDirty();
                });
            }

            if (firstPersonToggle != null)
            {
                firstPersonToggle.isOn = save.Data.firstPersonCamera;
                firstPersonToggle.onValueChanged.AddListener(enabled =>
                {
                    save.Data.firstPersonCamera = enabled;
                    FindFirstObjectByType<FollowCamera>()?.SetFirstPerson(enabled);
                    save.MarkDirty();
                });
            }

            if (fpsToggle != null)
            {
                fpsToggle.isOn = save.Data.showFps;
                fpsToggle.onValueChanged.AddListener(enabled =>
                {
                    save.Data.showFps = enabled;
                    save.MarkDirty();
                });
            }
        }

        private static void SetSlider(Slider slider, float value, System.Action<float> onChanged)
        {
            if (slider == null)
            {
                return;
            }
            slider.SetValueWithoutNotify(value);
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(v => onChanged(v));
            onChanged(value);
        }

        private static void ApplyQuality(int level)
        {
            // 0 Low, 1 Medium, 2 High - mapped to Unity quality levels when present.
            int unityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(unityLevel, true);
        }

        public void OnBack()
        {
            AudioManager.Instance?.PlayButton();
            SaveSystem.Instance?.Save();
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.State == GameState.Paused)
            {
                UIManager.Instance?.Show("PauseMenu");
            }
            else
            {
                UIManager.Instance?.Show("MainMenu");
            }
        }

        public void OnResetProgress()
        {
            AudioManager.Instance?.PlayButton();
            SaveSystem.Instance?.ResetAll();
            Refresh();
        }
    }
}
