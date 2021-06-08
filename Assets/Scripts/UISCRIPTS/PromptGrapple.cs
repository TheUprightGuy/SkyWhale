using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptGrapple : MonoBehaviour
{
    PlayerMovement pm;
    //public NPCScript npc;
    //bool first;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();

            /*if (npc && !first)
            {
                npc.pm = pm;
                npc.Interact(InputState.KEYDOWN);
                first = true;
                npc.pm = null;
                EventManager.TriggerEvent("");
                CallbackHandler.instance.ToggleRain(true);
            }*/

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
