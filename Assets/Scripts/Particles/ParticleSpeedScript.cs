/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   ParticleSpeedScript.cs
  Description :   Speed lines around whale fins based on current speed. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpeedScript : MonoBehaviour
{
    #region Setup
    ParticleSystem ps;
    /// <summary>
    /// Description: Setup Local Components.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    #endregion Setup

    /// <summary>
    /// Description: Update alpha based on movespeed - faster speeds = more opaque and longer lines.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    void Update()
    {
        if (!GetComponentInParent<WhaleMovement>())
            return;

        float alpha = GetComponentInParent<WhaleMovement>().currentSpeed / 200;
        
        //Debug.Log(alpha);
        ps.startColor = new Color(ps.startColor.r, ps.startColor.g, ps.startColor.b, alpha);
        ps.startLifetime = alpha * 100;

    }
}
