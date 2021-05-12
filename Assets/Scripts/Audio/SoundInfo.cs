// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   SoundInfo.cs
// Description :   Class that contains all the relevant variables for a sound. This is used by the audio manager.
//                 It also contains functions to initialise/reset this information.
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz

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