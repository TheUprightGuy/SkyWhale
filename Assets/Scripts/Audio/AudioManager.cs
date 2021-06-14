// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   AudioManager.cs
// Description :   Handles most functionality relating to audio.
//                 Simplifies playing sounds from other scripts and storing corresponding sound info,
//                 handles randomising and playing the music tracks,
//                 handles integrating the ambient layers and fade mixers,
//                 and other audio related functionality.
// Date        :   XX/XX/2020    (First created for the Wolf and Sheep's clothing game and has been
//                                 altered and upgraded through different projects and prototypes since then)
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz

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

        public Dictionary<string, SoundInfo> SoundDictionary;
        [HideInInspector] public bool randomlyCycleMusic = true;
        [HideInInspector] public bool playAmbientSounds = true;
        public float[] targetValueMultiplier = new float[3];

        #endregion

        //Private variables
        #region Private variables

        private AudioSource _musicSource;
        private int _currentTrackIndex;
        private List<string> _soundsUnrestricted;    //Audio source will play these sounds regardless of if whether they are already playing
        private Slider _slider;
        private Coroutine _thunderRainAmbientLayerCoroutine;

        #endregion

        //Public member functions
        #region Public member functions

        /// <summary>
        /// This function uses the sound name
        /// to access the relevant sound info class from the sound dictionary. This then is reset in preparation for
        /// it to be played in the private play sound function that takes in the audio source.
        /// </summary>
        /// <param name="soundName">The name of the sound and it's corresponding audio source game object</param>
        public void PlaySound(string soundName)
        {
            if (!SoundDictionary.ContainsKey(soundName))
                return;

            var sound = SoundDictionary[soundName];
            sound.Reset();
            PlaySound(sound.audioSource);
            if(sound.multiSoundAudioSource) sound.audioSource.gameObject.GetComponent<MultiSoundAudioSource>().ChangeSound();
        }

        /// <summary>
        /// This function uses the sound name to access the appropriate audio source and calls it's stop function.
        /// </summary>
        /// <param name="soundName">The name of the sound and it's corresponding audio source game object</param>
        public void StopSound(string soundName)
        {
            SoundDictionary[soundName].audioSource.Stop();
        }

        /// <summary>
        /// This function is used to add an audio source to the sound dictionary so that scripts can utilize the play sound
        /// function to cleanly play the sound corresponding to the audio source from anywhere.
        /// </summary>
        /// <param name="audioSource">The audio source being added</param>
        /// <param name="id"></param>
        public void AddAudioSourceToDictionary(AudioSource audioSource, int id = 0)
        {
            string name = id != 0 ? id.ToString() : audioSource.name;
            if (SoundDictionary.ContainsKey(name))    //Ensure sound dictionary doesn't already contain the audio source
            {
                Debug.Log("Replacing sound dictionary reference for: " + name);
                var soundInfoToReplace = SoundDictionary[name];
                soundInfoToReplace.audioSource = audioSource;
                soundInfoToReplace.pitchDefault = audioSource.pitch;
                soundInfoToReplace.volumeDefault = audioSource.volume;
                soundInfoToReplace.multiSoundAudioSource =
                    audioSource.gameObject.GetComponent<MultiSoundAudioSource>() != null;
                return;
            }
            
            //Set up necessary information and references from the audio source
            var soundInfo = new SoundInfo();
            soundInfo.InitialiseSound(name, audioSource);
            soundInfo.multiSoundAudioSource =
                audioSource.gameObject.GetComponent<MultiSoundAudioSource>() != null;
            SoundDictionary.Add(name, soundInfo);
        }

        /// <summary>
        /// This function is called when the volume sliders are changed. It converts the slider value (0-100) to a value
        /// that matches unity's audio mixer values. It then updates the relevant exposed parameter to the new volume
        /// by using the mixerNumber to figure out the parameter name. 
        /// </summary>
        /// <param name="mixerNumber">The number corresponding to the relevant mixer. (0 for master, 1 for music and 2 for sfx)</param>
        /// <param name="value">The value from the audio slider that was altered</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an out of range mixer number is given</exception>
        public void OnVolumeAdjusted(int mixerNumber, float value)
        {
            //Calculate the new volume (which works with the audio mixer) from the menu slider (which ranges from 0-100)
            var newVolume = (value * 4 / 100 - 4.0f) * 10.0f;
            if (Math.Abs(newVolume - -40f) < .01f) newVolume = -80f;    //Completely mute the volume if it's zero
            switch (mixerNumber)    //Find the appropriate audio mixer parameter which is being updated.
            {
                case 0:
                    audioMixer.SetFloat("MasterVolume", newVolume);
                    break;
                case 1:
                    audioMixer.SetFloat("MusicVolume", newVolume);
                    break;
                case 2:
                    audioMixer.SetFloat("SfxVolume", newVolume);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(mixerNumber), mixerNumber, null);
            }
        }

        /// <summary>
        /// Starts the fade coroutine which plays the whale ambient layer
        /// </summary>
        public void PlayWhaleAmbientLayer()
        {
            StartCoroutine(PlayRandomAmbientTracks(
                ambientLayers[0].GetComponent<AmbientLayer>(), 
                ambientLayers[0].GetComponent<AudioSource>(), 0));
        }

        private bool rainLayerStarted = false;
        /// <summary>
        /// Starts the fade coroutine which plays the whale ambient layer
        /// </summary>
        public void PlayRainThunderAmbientLayer(bool toggle)
        {
            if (!toggle)
            {
                if(!rainLayerStarted) return;
                StopRainThunderAmbientLayer();
                return;
            }

            if (rainLayerStarted)
            {
                ambientLayers[2].GetComponent<AudioSource>().Play();
                return;
            }
            StartCoroutine(PlayRandomAmbientTracks(
                ambientLayers[2].GetComponent<AmbientLayer>(), 
                ambientLayers[2].GetComponent<AudioSource>(), 2));
            rainLayerStarted = true;
        }

        public void StopRainThunderAmbientLayer()
        {
            //Most likely very abrupt
            ambientLayers[2].GetComponent<AudioSource>().Stop();
        }

        /// <summary>
        /// This function allows you to switch the currently playing music track if the music tracks aren't being randomised.
        /// </summary>
        /// <param name="trackName">Name of the track to change to</param>
        public void SwitchMusicTrack(string trackName)
        {
            foreach (var track in musicTracks.Where(track => track.name == trackName))
            {
                _musicSource.clip = track;
                _musicSource.Play();
            }
        }

        /// <summary>
        /// Function returns if the audio source of the relevant sound is currently played.
        /// </summary>
        /// <param name="soundName">Name of the sound to check</param>
        /// <returns>Whether or not the audio source is currently playing</returns>
        public bool IsSoundPlaying(string soundName)
        {
            return SoundDictionary[soundName].audioSource.isPlaying;
        }

        /// <summary>
        /// Stop the currently playing music source
        /// </summary>
        public void StopMusic()
        {
            _musicSource.Stop();
        }

        #endregion

        //Private member functions
        #region Private member functions
        /// <summary>
        /// Function is called on awake. Starts all the relevant initial functionality.
        /// </summary>
        private void OnAwake()
        {
            InitialisePrivateVariables();
            _musicSource.loop = false;
            //Start recursive coroutine for changing the music
            StartCoroutine(PlayRandomMusicTracks());
            StartCoroutine(
                PlayRandomAmbientTracks(ambientLayers[1].GetComponent<AmbientLayer>(), 
                    ambientLayers[1].GetComponent<AudioSource>(), 1));
            /*foreach (var layer in ambientLayers)
            {
                var ambientLayer = layer.GetComponent<AmbientLayer>();
                var audioSource = layer.GetComponent<AudioSource>();
                StartCoroutine(PlayRandomAmbientTracks(ambientLayer, audioSource));
            }*/
        }

        private void Start()
        {
            CallbackHandler.instance.toggleRain += PlayRainThunderAmbientLayer;
        }

        /// <summary>
        /// Initialises all the audio managers private variables. (Most importantly all the audio sources added through the inspector).
        /// </summary>
        private void InitialisePrivateVariables()
        {
            _musicSource = GetComponent<AudioSource>();
            SoundDictionary = new Dictionary<string, SoundInfo>();
            _soundsUnrestricted = new List<string> { "Click", "Click_Progress", "Switch", "Footsteps" };
            
            foreach (var audioSource in audioSources) 
                AddAudioSourceToDictionary(audioSource);

            targetValueMultiplier = new[] {1f, 1f, 1f};
            //AddInitiallyDisabledAudioSources();
        }

        /// <summary>
        /// Function that adds initially disabled audio sources to the sound dictionary.
        /// This is currently disabled as it only works in editor. (Object finder only works in editor).
        /// This will be replaced with a function called on awake that copies over any inspector assigned audio sources
        /// from new versions of the audio manager singleton before they are destroyed.
        /// (So that inspector assigned audio sources don't get lost due to scene transitions.)
        /// </summary>
        private void AddInitiallyDisabledAudioSources()
        {
            //Object finder only works in editor
            //Need to use replacement functionality if needed
            /*var initiallyDisabledAudioSources = ObjectFinder.FindAllObjectsWithTag("InitiallyDisabledAudioSources");
            foreach (var initiallyDisabledAudioSource in initiallyDisabledAudioSources)
                AddAudioSourceToDictionary(initiallyDisabledAudioSource.GetComponent<AudioSource>());*/
        }

        /// <summary>
        /// Co-routine function that plays a random music track for it's entire length before recursively calling itself.
        /// </summary>
        /// <returns>For the full track duration</returns>
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

        /// <summary>
        /// Fades the currently played ambient sound to zero volume before choosing a new ambient sound
        /// and fading that in to it's default volume. After a random duration within a predefined range has passed
        /// the coroutine is started again.
        /// </summary>
        /// <param name="ambientLayer">The ambient layer containing the tracks being faded</param>
        /// <param name="audioSource">The audio source the ambient layer is played through</param>
        /// <returns>After the random (within range) duration</returns>
        private IEnumerator PlayRandomAmbientTracks(AmbientLayer ambientLayer, AudioSource audioSource, int index)
        {
            yield return FadeAmbientTracks(ambientLayer.exposedParamName, 0f, index);
            RandomiseAudioClip(ambientLayer, audioSource);
            yield return FadeAmbientTracks(ambientLayer.exposedParamName, ambientLayer.soundInfo.volumeDefault, index);
            yield return new WaitForSeconds(Random.Range(ambientSoundTimerMin, ambientSoundTimerMax));
            if (playAmbientSounds) StartCoroutine(PlayRandomAmbientTracks(ambientLayer, audioSource, index));
        }

        /// <summary>
        /// Starts fading the exposed parameter to the target volume over the fade duration and returns once this has been completed.
        /// </summary>
        /// <param name="exposedParameter">The name of the exposed parameter (corresponding to an audio mixer group's volume) to fade</param>
        /// <param name="targetVolume">The target volume to fade to</param>
        /// <returns>After the fade duration</returns>
        private object FadeAmbientTracks(string exposedParameter, float targetVolume, int index)
        {
            //Fade ambient tracks to target volume
            StartCoroutine(FadeMixerGroup.StartFade(audioMixer, exposedParameter, fadeDuration, targetVolume, index));
            return new WaitForSeconds(fadeDuration);
        }

        /// <summary>
        /// Randomise the audio clip for the given ambient layer's audio source. (From the ambient layers list of audio clips).
        /// </summary>
        /// <param name="ambientLayer">The ambient layer to randomise the audio clip for</param>
        /// <param name="audioSource">The ambient layer's corresponding audio source</param>
        private static void RandomiseAudioClip(AmbientLayer ambientLayer, AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.clip = ambientLayer.RandomAudioClip();
            audioSource.Play();
        }

        /// <summary>
        /// Function slightly randomises the audio sources pitch for sound variation
        /// </summary>
        /// <param name="audioSource">The audio source being altered</param>
        private static void RandomisePitch(AudioSource audioSource)
        {
            audioSource.pitch *=
                Random.value * 0.5f + 0.75f; //Pitch is default multiplied by random value between 0.75 and 1.25
        }

        /// <summary>
        /// Play the sound connected to the audio source if not already playing or if within the unrestricted list.
        /// Also call the randomise pitch function to avoid repetitiveness
        /// </summary>
        /// <param name="audioSource">The audio source with the sound being played</param>
        private void PlaySound(AudioSource audioSource) //Only play sound if it's not already playing
        {
            RandomisePitch(audioSource);
            if (!audioSource.isPlaying || _soundsUnrestricted.Contains(audioSource.name))
            {
                audioSource.Play();
            }
        }

        /// <summary>
        /// Play each sound in the list sequentially and wait for each to complete before playing the next.
        /// </summary>
        /// <param name="soundNamesInOrder">The list of sounds to play</param>
        /// <returns>When all sounds in the list have been played</returns>
        private IEnumerator PlaySoundsInSequence(IEnumerable<string> soundNamesInOrder)
        {
            foreach (var currentSound in soundNamesInOrder)
            {
                PlaySound(currentSound);
                while (SoundDictionary[currentSound].audioSource.isPlaying) yield return new WaitForSeconds(0.05f);
            }

            yield return null;
        }
        #endregion
    }
}