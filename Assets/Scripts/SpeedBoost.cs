using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boostAmount = 5f;

    private void Start()
    {
        if (EntityManager.instance.SpeedBoostRingContainer == null)
        {
            EntityManager.instance.SpeedBoostRingContainer = transform.parent.gameObject;
            transform.parent.gameObject.SetActive(false);
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