using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class PilotQuestCollectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerMovement>()) return;
        AudioManager.instance.PlaySound("Collect");
        EventManager.TriggerEvent("FindParts");
        CallbackHandler.instance.GliderPartsCollected();
        GetComponentInParent<Puzzle.PuzzleGenerator>().disabled = true;
        Destroy(gameObject);
    }
}
