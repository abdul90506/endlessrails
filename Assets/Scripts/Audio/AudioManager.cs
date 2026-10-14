using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace EndlessRails
{
    /// <summary>
    /// Central audio service.
    ///
    /// Uses three "bus" AudioSources (music / sfx / ambient) whose volumes are
    /// driven by the settings screen, plus a pool of 3D one-shot sources for
    /// spatial playback (coins, footsteps, trains). Optional AudioMixer hookup:
    /// assign a mixer in the inspector and the buses route through mixer groups
    /// named "Music", "SFX", "Ambient" instead of direct volume control.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Data")]
        [SerializeField] private AudioLibrary library;

        [Header("Optional Mixer Routing")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup ambientMixerGroup;

        [Header("Bus Sources (auto-created if empty)")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambientSource;
        [SerializeField] private AudioSource rainSource;

        [Header("Pitch Randomization")]
        [SerializeField] private float pitchJitter = 0.06f;

        public AudioLibrary Library => library;

        // 3D one-shot pool.
        private readonly List<AudioSource> _sfxPool = new List<AudioSource>(24);
        private const int SfxPoolSize = 24;
        private int _sfxCursor;

        // Bus volumes (settings).
        public float MusicVolume { get; private set; } = 0.8f;
        public float SfxVolume { get; private set; } = 1f;
        public float AmbientVolume { get; private set; } = 0.7f;

        protected override void OnSingletonAwake()
        {
            EnsureBusSource(ref musicSource, "MusicSource", loop: true);
            EnsureBusSource(ref ambientSource, "AmbientSource", loop: true);
            EnsureBusSource(ref rainSource, "RainSource", loop: true);

            if (musicMixerGroup != null) musicSource.outputAudioMixerGroup = musicMixerGroup;
            if (sfxMixerGroup != null) { /* assigned per one-shot below */ }
            if (ambientMixerGroup != null)
            {
                ambientSource.outputAudioMixerGroup = ambientMixerGroup;
                rainSource.outputAudioMixerGroup = ambientMixerGroup;
            }

            for (int i = 0; i < SfxPoolSize; i++)
            {
                GameObject sourceObject = new GameObject($"SfxSource_{i}");
                sourceObject.transform.SetParent(transform, false);
                AudioSource source = sourceObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 1f;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.maxDistance = 55f;
                if (sfxMixerGroup != null)
                {
                    source.outputAudioMixerGroup = sfxMixerGroup;
                }
                _sfxPool.Add(source);
            }
        }

        private void EnsureBusSource(ref AudioSource source, string name, bool loop)
        {
            if (source != null)
            {
                source.loop = loop;
                return;
            }
            GameObject go = new GameObject(name);
            go.transform.SetParent(transform, false);
            source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
        }

        private void Start()
        {
            StartAmbience();
        }

        // ------------------------------------------------------------------
        // Volume control (settings screen)
        // ------------------------------------------------------------------

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = MusicVolume;
            }
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolume = Mathf.Clamp01(volume);
        }

        public void SetAmbientVolume(float volume)
        {
            AmbientVolume = Mathf.Clamp01(volume);
            if (ambientSource != null)
            {
                ambientSource.volume = AmbientVolume;
            }
            if (rainSource != null)
            {
                rainSource.volume = AmbientVolume;
            }
        }

        // ------------------------------------------------------------------
        // Ambience + music
        // ------------------------------------------------------------------

        public void StartAmbience()
        {
            if (library == null)
            {
                return;
            }
            if (library.cityAmbient != null && ambientSource != null)
            {
                ambientSource.clip = library.cityAmbient;
                ambientSource.volume = AmbientVolume;
                ambientSource.spatialBlend = 0f;
                ambientSource.Play();
            }
            if (library.musicLoop != null && musicSource != null)
            {
                musicSource.clip = library.musicLoop;
                musicSource.volume = MusicVolume;
                musicSource.spatialBlend = 0f;
                musicSource.Play();
            }
        }

        public void SetRainActive(bool active)
        {
            if (rainSource == null || library == null || library.rainAmbient == null)
            {
                return;
            }
            if (active)
            {
                if (!rainSource.isPlaying)
                {
                    rainSource.clip = library.rainAmbient;
                    rainSource.volume = AmbientVolume * 0.9f;
                    rainSource.Play();
                }
            }
            else if (rainSource.isPlaying)
            {
                rainSource.Stop();
            }
        }

        // ------------------------------------------------------------------
        // Named one-shot helpers
        // ------------------------------------------------------------------

        public void PlayFootstep(SurfaceType surface, Vector3 position, float volumeScale, bool alternate)
        {
            if (library == null)
            {
                return;
            }
            AudioClip[] steps = library.GetFootsteps(surface);
            if (steps == null || steps.Length == 0)
            {
                return;
            }
            AudioClip clip = steps[(alternate ? 1 : 0) % steps.Length];
            PlayClipAt(clip, position, 0.8f * volumeScale, 1f + (alternate ? pitchJitter : -pitchJitter));
        }

        public void PlayCoin(Vector3 position)
        {
            if (library == null)
            {
                return;
            }
            bool multi = Random.value < 0.12f;
            AudioClip clip = multi && library.coinMulti != null ? library.coinMulti : library.coinPickup;
            PlayClipAt(clip, position, 0.85f, 1f + Random.Range(-pitchJitter, pitchJitter));
        }

        public void PlayPowerup(Vector3 position)
        {
            if (library == null)
            {
                return;
            }
            PlayClipAt(library.powerup, position, 1f);
        }

        public void PlayPowerdown(Vector3 position)
        {
            if (library == null)
            {
                return;
            }
            PlayClipAt(library.powerdown, position, 0.9f);
        }

        public void PlayCrash()
        {
            if (library == null)
            {
                return;
            }
            PlayClipAt(library.crash, Vector3.zero, 1f, 1f, spatial: false);
        }

        public void PlayStumble()
        {
            if (library == null)
            {
                return;
            }
            PlayClipAt(library.stumble, Vector3.zero, 0.9f, 1f, spatial: false);
        }

        public void PlayJump(Vector3 position)
        {
            if (library == null)
            {
                return;
            }
            PlayClipAt(library.jumpWhoosh, position, 0.7f);
        }

        public void PlayLand(Vector3 position, float impact)
        {
            if (library == null)
            {
                return;
            }
            PlayClipAt(library.landThud, position, Mathf.Clamp01(0.4f + impact * 0.6f), 1f + Random.Range(-0.05f, 0.05f));
        }

        public void PlaySlide(Vector3 position)
        {
            if (library == null)
            {
                return;
            }
            PlayClipAt(library.slideScrape, position, 0.8f, 1f, spatial: false);
        }

        public void PlayLaneChange()
        {
            if (library == null || library.laneWhoosh == null)
            {
                return;
            }
            PlayClipAt(library.laneWhoosh, Vector3.zero, 0.45f, 1f + Random.Range(-0.08f, 0.08f), spatial: false);
        }

        public void PlayUIShieldBreak()
        {
            if (library == null)
            {
                return;
            }
            PlayClipAt(library.shieldBreak, Vector3.zero, 1f, 1f, spatial: false);
        }

        public void PlayChaserShout(Vector3 position)
        {
            if (library == null || library.chaserShout == null)
            {
                return;
            }
            PlayClipAt(library.chaserShout, position, 1f, 1f + Random.Range(-0.06f, 0.1f));
        }

        public void PlayButton()
        {
            if (library == null || library.buttonClick == null)
            {
                return;
            }
            PlayClipAt(library.buttonClick, Vector3.zero, 0.8f, 1f, spatial: false);
        }

        public void PlayUISwoosh()
        {
            if (library == null || library.uiSwoosh == null)
            {
                return;
            }
            PlayClipAt(library.uiSwoosh, Vector3.zero, 0.7f, 1f, spatial: false);
        }

        public void PlayGameOverSting()
        {
            if (library == null || library.gameOverSting == null)
            {
                return;
            }
            PlayClipAt(library.gameOverSting, Vector3.zero, 1f, 1f, spatial: false);
        }

        // ------------------------------------------------------------------
        // Core playback
        // ------------------------------------------------------------------

        /// <summary>Plays a one-shot clip on a pooled 3D source. No allocation.</summary>
        public void PlayClipAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, bool spatial = true)
        {
            if (clip == null || _sfxPool.Count == 0)
            {
                return;
            }

            AudioSource source = _sfxPool[_sfxCursor];
            _sfxCursor = (_sfxCursor + 1) % _sfxPool.Count;

            source.transform.position = position;
            source.clip = clip;
            source.volume = volume * SfxVolume;
            source.pitch = pitch;
            source.spatialBlend = spatial ? 1f : 0f;
            source.loop = false;
            source.Play();
        }
    }
}
