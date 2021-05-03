/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   GrappleStartPoint.cs
  Description :   Starts the grapple challenge. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCheckPoint : MonoBehaviour
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
    /// Description: Gets player reference for grapple challenge master and updates last checkpoint.
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 04/27/2021</br>
    /// </summary>
    /// <param name="other">Triggering Object</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            gcm.pm = other.GetComponent<PlayerMovement>();
            gcm.LastCheckPoint = this;
        }
    }
    #endregion Trigger
}
