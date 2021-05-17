using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boostAmount = 5f;
    private void Update()
    {
        //Testing
        if (Input.GetKeyDown(KeyCode.B))
        {
            EntityManager.instance.whale.GetComponent<WhaleMovement>().boost = boostAmount;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Whale"))
        {
            //Provide speed boost to whale
            other.gameObject.GetComponent<WhaleMovement>().boost = boostAmount;
        }
    }
}
