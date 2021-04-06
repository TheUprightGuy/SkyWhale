using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaddleScript : MonoBehaviour
{
    WhaleMovement parent;

    public Camera player;
    public Camera whale;

    private void Awake()
    {
        parent = GetComponentInParent<WhaleMovement>();
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerMovement pc = other.gameObject.GetComponent<PlayerMovement>();

        if (pc)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("saddleUp");
                parent.GiveControl();
                player.depth = 0;
                whale.depth = 10;
                other.GetComponent<PlayerMovement>().haveControl = false;
            }
        }
    }
}
