using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpeedScript : MonoBehaviour
{
    #region Setup
    ParticleSystem ps;
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    #endregion Setup

    // Update is called once per frame
    void Update()
    {
        float alpha = GetComponentInParent<WhaleMovement>().currentSpeed / 200;
        
        //Debug.Log(alpha);
        ps.startColor = new Color(ps.startColor.r, ps.startColor.g, ps.startColor.b, alpha);
        ps.startLifetime = alpha * 100;

    }
}
