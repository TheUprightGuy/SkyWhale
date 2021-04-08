﻿// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   EntityManager.cs
// Description :   Singleton that handles enabling and disabling objects appropriately on different events.
//                 Mainly when mounting/dismounting the whale.
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz

//This was also worked on by Wayd when he helped clean up/fix/simplify dismount functionality after merging broke dismount by grapple
using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    #region Local Variables
    public GameObject player;
    public GameObject playerOnWhale;
    public GameObject _whaleGrappleUIElement;
    public NewGrappleHook grappleHook;
    #endregion
    #region Singleton
    public static EntityManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("EntityManager already exists!");
            Destroy(this.gameObject);
        }
        instance = this;
    }
    #endregion Singleton

    private void Start()
    {
        TogglePlayer(true); 
    }

    // Action to link up whale control and grapple script active to
    public event Action<bool> toggleControl;
    public void TogglePlayer(bool _toggle)
    {
        player.SetActive(_toggle);
        playerOnWhale.SetActive(!_toggle);
        grappleHook.pc = player.GetComponent<GrappleScript>().shootPoint.transform;

        if (!_toggle)
        {
            player.transform.position = playerOnWhale.transform.position;
            player.transform.rotation = Quaternion.LookRotation(grappleHook.transform.position.normalized, Vector3.up);
        }

        CameraManager.instance.SwitchCamera(_toggle ? CameraType.PlayerCamera : CameraType.WhaleCamera);

        if (toggleControl != null)
            toggleControl(_toggle);
    }

    public void OnDismountPlayer(Transform dismountPosition)
    {
        grappleHook.connected = false;
        grappleHook.connectedObj = null;
        grappleHook.gameObject.layer = LayerMask.NameToLayer("Hook");
        grappleHook.flightTime = 0.0f;

        TogglePlayer(true);

        player.transform.position = dismountPosition.position;
        player.transform.rotation = Quaternion.identity;
    }
} 
