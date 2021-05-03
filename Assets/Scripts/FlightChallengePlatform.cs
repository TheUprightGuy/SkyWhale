using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightChallengePlatform : MonoBehaviour
{
    #region Setup
    MeshRenderer mr;
    [HideInInspector] public FlightChallengeMaster parent;
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
        mr.material = _complete ? completeMat : incompleteMat;
    }

    /// <summary>
    /// Description: Updates material if challenge completed.
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="other">Triggering Object</param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() && parent.rings.Count == 0)
        {
            SetMat(true);
        }
    }
}
