﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CallbackHandler : MonoBehaviour
{
    public static CallbackHandler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Callback Handler already exists!");
            Destroy(this.gameObject);
        }
        instance = this;
    }

    private void Update()
    {
        // temp - restart scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    public event Action<bool> pause;
    public void Pause(bool _pause)
    {
        if (pause != null)
        {
            pause(_pause);
        }
    }



    public Action<GameObject> setDestination;
    public void SetDestination(GameObject _destination)
    {
        if (setDestination != null)
        {
            setDestination(_destination);
        }
    }

    public Action<GameObject> setOrbit;
    public void SetOrbit(GameObject _orbit)
    {
        if (setOrbit != null)
        {
            setOrbit(_orbit);
        }
    }

    public event Action spawnCollectableIsland;
    public void SpawnCollectableIsland()
    {
        if (spawnCollectableIsland != null)
        {
            spawnCollectableIsland();
        }
    }


    public event Action<Dialogue> setDialogue;
    public void SetDialogue(Dialogue _dialogue)
    {
        if (setDialogue != null)
        {
            setDialogue(_dialogue);
        }
    }

    public event Action stopDialogue;
    public void StopDialogue()
    {
        if (stopDialogue != null)
        {
            stopDialogue();
        }
    }
}
