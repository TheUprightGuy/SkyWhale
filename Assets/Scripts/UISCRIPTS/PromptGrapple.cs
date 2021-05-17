using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptGrapple : MonoBehaviour
{
    PlayerMovement pm;
    public NPCScript npc;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();

            if (npc)
            {
                npc.pm = pm;
                npc.Interact(InputState.KEYDOWN);
            }

            if (pm.GetComponent<GrappleScript>().enabled)
            {
                CallbackHandler.instance.DisplayPrompt(PromptType.GrappleAim);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() == pm)
        {
            CallbackHandler.instance.HidePrompt(PromptType.GrappleAim);
        }
    }
}
