//Script owner: Jacob Gallagher
//Description: This script stores the audio source and the default pitch and volume values for a specific sound in
//the audio managers sound dictionary.
//It handles resetting the audio source's volume and pitch to their default volumes

using UnityEngine;

namespace Audio
{
    public class SoundInfo
    {
        //Public variables
        #region Public variables
        public AudioSource audioSource;
        public float pitchDefault;
        public float volumeDefault;
        #endregion

        //Public functions
        #region Public functions
        public void InitialiseSound(string name, AudioSource _source)
        {
            audioSource = _source;// ObjectFinder.FindObject(name).GetComponent<AudioSource>();

            pitchDefault = audioSource.pitch;
            volumeDefault = audioSource.volume;
        }

        public void InitialiseSound()
        {
            pitchDefault = audioSource.pitch;
            volumeDefault = audioSource.volume;
        }

        public void Reset()
        {
            //Reset altered pitch and volume to defaults
            audioSource.pitch = pitchDefault;
            audioSource.volume = volumeDefault;
        }

        public void Reset(float newVolume)    //Resets audio source to default before adjusting it by the new volume
        {
            //Reset altered pitch and volume to defaults
            audioSource.pitch = pitchDefault;
            audioSource.volume = volumeDefault * newVolume;
        }
        #endregion
    }
}