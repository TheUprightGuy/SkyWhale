using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventHandler.instance.OnEndTriggered();
    }
}
