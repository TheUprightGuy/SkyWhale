using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandRangeTrigger : MonoBehaviour
{
    public GameObject heightRef;
    public Transform pmTPPos;

    private void OnTriggerEnter(Collider other)
    {
        WhaleMovement wm = other.GetComponent<WhaleMovement>();

        if (wm)
        {
            EventManager.TriggerEvent("MoveToSecondIsland");
            CallbackHandler.instance.SetNewOrbitRefer(heightRef);
            other.GetComponent<PickUp>().secondIslandPos = pmTPPos;
        }
    }
}
