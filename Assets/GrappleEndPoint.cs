using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleEndPoint : MonoBehaviour
{
    #region Setup
    GrappleChallengeMaster gcm;
    private void Awake()
    {
        gcm = GetComponentInParent<GrappleChallengeMaster>();
    }
    #endregion Setup

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            gcm.CompleteChallenge();
        }
    }
}
