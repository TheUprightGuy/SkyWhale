﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    [Header("Required Fields")]
    public NewGrappleHook hook;
    public LayerMask grappleableLayers;

    public bool grapplingFromWhale = false;
    public bool shotGrapple = false;

    #region Setup
    // Local Variables
    public Transform camToShootFrom;
    GameObject grappleReticule;
    PlayerMovement pm;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        grappleReticule = GetComponentInChildren<UnityEngine.UI.Image>().gameObject;
    }

    private void OnEnable()
    {
        shotGrapple = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        hook.grappleableLayers = grappleableLayers;
        ToggleAim(false);
    }
    #endregion Setup

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))//Aiming with right click
        {
            ToggleAim(true);
            TimeSlowDown.instance.SlowDown();
        }

        if (Input.GetMouseButtonUp(1))//Stopping aiming
        {
            ToggleAim(false);
            TimeSlowDown.instance.SpeedUp();
        }

        if (Input.GetMouseButtonDown(0) && (Input.GetMouseButton(1) || AbleToRetract()))
        {
            FireHook();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!grapplingFromWhale)
        {
            pm.enabled = !hook.connected;
            
            transform.up = Vector3.up;
        }

        if (!hook.connected) return;
        switch (grapplingFromWhale)
        {
            case true when shotGrapple:
                if(hook.connectedObj.layer == 10) return;
                CallbackHandler.instance.GrappleHitFromWhale(transform);
                gameObject.SetActive(false);
                return;
            case false:
            {
                Vector3 moveDir = Vector3.Normalize(hook.transform.position - transform.position) * 8.0f;
                rb.AddForce(moveDir * TimeSlowDown.instance.timeScale, ForceMode.Acceleration);
                transform.LookAt(hook.transform);
                return;
            }
        }
    }

    public void FireHook()
    {
        if (!HookInUse())
        {
            hook.Fire(this.transform, camToShootFrom.transform.forward);
            shotGrapple = true;
        }
        else if (AbleToRetract())
        {
            // Start retracting
            hook.YeetPlayer(this.transform);
            hook.retracting = true;
            hook.connected = false;
            hook.manualRetract = true;
        }
    }

    void ToggleAim(bool _startAim)
    {
        if(grapplingFromWhale) return;
        if (grappleReticule != null)
        {
            grappleReticule.SetActive(_startAim);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hook.connected && shotGrapple)
        {
            hook.connected = false;
            hook.retracting = true;
        }
    }

    private void OngrappleHitFromWhale(Transform obj)
    {
        ToggleAim(false);
    }


    #region Checks
    bool HookInUse()
    {
        return !(!hook.connected && !hook.retracting && hook.flightTime <= 0.0f);
    }

    bool AbleToRetract()
    {
        return (IsConnected() || InFlight());
    }

    public bool IsConnected()
    {
        return (hook.connected && !hook.retracting);
    }

    bool InFlight()
    {
        return (hook.enabled && hook.flightTime > 0.0f);
    }
    #endregion Checks
}
