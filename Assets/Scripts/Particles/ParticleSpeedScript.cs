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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float alpha;
        if (Movement.instance)
        {
            alpha = Movement.instance.currentSpeed / 200;
        }
        else
        {
            alpha = Movement.instance.currentSpeed / 200;
        }
        //Debug.Log(alpha);
        ps.startColor = new Color(ps.startColor.r, ps.startColor.g, ps.startColor.b, alpha);
        ps.startLifetime = alpha * 100;

    }
}
