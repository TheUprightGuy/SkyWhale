using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class InactiveAudiosource : MonoBehaviour
{
    //This script checks/ensures the audio source is registered with the audiomanager

    // Start is called before the first frame update
    void OnEnable()
    {
        Debug.Log("Registering audio source");
        var audioSource = this.GetComponent<AudioSource>();
        if(!AudioManager.instance.soundDictionary.ContainsKey(audioSource.name))
        {
            AudioManager.instance.Register(audioSource);
        }
    }
}
