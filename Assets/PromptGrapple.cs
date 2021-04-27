using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptGrapple : MonoBehaviour
{
    PlayerMovement pm;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();

            if (pm.GetComponent<GrappleScript>().enabled)
                CallbackHandler.instance.ShowGrapple();
                //CallbackHandler.instance.DisplayHotkey(InputType.PLAYER, "GrappleAim", "Aim");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() == pm)
        {
            //CallbackHandler.instance.HideHotkey("GrappleAim");
            CallbackHandler.instance.HideGrapple();
        }
    }
}
