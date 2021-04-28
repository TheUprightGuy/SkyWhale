using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTeleportLocations : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DebugTool.instance.AddNewTeleportLocation(transform);
    }
}
