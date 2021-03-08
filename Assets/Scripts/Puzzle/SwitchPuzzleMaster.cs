using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SwitchPuzzleMaster : MonoBehaviour
{
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
    }

    public List<PuzzleSwitch> switches;
    public Material on;
    public Material off;
    public bool complete;
    public UnityEvent CompletionEvent;
    public Cinemachine.CinemachineVirtualCamera cam;

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

    private void Update()
    {
        if (CheckComplete())
        {
            CompletionEvent.Invoke();
        }
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
        cam.m_Priority = 0;
        complete = true;
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TestMovement player = other.GetComponent<TestMovement>();
        if (player && !complete)
        {
            CallbackHandler.instance.LerpCam();
            cam.m_Priority = 20;
            // switch to fixed cam;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TestMovement player = other.GetComponent<TestMovement>();
        if (player)
        {
            cam.m_Priority = 0;
            // switch to normal cam;
        }
    }
}
