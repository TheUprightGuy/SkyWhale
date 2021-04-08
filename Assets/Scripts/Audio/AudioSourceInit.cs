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
        // Start is called before the first frame update
        private void Start()
        {
            AudioManager.instance.AddAudioSourceToDictionary(gameObject.GetComponent<AudioSource>());
        }
    }
}
