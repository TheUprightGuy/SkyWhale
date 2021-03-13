using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    public void PlaySound(AudioClip _sound)
    {
        PauseMenuCanvasController.instance.audioSource.PlayOneShot(_sound);
    }
}
