using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSystem : MonoBehaviour
{
    ParticleSystem ps;
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        CallbackHandler.instance.toggleRain += ToggleRain;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.toggleRain += ToggleRain;
    }

    void ToggleRain(bool _toggle)
    {
        if (_toggle)
        {
            ps.Play();
            return;
        }
        ps.Stop();
    }
}
