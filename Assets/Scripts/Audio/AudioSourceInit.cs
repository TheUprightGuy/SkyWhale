using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class AudioSourceInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.AddAudioSourceToDictionary(gameObject.GetComponent<AudioSource>());
    }
}
