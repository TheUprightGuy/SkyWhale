using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleChallengePoint : MonoBehaviour
{
    #region Setup
    // Local Variables
    bool collided;
    float lifeTime = 3.0f;
    MeshCollider bc;
    MeshRenderer mr;
    new bool enabled = true;

    private void Awake()
    {
        bc = GetComponent<MeshCollider>();
        mr = GetComponent<MeshRenderer>();
    }
    #endregion Setup

    private void Update()
    {
        if (!enabled)
        {
            lifeTime -= Time.deltaTime * TimeSlowDown.instance.timeScale;
            if (lifeTime <= 0)
            {
                Toggle(true);
            }
        }

        if (!collided)
            return;

        lifeTime -= Time.deltaTime * TimeSlowDown.instance.timeScale;
        if (lifeTime <= 0)
        {
            Toggle(false);
        }
    }

    void Toggle(bool _toggle)
    {
        bc.enabled = _toggle;
        mr.enabled = _toggle;
        enabled = _toggle;
        lifeTime = 3.0f;
        collided = false;
    }

    public void ResetMe()
    {
        Toggle(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        collided = true;
    }
}
