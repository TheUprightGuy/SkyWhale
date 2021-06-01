using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTransparency : MonoBehaviour
{
    ParticleSystem ps;
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        CheckDistanceToPlayer();        
    }

    void SetTransparency(float _alpha)
    {
        ps.startColor = new Color(ps.startColor.r, ps.startColor.g, ps.startColor.b, _alpha);
    }

    public float dist;

    void CheckDistanceToPlayer()
    {
        dist = Mathf.Min(Vector3.Distance(EntityManager.instance.player.transform.position, this.transform.position), Vector3.Distance(EntityManager.instance.playerOnWhale.transform.position, this.transform.position));
        float temp = Mathf.Clamp(dist, 0, 80.0f);
        temp = Remap(temp, 0.0f, 80.0f, 0.8f, 0.0f);
        SetTransparency(temp);
    }

    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
