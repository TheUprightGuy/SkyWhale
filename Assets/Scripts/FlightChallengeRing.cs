/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   FlightChallengeRing.cs
  Description :   Notifies the FlightChallengeMaster when ring is flown through. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class FlightChallengeRing : MonoBehaviour
{
    #region Setup
    MeshRenderer mr;
    [HideInInspector] public FlightChallengeMaster parent;
    [HideInInspector] public bool complete;
    /// <summary>
    /// Description: Setup local references.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        mr = GetComponentInChildren<MeshRenderer>();
    }
    #endregion Setup

    [Header("Setup Fields")]
    public Material incompleteMat;
    public Material completeMat;

    /// <summary>
    /// Description: Sets material to completed/incompleted colors.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_complete">Completed?</param>
    void SetMat(bool _complete)
    {
        mr.material = complete ? completeMat : incompleteMat;
    }

    /// <summary>
    /// Description: Sets ring to completed.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void CompleteRing()
    {
        complete = true;
        SetMat(complete);
        AudioManager.instance.PlaySound("Checkpoint");
    }

    /// <summary>
    /// Description: Rests the ring.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void ResetRing()
    {
        complete = false;
        SetMat(complete);
    }

    /// <summary>
    /// Description: Detects player passing through ring.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="other">Triggering Object</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            CompleteRing();
            parent.CheckRings();
        }
    }
}
