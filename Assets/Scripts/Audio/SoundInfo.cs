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
        public AudioSource AudioSource;
        public float PitchDefault;
        public float VolumeDefault;
        #endregion

        //Public functions
        #region Public functions
        public void InitialiseSound(string name, AudioSource _source)
        {
            AudioSource = _source;// ObjectFinder.FindObject(name).GetComponent<AudioSource>();

            PitchDefault = AudioSource.pitch;
            VolumeDefault = AudioSource.volume;
        }

        public void InitialiseSound()
        {
            PitchDefault = AudioSource.pitch;
            VolumeDefault = AudioSource.volume;
        }

        public void Reset()
        {
            //Reset altered pitch and volume to defaults
            AudioSource.pitch = PitchDefault;
            AudioSource.volume = VolumeDefault;
        }

        public void Reset(float newVolume)    //Resets audio source to default before adjusting it by the new volume
        {
            //Reset altered pitch and volume to defaults
            AudioSource.pitch = PitchDefault;
            AudioSource.volume = VolumeDefault * newVolume;
        }
        #endregion
    }
}