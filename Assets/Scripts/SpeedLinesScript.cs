using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLinesScript : MonoBehaviour
{
    ParticleSystem ps;
    GliderMovement gm;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        gm = GetComponentInParent<GliderMovement>();
    }

    private void Update()
    {
        ps.startSize = Mathf.Clamp01(0.75f * (gm.currentSpeed / gm.maxSpeed) - 0.25f); 
    }
}
