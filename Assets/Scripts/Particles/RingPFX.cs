using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingPFX : MonoBehaviour
{
    #region Setup
    ParticleSystem[] ps;
    private void Awake()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem n in ps)
        {
            n.Stop();
        }
    }
    #endregion Setup

    public void StartPFX()
    {
        foreach (ParticleSystem n in ps)
        {
            n.Play();
        }
    }

    public void StopPFX()
    {
        foreach (ParticleSystem n in ps)
        {
            n.Stop();
        };
    }
}
