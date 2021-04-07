/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   CallbackHandler.cs
  Description :   Handles a number of different callbacks between unrelated game elements. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CallbackHandler : MonoBehaviour
{
    #region Singleton
    public static CallbackHandler instance;
    /// <summary>
    /// Description: Singleton Setup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Callback Handler already exists!");
            Destroy(this.gameObject);
        }
        instance = this;
    }
    #endregion Singleton

    private void Update()
    {
        // temp - restart scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    public event Action<bool> pause;
    /// <summary>
    /// Description: Pauses all movement based objects.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_pause">Pause State</param>
    public void Pause(bool _pause)
    {
        if (pause != null)
        {
            pause(_pause);
        }
    }

    public Action<GameObject> setDestination;
    /// <summary>
    /// Description: Sets whales destination for pickup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_destination">Object to find path to</param>
    public void SetDestination(GameObject _destination)
    {
        if (setDestination != null)
        {
            setDestination(_destination);
        }
    }

    public Action<GameObject> setOrbit;
    /// <summary>
    /// Description: Sets whales object to orbit.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_orbit">Object to orbit</param>
    public void SetOrbit(GameObject _orbit)
    {
        if (setOrbit != null)
        {
            setOrbit(_orbit);
        }
    }

    public event Action spawnCollectableIsland;
    /// <summary>
    /// Description: Spawns Collectable Island.
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void SpawnCollectableIsland()
    {
        if (spawnCollectableIsland != null)
        {
            spawnCollectableIsland();
        }
    }


    public event Action<Dialogue> setDialogue;
    /// <summary>
    /// Description: Passes dialogue to dialogue manager.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_dialogue">Dialogue to Start</param>
    public void SetDialogue(Dialogue _dialogue)
    {
        if (setDialogue != null)
        {
            setDialogue(_dialogue);
        }
    }

    public event Action stopDialogue;
    /// <summary>
    /// Description: Stops current dialogue.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void StopDialogue()
    {
        if (stopDialogue != null)
        {
            stopDialogue();
        }
    }
}
