using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPFX : MonoBehaviour
{
    public GameObject rings;
    public GameObject door;
    public ParticleSystem[] ringPFX;
    public ParticleSystem[] doorPFX;

    private void Awake()
    {
        ringPFX = rings.GetComponentsInChildren<ParticleSystem>();
        doorPFX = door.GetComponentsInChildren<ParticleSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartPFX();
        }
    }

    public void StartPFX()
    {
        foreach(ParticleSystem n in ringPFX)
        {
            n.Play();
        }
    }

    public void StopPFX()
    {
        foreach (ParticleSystem n in ringPFX)
        {
            n.Stop();
        }
    }

    public void PlayDoorPFX()
    {
        foreach (ParticleSystem n in doorPFX)
        {
            n.Play();
        }
    }

    public void StopDoorPFX()
    {
        foreach (ParticleSystem n in doorPFX)
        {
            n.Stop();
        }
    }
}
