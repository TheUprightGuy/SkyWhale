using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using Random = UnityEngine.Random;

public class AmbientLayer : MonoBehaviour
{
    public List<AudioClip> ambientSounds;
    public SoundInfo soundInfo;
    public string exposedParamName;

    private void Awake()
    {
        soundInfo = new SoundInfo {AudioSource = GetComponent<AudioSource>()};
        soundInfo.InitialiseSound();
    }

    public AudioClip RandomAudioClip()
    {
        var length = ambientSounds.Count;
        var randomIndex = Random.Range(0, length);
        return ambientSounds[randomIndex];
    }
}
