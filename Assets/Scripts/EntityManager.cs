// Bachelor of Software Engineering
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
    #region Singleton
    public static EntityManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("EntityManager already exists!");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion Singleton
    #region Inspector Variables
    public GameObject player;
    public GameObject playerOnWhale;
    public NewGrappleHook grappleHook;
    public GameObject whale;
    [HideInInspector] public GameObject SpeedBoostRingContainer;
    #endregion

    #region Local Variables

    private const float BoundaryWhaleOffset = 20f; //Value whale is moved by when it is outside level boundaries

    #endregion

    private void Start()
    {
        TogglePlayer(true);
    }

    // Action to link up whale control and grapple script active to
    public event Action<bool> toggleControl;
    public void TogglePlayer(bool _toggle)
    {
        //whale.GetComponent<OrbitScript>().enabled = _toggle;
        if(player.GetComponent<GliderMovement>().enabled && !_toggle) player.GetComponent<GliderMovement>().Toggle();
        player.SetActive(_toggle);
        playerOnWhale.SetActive(!_toggle);

        if (!_toggle) EventManager.TriggerEvent("WhaleTutorial");

        grappleHook.pc = player.GetComponent<GrappleScript>().shootPoint.transform;

        if(!player.GetComponent<GliderMovement>().enabled) CameraManager.instance.SwitchCamera(_toggle ? CameraType.PlayerCamera : CameraType.WhaleCamera);
        Cursor.lockState = CursorLockMode.Locked;

        if (toggleControl != null)
            toggleControl(_toggle);
    }

    public bool TeleportPlayer(Transform locationToTeleport)
    {
        if(locationToTeleport == null) return false;
        if (!player || !instance.player.activeSelf) return false;
        for (int i = 0; i < 2; i++) //not sure why but this needs to be repeated twice in order for the offset to actually be removed
        {
            //Update player container position by calculating offset
            var offset = player.transform.parent.position - player.transform.position;
            player.transform.parent.position = (locationToTeleport.position + Vector3.up) + offset;
            
            //Update rotation for both player
            var rotation = locationToTeleport.rotation;
            player.transform.rotation = rotation;
        }
        return true;
    }

    public void TeleportPlayer(Vector3 pos)
    {
        if (!player || !instance.player.activeSelf) return;
        for (int i = 0; i < 2; i++) //not sure why but this needs to be repeated twice in order for the offset to actually be removed
        {
            //Update player container position by calculating offset
            var offset = player.transform.parent.position - player.transform.position;
            player.transform.parent.position = pos + offset;
        }
        return;
    }

    public void MovePlayerToPlayerOnWhale()
    {
        player.transform.position = playerOnWhale.transform.position;
        player.transform.rotation = Quaternion.LookRotation(grappleHook.transform.position.normalized, Vector3.up);
    }

    public void OnDismountPlayer(Transform dismountPosition)
    {
        grappleHook.connected = false;
        grappleHook.connectedObj = null;
        //player.GetComponent<GrappleScript>().active = false;
        //grappleHook.gameObject.layer = LayerMask.NameToLayer("Hook");
        grappleHook.flightTime = 0.0f;



        TogglePlayer(true);
        player.GetComponent<Rigidbody>().velocity = -Vector3.up;
        player.GetComponent<Rigidbody>().useGravity = true;
        player.transform.position = dismountPosition.position;
        player.transform.rotation = Quaternion.identity;
    }
    
    /// <summary>
    /// Description: Moves the whale up if they are below the level boundary.
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 04/12/2021</br>
    /// </summary>
    public void MoveWhaleAboveBoundary(int yLowestBoundary)
    {
        var newWhaleTransform = whale.transform.position;
        newWhaleTransform.y = yLowestBoundary + BoundaryWhaleOffset;
        whale.transform.position = newWhaleTransform;
    }
    
    public void OnPlayerLowerThanBoundary()
    {
        whale.GetComponent<WhaleMovement>().OnPlayerMountWhale();
    }
} 
