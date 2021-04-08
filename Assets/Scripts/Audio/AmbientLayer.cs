// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   AmbientLayer.cs
// Description :   Contains ambient sound list for a specific ambient sound layer.
//                 Contains string for audio mixers exposed volume parameter name.
//                 (This is used to fade in and out each sound before choosing another random sound).
// Date        :   XX/XX/2020
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Audio
{
    public class AmbientLayer : MonoBehaviour
    {
        //Inspector variables
        #region Inpector Variables
        public List<AudioClip> ambientSounds;
        public string exposedParamName;
        #endregion

        //Public variables
        public SoundInfo soundInfo;

        //Public member functions
        #region Public member functions
        /// <summary>
        /// Returns a random ambient sound. (Intended for the audio manager to play while fading in/out the volume over the sounds duration.)
        /// </summary>
        public AudioClip RandomAudioClip()
        {
            var length = ambientSounds.Count;
            var randomIndex = Random.Range(0, length);
            return ambientSounds[randomIndex];
        }
        #endregion

        //Private member functions
        #region Private member functions
        /// <summary>
        /// Corresponding sound info class is created and initialized on awake
        /// </summary>
        private void Awake()
        {
            soundInfo = new SoundInfo { AudioSource = GetComponent<AudioSource>() };
            soundInfo.InitialiseSound();
        }
        #endregion
    }
}
