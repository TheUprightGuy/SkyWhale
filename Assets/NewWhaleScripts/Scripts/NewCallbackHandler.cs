using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NewCallbackHandler : MonoBehaviour
{
    public static NewCallbackHandler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Callback Handler already exists!");
            Destroy(this.gameObject);
        }
        instance = this;
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


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        
    }
}
