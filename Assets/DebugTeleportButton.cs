using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTeleportButton : MonoBehaviour
{
    public Transform locationToTeleport;
    public void TeleportToLocation()
    {
        if(locationToTeleport == null) return;
        if (!EntityManager.instance.player) return;
        EntityManager.instance.player.transform.parent.position = locationToTeleport.position;
        EntityManager.instance.player.transform.parent.rotation = locationToTeleport.rotation;
        transform.parent.parent.BroadcastMessage("ToggleDebugToolMenu");
    }
}
