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

        if (CheckComplete())
        {
            Debug.Log("COMPLETE");
        }
    }

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
        return true;
    }

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
}
