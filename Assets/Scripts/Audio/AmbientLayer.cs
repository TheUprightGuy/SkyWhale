//not using template for header comment
//Script owner: Jacob Gallagher
//Description: This mono behaviour script stores a list of ambient sounds that are faded in and out by
//the audio manager to create a looping ambient track. It also stores the name of the corresponding
//audio mixer exposed parameter (which enables the audio manager to alter the volume).
//It also handles creating/storing the corresponding sound info

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
            soundInfo = new SoundInfo { audioSource = GetComponent<AudioSource>() };
            soundInfo.InitialiseSound();
        }
        #endregion
    }
}
