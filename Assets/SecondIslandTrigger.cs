using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondIslandTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement pm = other.GetComponent<PlayerMovement>();

        if (pm)
        {
            EventManager.TriggerEvent("FlyToRuinIsland");
        }
    }
}
