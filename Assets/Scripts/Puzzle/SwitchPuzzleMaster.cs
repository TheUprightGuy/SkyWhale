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
    #region Singleton
    private void Start()
    {
        foreach (Transform n in transform)
        {
            switches.Add(n.GetComponent<PuzzleSwitch>());
        }

        foreach (PuzzleSwitch n in switches)
        {
            n.active = Random.value > 0.5f;
            n.on = on;
            n.off = off;
            n.Switch();
        }
    }
    #endregion Singleton

    [Header("Setup Fields")]
    public Material on;
    public Material off;

    //Local Variables
    List<PuzzleSwitch> switches;
    PlayerMovement pm;
    [HideInInspector] public bool complete;
    bool inUse;


    /// <summary>
    /// Description: Switches camera upon keypress when player is in trigger area.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Update()
    {
        if (complete)
            return;

        if (pm && Input.GetKeyDown(KeyCode.E))
        {
            inUse = !inUse;
            ToggleCam(inUse);
            Cursor.lockState = (inUse) ? CursorLockMode.None : CursorLockMode.Locked;
        }
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player)
        {
            ToggleCam(false);
            pm = null;
        }
    }
    #endregion Triggers
}
