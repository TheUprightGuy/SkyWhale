using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotQuestCollectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerMovement>()) return;
        Debug.Log("Collected Parts");
        CallbackHandler.instance.GliderPartsCollected();
        Destroy(gameObject);
    }
}
