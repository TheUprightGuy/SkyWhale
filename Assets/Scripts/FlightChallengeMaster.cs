using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightChallengeMaster : MonoBehaviour
{
    #region Setup
    ParticleSystem ps;
    List<FlightChallengeRing> rings;
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

    void ResetRings()
    {
        foreach (FlightChallengeRing n in rings)
        {
            n.ResetRing();
        }
    }


    // Update is called once per frame
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
