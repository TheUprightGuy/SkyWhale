/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   ElevatorScript.cs
  Description :   A very basic elevator that cycles between up and down. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElevatorControl
{
    STATIONARYDOWN,
    DOWN,
    STATIONARYUP,
    UP
}

public class ElevatorScript : MonoBehaviour
{
    #region Setup
    Rigidbody rb;
    bool pause;
    /// <summary>
    /// Description: Setup local components.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Start()
    {
        EventManager.StartListening("TalkedToBlacksmith", StartElevator);
        CallbackHandler.instance.pause += Pause;
    }
    private void OnDestroy()
    {
        EventManager.StopListening("TalkedToBlacksmith", StartElevator);
        CallbackHandler.instance.pause -= Pause;
    }
    void Pause(bool _pause)
    {
        pause = _pause;
    }

    #endregion Setup

    /// <summary>
    /// Description: Turns the elevator on.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void StartElevator()
    {
        inUse = true;
    }


    // Local Variables
    public bool inUse;
    public ElevatorControl ec;

    public Vector3 topPos;
    public Vector3 botPos;
    public float timer;

    /// <summary>
    /// Description: Handles movement up and down.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void FixedUpdate()
    {
        if (!inUse || pause)
            return;

        switch (ec)
        {
            case ElevatorControl.STATIONARYDOWN:
            {
                timer -= Time.fixedDeltaTime * TimeSlowDown.instance.timeScale;
                if (timer <= 0)
                {
                    SwitchEC(ElevatorControl.UP);
                }

                break;
            }
            case ElevatorControl.STATIONARYUP:
            {
                timer -= Time.fixedDeltaTime * TimeSlowDown.instance.timeScale;
                if (timer <= 0)
                {
                    SwitchEC(ElevatorControl.DOWN);
                }

                break;
            }
            case ElevatorControl.UP:
            {
                if (transform.position.y >= topPos.y)
                {
                    SwitchEC(ElevatorControl.STATIONARYUP);
                    break;
                }
                rb.MovePosition(transform.position + transform.up * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale);
                break;
            }
            case ElevatorControl.DOWN:
            {
                if (transform.position.y <= botPos.y)
                {
                    SwitchEC(ElevatorControl.STATIONARYDOWN);
                    break;
                }
                rb.MovePosition(transform.position - transform.up * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale);
                break;
            }
        }
    }

    /// <summary>
    /// Description: Switches Elevator State.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_ec">State</param>
    void SwitchEC(ElevatorControl _ec)
    {
        ec = _ec;
        timer = 2.0f;
    }
}
