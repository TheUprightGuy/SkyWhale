using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTeleportButton : MonoBehaviour
{
    public Transform locationToTeleport;
    public void TeleportToLocation()
    {
        if(locationToTeleport == null) return;
        if (!EntityManager.instance.player || !EntityManager.instance.player.activeSelf) return;
        for (int i = 0; i < 2; i++) //not sure why but this needs to be repeated twice in order for the offset to actually be removed
        {
            var offset = EntityManager.instance.player.transform.parent.position -
                         EntityManager.instance.player.transform.position;
            EntityManager.instance.player.transform.parent.position = locationToTeleport.position + offset;
            //EntityManager.instance.player.transform.position = locationToTeleport.position;
            EntityManager.instance.player.transform.parent.rotation = locationToTeleport.rotation;
        }

        transform.parent.parent.BroadcastMessage("ToggleDebugToolMenu");
    }
}
