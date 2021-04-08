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
            if (instance != null)
            {
                Debug.Log("More than one AudioManager in scene!");
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        #endregion

        //Inspector variables
        #region Inpector Variables

        [Header("Main Settings")] public float ambientSoundTimerMin = 30f;
        public float ambientSoundTimerMax = 50f;
        public float fadeDuration = 3f;

        [Header("Audio Sources Used To Create Sound Dictionary:")]
        public List<AudioSource> audioSources;

        public List<AudioClip> musicTracks;
        public List<GameObject> ambientLayers;
        [Header("Audio Mixer")] public AudioMixer audioMixer;

        #endregion

        //Public variables
        #region Public variables

        public Dictionary<string, SoundInfo> soundDictionary;
        [HideInInspector] public bool randomlyCycleMusic = true;
        [HideInInspector] public bool playAmbientSounds = true;

        #endregion

        //Private variables
        #region Private variables

        private AudioSource _musicSource;
        private int _currentTrackIndex;
        private List<string> _soundsUnrestricted;

        //Audio source will play these sounds regardless of if whether they are already playing
        private float _sfxVolume = 1.0f;
        private Slider _slider;

        #endregion

        //Public member functions
        #region Public member functions

        public void PlaySound(string soundName)
        {
            var sound = soundDictionary[soundName];
            sound.Reset();
            PlaySound(sound.audioSource);
        }

        public void StopSound(string soundName)
        {
            soundDictionary[soundName].audioSource.Stop();
        }

        public void AddAudioSourceToDictionary(AudioSource audioSource)
        {
            if (soundDictionary.ContainsKey(audioSource.name))
            {
                Debug.Log("Sound dictionary already contains an audio source with key: " + audioSource.name);
                return;
            }

            var soundName = audioSource.name;
            var soundInfo = new SoundInfo();
            soundInfo.InitialiseSound(soundName, audioSource);
            soundDictionary.Add(soundName, soundInfo);
        }

        public void OnVolumeAdjusted(int mixerNumber, float value)
        {
            var newVolume = (value * 4 / 100 - 4.0f) * 10.0f;
            if (Math.Abs(newVolume - -40f) < .01f) newVolume = -80f;
            switch (mixerNumber)
            {
                case 0:
                    audioMixer.SetFloat("MasterVolume", newVolume);
                    break;
                case 1:
                    audioMixer.SetFloat("MusicVolume", newVolume);
                    break;
                case 2:
                    _sfxVolume = value / 100f;
                    audioMixer.SetFloat("SfxVolume", newVolume);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(mixerNumber), mixerNumber, null);
            }
        }

        public void Register(AudioSource audioSource)
        {
            AddAudioSourceToDictionary(audioSource);
        }

        public void SwitchMusicTrack(string trackName)
        {
            foreach (var track in musicTracks.Where(track => track.name == trackName))
            {
                _musicSource.clip = track;
                _musicSource.Play();
            }
        }

        public bool IsSoundPlaying(string soundName)
        {
            return soundDictionary[soundName].audioSource.isPlaying;
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        #endregion

        //Private member functions
        #region Private member functions
        private void OnAwake()
        {
            InitialisePrivateVariables();
            _musicSource.loop = false;
            //Start recursive coroutine for changing the music
            StartCoroutine(PlayRandomMusicTracks());
            foreach (var layer in ambientLayers)
            {
                var ambientLayer = layer.GetComponent<AmbientLayer>();
                var audioSource = layer.GetComponent<AudioSource>();
                StartCoroutine(PlayRandomAmbientTracks(ambientLayer, audioSource));
            }
        }

        private void InitialisePrivateVariables()
        {
            _musicSource = GetComponent<AudioSource>();
            soundDictionary = new Dictionary<string, SoundInfo>();
            _soundsUnrestricted = new List<string> { "Click", "Click_Progress" };
            
            foreach (var audioSource in audioSources) 
                AddAudioSourceToDictionary(audioSource);

            //AddInitiallyDisabledAudioSources();
        }

        private void AddInitiallyDisabledAudioSources()
        {
            //Object finder only works in editor
            //Need to use replacement functionality if needed
            /*var initiallyDisabledAudioSources = ObjectFinder.FindAllObjectsWithTag("InitiallyDisabledAudioSources");
            foreach (var initiallyDisabledAudioSource in initiallyDisabledAudioSources)
                AddAudioSourceToDictionary(initiallyDisabledAudioSource.GetComponent<AudioSource>());*/
        }

        private IEnumerator PlayRandomMusicTracks()
        {
            //_musicSource = gameObject.GetComponent<AudioSource>(); //Update cached audio source
            _musicSource.Stop();
            var randomIndex = Random.Range(0, musicTracks.Count);
            _musicSource.clip = musicTracks[randomIndex];
            _musicSource.Play();
            yield return new WaitForSeconds(_musicSource.clip.length);
            if (randomlyCycleMusic) StartCoroutine(PlayRandomMusicTracks());
        }

        private IEnumerator PlayRandomAmbientTracks(AmbientLayer ambientLayer, AudioSource audioSource)
        {
            yield return FadeAmbientTracks(ambientLayer.exposedParamName, 0f);
            RandomiseAudioClip(ambientLayer, audioSource);
            yield return FadeAmbientTracks(ambientLayer.exposedParamName, ambientLayer.soundInfo.volumeDefault);
            yield return new WaitForSeconds(Random.Range(ambientSoundTimerMin, ambientSoundTimerMax));
            if (playAmbientSounds) StartCoroutine(PlayRandomAmbientTracks(ambientLayer, audioSource));
        }

        private object FadeAmbientTracks(string exposedParameter, float targetVolume)
        {
            //Fade ambient tracks to target volume
            StartCoroutine(FadeMixerGroup.StartFade(audioMixer, exposedParameter, fadeDuration, targetVolume));
            return new WaitForSeconds(fadeDuration);
        }

        private static void RandomiseAudioClip(AmbientLayer ambientLayer, AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.clip = ambientLayer.RandomAudioClip();
            audioSource.Play();
        }

        private void AdjustPitchAndVolume(AudioSource audioSource)
        {
            audioSource.pitch *=
                Random.value * 0.5f + 0.75f; //Pitch is default multiplied by random value between 0.75 and 1.25
            audioSource.volume *= _sfxVolume;
        }

        private void PlaySound(AudioSource audioSource) //Only play sound if it's not already playing
        {
            AdjustPitchAndVolume(audioSource);
            if (!audioSource.isPlaying || _soundsUnrestricted.Contains(audioSource.name)) audioSource.Play();
        }

        private IEnumerator PlaySoundsInSequence(IEnumerable<string> soundNamesInOrder)
        {
            foreach (var currentSound in soundNamesInOrder)
            {
                PlaySound(currentSound);
                while (soundDictionary[currentSound].audioSource.isPlaying) yield return new WaitForSeconds(0.05f);
            }

            yield return null;
        }
        #endregion
    }
}