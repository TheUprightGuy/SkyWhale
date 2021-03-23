using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    public void PlaySound(string _sound)
    {
        Audio.AudioManager.instance.PlaySound(_sound);
    }
}
