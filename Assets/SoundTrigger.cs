using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public bool onGrass = true;
    public void TriggerSound(string soundName)
    {
        if (onGrass && transform.parent.GetComponent<PlayerMovement>().playerState != PlayerMovement.PlayerStates.CLIMBING)
        {
            AudioManager.instance.PlaySound("GrassFootsteps");
            return;
        }
        AudioManager.instance.PlaySound(soundName);
    }
}
