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
    private bool _isWhaleGrappleUIActive = false;
    #endregion

    public static EntityManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("EntityManager already exists!");
            Destroy(this.gameObject);
        }
        instance = this;
    }
    
    private void Start()
    {
        CallbackHandler.instance.dismountPlayer += OndismountPlayer;
        CallbackHandler.instance.grappleFromWhale += OngrappleFromWhale;
        CallbackHandler.instance.grappleHitFromWhale += OngrappleHitFromWhale;
        CallbackHandler.instance.mountWhale += OnMountWhale;
        
        _whaleGrappleUIElement.SetActive(false);
    }

    private void OngrappleFromWhale(Transform grapplePos)
    {
        //If UI was active then the player has stopped aiming the grapple and the camera should be disabled and UI hidden
        _isWhaleGrappleUIActive = !_isWhaleGrappleUIActive;
        if (isActiveAndEnabled) playerOnWhale.GetComponentInChildren<GrappleScript>().enabled = true;
        _whaleGrappleUIElement.SetActive(_isWhaleGrappleUIActive);

        CameraManager.instance.SwitchCamera(_isWhaleGrappleUIActive
            ? CameraType.WhaleGrappleCamera
            : CameraType.WhaleCamera);
    }

    private void OngrappleHitFromWhale(Transform grapplePos)
    {
        player.transform.position = grapplePos.position;
        player.transform.rotation = Quaternion.LookRotation(grappleHook.transform.position.normalized, Vector3.up);


        player.SetActive(true);
        playerOnWhale.SetActive(false);

        CameraManager.instance.SwitchCamera(CameraType.PlayerCamera);

        _whaleGrappleUIElement.SetActive(false);

        grappleHook.connected = true;
        grappleHook.pc = player.transform;
    }

    private void OndismountPlayer(Transform dismountPosition)
    {
        player.transform.position = dismountPosition.position;
        player.transform.rotation = Quaternion.identity;
        player.SetActive(true);
        playerOnWhale.SetActive(false);

        CameraManager.instance.SwitchCamera(CameraType.PlayerCamera);
    }
    
    private void OnMountWhale()
    {
        player.SetActive(false);
        playerOnWhale.SetActive(true);

        CameraManager.instance.SwitchCamera(CameraType.WhaleCamera);
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.dismountPlayer -= OndismountPlayer;
        CallbackHandler.instance.grappleFromWhale -= OngrappleFromWhale;
        CallbackHandler.instance.grappleHitFromWhale -= OngrappleHitFromWhale;
    }

    public void DisablePlayerOnWhale()
    {
        playerOnWhale.SetActive(false);
    }
} 
