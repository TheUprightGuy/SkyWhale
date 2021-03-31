using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class AudioSourceInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.AddAudioSourceToDictionary(gameObject.GetComponent<AudioSource>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
