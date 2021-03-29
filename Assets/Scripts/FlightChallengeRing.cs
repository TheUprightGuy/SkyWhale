using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightChallengeRing : MonoBehaviour
{
    #region Setup
    MeshRenderer mr;
    [HideInInspector] public FlightChallengeMaster parent;
    [HideInInspector] public bool complete;
    private void Awake()
    {
        mr = GetComponentInChildren<MeshRenderer>();
    }
    #endregion Setup

    [Header("Setup Fields")]
    public Material incompleteMat;
    public Material completeMat;

    void SetMat(bool _complete)
    {
        mr.material = complete ? completeMat : incompleteMat;
    }

    void CompleteRing()
    {
        complete = true;
        SetMat(complete);
    }

    public  void ResetRing()
    {
        complete = false;
        SetMat(complete);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            CompleteRing();
            parent.CheckRings();
        }
    }
}
