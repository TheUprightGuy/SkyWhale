// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   AudioSourceInit.cs
// Description :   Registers the audio source on the game object with the audio manager on start
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz

using UnityEngine;

namespace Audio
{
    public class AudioSourceInit : MonoBehaviour
    {
        public bool useInstanceIdForSoundName = false;
        // Start is called before the first frame update
        private void Start()
        {
            if (useInstanceIdForSoundName)
            {
                AudioManager.instance.AddAudioSourceToDictionary(gameObject.GetComponent<AudioSource>(), gameObject.GetInstanceID());
                return;
            }

            AudioManager.instance.AddAudioSourceToDictionary(gameObject.GetComponent<AudioSource>());
        }
    }
}
