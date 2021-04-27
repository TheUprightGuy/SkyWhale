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
            SpeechOutOfRange();
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

    public event Action<Transform> speechInRange;
    /// <summary>
    /// Description: Prompts the speech ui element to show.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    /// <param name="_dialogueTransform"></param>
    public void SpeechInRange(Transform _dialogueTransform)
    {
        if (speechInRange != null)
        {
            speechInRange(_dialogueTransform);
        }
    }

    public event Action speechOutOfRange;
    /// <summary>
    /// Description: Prompts the speech ui element to hide.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    public void SpeechOutOfRange()
    {
        if (speechOutOfRange != null)
        {
            speechOutOfRange();
        }
    }

    public event Action<Transform> puzzleInRange;
    /// <summary>
    /// Description: Prompts the speech ui element to show.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    /// <param name="_position"></param>
    public void PuzzleInRange(Transform _position)
    {
        if (puzzleInRange != null)
        {
            puzzleInRange(_position);
        }
    }

    public event Action puzzleOutOfRange;
    /// <summary>
    /// Description: Prompts the speech ui element to hide.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    public void PuzzleOutOfRange()
    {
        if (puzzleOutOfRange != null)
        {
            puzzleOutOfRange();
        }
    }

    public event Action<InputType,string, string> displayHotkey;
    public void DisplayHotkey(InputType _type, string _action, string _subName)
    {
        if (displayHotkey != null)
        {
            displayHotkey(_type, _action, _subName);
        }
    }

    public event Action<string> hideHotkey;
    public void HideHotkey(string _action)
    {
        if (hideHotkey != null)
        {
            hideHotkey(_action);
        }
    }

    public event Action showGlide;
    public void ShowGlide()
    {
        if (showGlide != null)
        {
            showGlide();
        }
    }

    public event Action hideGlide;
    public void HideGlide()
    {
        if (hideGlide != null)
        {
            hideGlide();
        }
    }

    public event Action showGrapple;
    public void ShowGrapple()
    {
        if (showGrapple != null)
        {
            showGrapple();
        }
    }

    public event Action hideGrapple;
    public void HideGrapple()
    {
        if (hideGrapple != null)
        {
            hideGrapple();
        }
    }

    public event Action showSpeech;
    public void ShowSpeech()
    {
        if (showSpeech != null)
        {
            showSpeech();
        }
    }

    public event Action hideSpeech;
    public void HideSpeech()
    {
        if (hideSpeech != null)
        {
            hideSpeech();
        }
    }

    public event Action resetCamera;
    public void ResetCamera()
    {
        if (resetCamera != null)
        {
            resetCamera();
        }
    }


    public event Action<Quest> startTrackingQuest;
    public void StartTrackingQuest(Quest _quest)
    {
        if (startTrackingQuest != null)
        {
            startTrackingQuest(_quest);
        }
    }

    public event Action updateObjectives;
    public void UpdateObjectives()
    {
        if (updateObjectives != null)
        {
            updateObjectives();
        }
    }
}
