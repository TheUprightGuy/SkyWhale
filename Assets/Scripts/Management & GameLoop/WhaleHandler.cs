using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleHandler : MonoBehaviour
{
    #region Singleton
    public static WhaleHandler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Whale Handler exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion Singleton

    /***************WHALE******************/
    #region Orbit
    public event Action<bool> landingTooltip;
    public void LandingTooltip(bool _toggle)
    {
        if (landingTooltip != null)
        {
            landingTooltip(_toggle);
        }
    }

    public event Action<bool> orbit;
    public void Orbit(bool _toggle)
    {
        if (orbit != null)
        {
            orbit(_toggle);
        }
    }
    public event Action shiftWhale;
    public void ShiftWhale()
    {
        if (shiftWhale != null)
        {
            shiftWhale();
        }
    }
    public event Action<Transform> startHoming;
    public void StartHoming(Transform _player)
    {
        if (startHoming != null)
        {
            startHoming(_player);
        }
    }
    public event Action pickUpMC;
    public void PickUpMC()
    {
        if (pickUpMC != null)
        {
            pickUpMC();
        }
    }
    public event Action startExit;
    public void StartExit()
    {
        if (startExit != null)
        {
            startExit();
        }
    }

    public event Action moveWhale;
    public void MoveWhale()
    {
        if (moveWhale != null)
        {
            ZeroOut();
            moveWhale();
        }
    }
    public event Action zeroOut;
    public void ZeroOut()
    {
        if (zeroOut != null)
        {
            zeroOut();
        }
    }

    #endregion Orbit

    /************RIDER**************/
    #region RiderMovement
    public event Action moveToSaddle;
    public void MoveToSaddle()
    {
        if (moveToSaddle != null)
        {
            moveToSaddle();
        }
    }

    public event Action moveToFire;
    public void MoveToFire()
    {
        if (moveToFire != null)
        {
            moveToFire();
        }
    }
    #endregion RiderMovement
}
