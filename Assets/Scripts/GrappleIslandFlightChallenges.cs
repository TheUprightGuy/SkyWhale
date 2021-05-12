using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleIslandFlightChallenges : MonoBehaviour
{
    public List<GameObject> flightChallenges;
    private void Awake()
    {
        EventManager.StartListening("PilotVillagerQuestStart", EnableChallenges);
        foreach (var flightChallenge in flightChallenges)
        {
            flightChallenge.SetActive(false);
        }
    }

    private void EnableChallenges()
    {
        foreach (var flightChallenge in flightChallenges)
        {
            flightChallenge.SetActive(true);
        }
    }
}
