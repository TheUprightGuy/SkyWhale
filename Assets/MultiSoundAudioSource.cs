using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MultiSoundAudioSource : MonoBehaviour
{
    public List<AudioClip> sounds;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void ChangeSound()
    {
        Invoke(nameof(ChangeSoundPrivate), .75f);
    }

    public void ChangeSoundPrivate()
    {
        _audioSource.clip = RandomAudioClip();
    }
    
    public AudioClip RandomAudioClip()
    {
        var length = sounds.Count;
        var randomIndex = Random.Range(0, length);
        return sounds[randomIndex];
    }
    
}
