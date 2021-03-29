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
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        EventManager.StartListening("TalkedToBlacksmith", StartElevator);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("TalkedToBlacksmith", StartElevator);
    }

    #endregion Setup

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


    private void FixedUpdate()
    {
        if (!inUse)
            return;

        switch (ec)
        {
            case ElevatorControl.STATIONARYDOWN:
            {
                timer -= Time.fixedDeltaTime;
                if (timer <= 0)
                {
                    SwitchEC(ElevatorControl.UP);
                }

                break;
            }
            case ElevatorControl.STATIONARYUP:
            {
                timer -= Time.fixedDeltaTime;
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
                rb.MovePosition(transform.position + transform.up * Time.fixedDeltaTime);
                break;
            }
            case ElevatorControl.DOWN:
            {
                if (transform.position.y <= botPos.y)
                {
                    SwitchEC(ElevatorControl.STATIONARYDOWN);
                    break;
                }
                rb.MovePosition(transform.position - transform.up * Time.fixedDeltaTime);
                break;
            }
        }
    }

    void SwitchEC(ElevatorControl _ec)
    {
        ec = _ec;
        timer = 2.0f;
    }
}
