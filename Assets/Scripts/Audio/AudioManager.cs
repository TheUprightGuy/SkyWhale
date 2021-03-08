using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        public static AudioManager instance;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (instance != null)
            {
                Debug.Log("More than one AudioManager in scene!");
                Destroy(this.gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(this.gameObject);
            
            
            OnAwake();
        }
        #endregion
        [Header("Main Settings")]
        public float masterVolume = 1.0f;
        public float defaultVolume;
        public bool randomlyCycleMusic;
        public bool playAmbientSounds = true;
        public float ambientSoundTimerMin = 30f;
        public float ambientSoundTimerMax = 50f;
        public float fadeDuration = 3f;
        
        [Header("Audio Sources Used To Create Sound Dictionary:")]
        public List<AudioSource> audioSources;
        public List<AudioClip> musicTracks;
        public List<GameObject> ambientLayers;

        [Header("Audio Mixer")] public AudioMixer audioMixer;

        public Dictionary<string, SoundInfo> soundDictionary;
        private List<string> _soundsUnrestricted;    //Audio source will play this sound regardless of if it's already playing
        [SerializeField] private GameObject volumeSlider = null;
        private Slider _slider;
        private AudioSource _musicSource;
        private float _musicDefaultVolume;
        private AudioSource _audioSource;
        private int _currentTrackIndex;

        private void OnAwake()
        {
            InitialisePrivateVariables();
            foreach (var audioSource in audioSources)
            {
                AddAudioSourceToDictionary(audioSource);
            }

            _soundsUnrestricted = new List<string> {};
            
            if (!randomlyCycleMusic) return;
            _audioSource.loop = false;
            //Start recursive coroutine for changing the music
            StartCoroutine(PlayRandomMusicTracks());
            foreach (var layer in ambientLayers)
            {
                var ambientLayer = layer.GetComponent<AmbientLayer>();
                var audioSource = layer.GetComponent<AudioSource>();
                StartCoroutine(PlayRandomAmbientTracks(ambientLayer, audioSource));
            }
            
            audioMixer.GetFloat("MasterVolume", out defaultVolume);
            defaultVolume = Mathf.Pow(10, defaultVolume / 20);
        }
        
        private void InitialisePrivateVariables()
        {
            soundDictionary = new Dictionary<string, SoundInfo>();
            if (volumeSlider != null) _slider = volumeSlider.GetComponent<Slider>();
            _musicSource = this.GetComponent<AudioSource>();
            _musicDefaultVolume = _musicSource.volume;
        }

        private void AddAudioSourceToDictionary(AudioSource audioSource)
        {
            var soundName = audioSource.name;
            var soundInfo = new SoundInfo();
            soundInfo.InitialiseSound(soundName);
            soundDictionary.Add(soundName, soundInfo);
        }

        private IEnumerator PlayRandomMusicTracks()
        {
            _audioSource = gameObject.GetComponent<AudioSource>(); //Update cached audio source
            //Stop old music track
            _audioSource.Stop();
            //Play music track
            var randomIndex = Random.Range(0, musicTracks.Count);
            _audioSource.clip = musicTracks[randomIndex];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            if(randomlyCycleMusic) StartCoroutine(PlayRandomMusicTracks());
        }
        
        private IEnumerator PlayRandomAmbientTracks(AmbientLayer ambientLayer, AudioSource audioSource)
        {
            yield return FadeAmbientTracks(ambientLayer.exposedParamName, 0f);
            RandomiseAudioClip(ambientLayer, audioSource);
            yield return FadeAmbientTracks(ambientLayer.exposedParamName, ambientLayer.soundInfo.VolumeDefault);
            yield return new WaitForSeconds(Random.Range(ambientSoundTimerMin, ambientSoundTimerMax));
            if(playAmbientSounds) StartCoroutine(PlayRandomAmbientTracks(ambientLayer, audioSource));
        }

        private static void RandomiseAudioClip(AmbientLayer ambientLayer, AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.clip = ambientLayer.RandomAudioClip();
            audioSource.Play();
        }

        private object FadeAmbientTracks(string exposedParameter, float targetVolume)
        {
            //Fade ambient tracks to target volume
            StartCoroutine(
                FadeMixerGroup.StartFade(audioMixer, exposedParameter, fadeDuration, targetVolume));
            return new WaitForSeconds(fadeDuration);
        }

        public void PlaySound(string soundName)
        {
            var sound = soundDictionary[soundName];
            sound.Reset();
            PlaySound(sound.AudioSource);
        }

        IEnumerator PlaySoundsInSequence(List<string> soundNamesInOrder)
        {
            foreach (var currentSound in soundNamesInOrder)
            {
                PlaySound(currentSound);
                while (soundDictionary[currentSound].AudioSource.isPlaying)
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
            yield return null;
        }

        public void StopSound(string soundName)
        {
            soundDictionary[soundName].AudioSource.Stop();
        }

        public void OnVolumeAdjusted()
        {
            masterVolume = _slider.value * 10;
            if (Math.Abs(masterVolume - (-40f)) < .01f) masterVolume = -80f;
            audioMixer.SetFloat("MasterVolume", masterVolume);
            PlaySound("crackle");
            /*_musicSource.volume = _musicDefaultVolume * masterVolume;
            foreach (var layer in ambientLayers)
            {
                layer.GetComponent<AmbientLayer>().soundInfo.Reset(masterVolume);
            }

            foreach (var sound in soundDictionary)
            {
                var curSoundInfo = sound.Value;
                var curAs = curSoundInfo.AudioSource;
                if (curAs.isPlaying) curAs.volume = curSoundInfo.VolumeDefault * masterVolume;
            }*/
        }
    

        private void PlaySound(AudioSource audioSource) //Only play sound if it's not already playing
        {
            AdjustPitchAndVolume(audioSource);
            if (!audioSource.isPlaying || _soundsUnrestricted.Contains(audioSource.name))
                audioSource.Play();
        }

        private void AdjustPitchAndVolume(AudioSource audioSource)
        {
            audioSource.pitch *= (Random.value * 0.5f + 0.75f); //Pitch is default multiplied by random value between 0.75 and 1.25
            audioSource.volume *= masterVolume;
        }

        public void SwitchMusicTrack(string trackName)
        {
            foreach (var track in musicTracks.Where(track => track.name == trackName))
            {
                _audioSource.clip = track;
                _audioSource.Play();
            }
        }

        public bool IsSoundPlaying(string soundName)
        {
            return soundDictionary[soundName].AudioSource.isPlaying;
        }

        public void StopMusic()
        {
            _audioSource.Stop();
        }

        public void Register(AudioSource audioSource)
        {
            AddAudioSourceToDictionary(audioSource);
        }
    }
}