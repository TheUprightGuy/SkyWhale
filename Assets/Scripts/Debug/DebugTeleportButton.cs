using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTeleportButton : MonoBehaviour
{
    public Transform locationToTeleport;
    public void TeleportToLocation()
    {
        EntityManager.instance.TeleportPlayer(locationToTeleport);
        transform.parent.parent.BroadcastMessage("ToggleDebugToolMenu");
    }
}
