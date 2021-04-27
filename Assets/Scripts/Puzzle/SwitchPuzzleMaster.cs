/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   SwitchPuzzleMaster.cs
  Description :   Handles puzzle layout and checks for completion upon switch change. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchPuzzleMaster : MonoBehaviour
{
    #region Singleton
    public static SwitchPuzzleMaster instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Another Puzzle Master exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        switches = new List<PuzzleSwitch>();
    }
    #endregion Singleton
    #region Setup
    private void Start()
    {
        foreach (Transform n in transform)
        {
            if (n.GetComponent<PuzzleSwitch>())
                switches.Add(n.GetComponent<PuzzleSwitch>());
        }

        int numOff = 0;

        foreach (PuzzleSwitch n in switches)
        {
            if (numOff < 5)
                n.active = Random.value > 0.5f;
            if (!n.active)
                numOff++;

            n.on = on;
            n.off = off;
            n.Switch();
        }

        VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").MethodToCall.AddListener(Interact);
    }
    #endregion Setup

    [Header("Setup Fields")]
    public Material on;
    public Material off;
    public Transform promptPosition;

    //Local Variables
    List<PuzzleSwitch> switches;
    PlayerMovement pm;
    [HideInInspector] public bool complete;
    bool inUse;


    public void Interact(InputState type)
    {
        if (!pm || complete)
            return;

        inUse = !inUse;
        ToggleCam(inUse);
        Cursor.lockState = (inUse) ? CursorLockMode.None : CursorLockMode.Locked;
        CallbackHandler.instance.HideSpeech();

        if (inUse)
            CallbackHandler.instance.PuzzleOutOfRange();
    }

    /// <summary>
    /// Description: Switches camera if matching type.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_toggle">Puzzle/Player Camera</param>
    public void ToggleCam(bool _toggle)
    {
        inUse = _toggle;
        if (_toggle)
        {
            CameraManager.instance.SwitchCamera(CameraType.PuzzleCamera);
            return;
        }
        CameraManager.instance.SwitchCamera(CameraType.PlayerCamera);
        CallbackHandler.instance.HideSpeech();
    }

    /// <summary>
    /// Description: Checks if puzzle is complete, if so triggers event.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <returns></returns>
    public bool CheckComplete()
    {
        foreach(PuzzleSwitch n in switches)
        {
            if (!n.active)
            {
                return false;
            }
        }

        complete = true;
        inUse = !inUse;
        ToggleCam(inUse);
        Cursor.lockState = (inUse) ? CursorLockMode.None : CursorLockMode.Locked;
        EventManager.TriggerEvent("SwitchPuzzleCompletion");
        EventManager.TriggerEvent("SolvePuzzle");
        CallbackHandler.instance.PuzzleOutOfRange();
        return true;
    }

    #region Triggers
    /// <summary>
    /// Description: Gets reference to player to check if in range.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="other">Triggering Object</param>
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player && !complete)
        {
            pm = player;
            CallbackHandler.instance.PuzzleInRange(promptPosition);
            CallbackHandler.instance.ShowSpeech();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player)
        {
            ToggleCam(false);
            pm = null;
            CallbackHandler.instance.PuzzleOutOfRange();
            CallbackHandler.instance.HideSpeech();
        }
    }
    #endregion Triggers
}
