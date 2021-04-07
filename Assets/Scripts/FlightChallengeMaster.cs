/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   FlightChallengeMaster.cs
  Description :   Checks if player has reached the end of the flight challenge. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightChallengeMaster : MonoBehaviour
{
    #region Setup
    ParticleSystem ps;
    List<FlightChallengeRing> rings;
    /// <summary>
    /// Description: Setup local components.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        rings = new List<FlightChallengeRing>();
        foreach(FlightChallengeRing n in GetComponentsInChildren<FlightChallengeRing>())
        {
            rings.Add(n);
            n.parent = this;
        }

        ps = GetComponentInChildren<ParticleSystem>();
    }
    #endregion Setup

    float timeOut;

    /// <summary>
    /// Description: Check if challenge was completed.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void CheckRings()
    {
        foreach(FlightChallengeRing n in rings)
        {
            if (n.complete)
                timeOut = 3.0f;

            if (!n.complete)
                return;
        }

        Debug.Log("Passed Challenge!");
        ps.Play();
        for (int i = rings.Count - 1; i >= 0; i--)
        {
            Destroy(rings[i].gameObject);
        }
        rings.Clear();
    }

    /// <summary>
    /// Description: Reset Challenge.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void ResetRings()
    {
        foreach (FlightChallengeRing n in rings)
        {
            n.ResetRing();
        }
    }

    /// <summary>
    /// Description: Resets rings after timer.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void Update()
    {
        if (timeOut <= 0)
        {
            ResetRings();
            return;
        }

        timeOut -= Time.deltaTime;
    }
}
