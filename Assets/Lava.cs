using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
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
            // ResetMe
            gcm.ResetChallenge();
        }
    }
}
