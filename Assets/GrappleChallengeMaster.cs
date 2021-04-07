/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   GrappleChallengeMaster.cs
  Description :   Checks if player has reached the end of the grapple challenge. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleChallengeMaster : MonoBehaviour
{
    #region Setup
    [HideInInspector] public PlayerMovement pm;
    ParticleSystem ps;
    GrappleStartPoint startPoint;

    bool complete;
    List<GrappleChallengePoint> grapplePoints;
    /// <summary>
    /// Description: Get Local Components.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        grapplePoints = new List<GrappleChallengePoint>();

        foreach(GrappleChallengePoint n in GetComponentsInChildren<GrappleChallengePoint>())
        {
            grapplePoints.Add(n);
        }

        ps = GetComponentInChildren<ParticleSystem>();
        startPoint = GetComponentInChildren<GrappleStartPoint>();
    }
    #endregion Setup

    /// <summary>
    /// Description: Start Confetti Burst.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void CompleteChallenge()
    {
        if (!complete)
        {
            complete = true;
            ps.Play();
        }
    }

    /// <summary>
    /// Description: Reset player position and grapple objects.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    public void ResetChallenge()
    {
        pm.transform.position = startPoint.transform.position;

        foreach(GrappleChallengePoint n in grapplePoints)
        {
            n.ResetMe();
        }
    }
}
