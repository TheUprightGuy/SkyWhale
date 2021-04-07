/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   GrappleEndPoint.cs
  Description :   Notifies the GrappleChallengeMaster that challenge was completed. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleEndPoint : MonoBehaviour
{
    #region Setup
    GrappleChallengeMaster gcm;
    /// <summary>
    /// Description: Get Local Components.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        gcm = GetComponentInParent<GrappleChallengeMaster>();
    }
    #endregion Setup
    #region Trigger
    /// <summary>
    /// Description: Notify GrappleChallengeMaster that challenge was completed.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="other">Triggering Object</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            gcm.CompleteChallenge();
        }
    }
    #endregion Trigger
}
