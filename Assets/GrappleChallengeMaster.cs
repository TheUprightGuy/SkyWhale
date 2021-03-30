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

    public void CompleteChallenge()
    {
        if (!complete)
        {
            complete = true;
            ps.Play();
        }
    }

    public void ResetChallenge()
    {
        pm.transform.position = startPoint.transform.position;

        foreach(GrappleChallengePoint n in grapplePoints)
        {
            n.ResetMe();
        }
    }
}
