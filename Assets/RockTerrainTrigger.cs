using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTerrainTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SoundTrigger soundTrigger = other.gameObject.GetComponentInChildren<SoundTrigger>();
        if (soundTrigger)
        {
            soundTrigger.onGrass = false;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        SoundTrigger soundTrigger = other.gameObject.GetComponentInChildren<SoundTrigger>();
        if (soundTrigger)
        {
            soundTrigger.onGrass = true;
        }
    }
}
