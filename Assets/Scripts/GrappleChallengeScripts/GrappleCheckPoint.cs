/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   GrappleStartPoint.cs
  Description :   Starts the grapple challenge. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class GrappleCheckPoint : MonoBehaviour
{
    #region Setup
    GrappleChallengeMaster gcm;
    private PlayerMovement pm;
    public bool whaleCheckpoint = false;
    /// <summary>
    /// Description: Get Local Components.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        gcm = transform.parent.GetComponentInParent<GrappleChallengeMaster>();
        if(!whaleCheckpoint) VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").MethodToCall.AddListener(Interact);
    }
    #endregion Setup
    
    /// <summary>
    /// Description: Sets player reference for grapple challenge master and updates last checkpoint. (After player interacts)
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 04/27/2021</br>
    /// </summary>
    private void Interact(InputState inputState)
    {
        if (!pm || gcm.LastCheckPoint == this) return;    //only proceed if checkpoint hasn't been set already and player movement reference is set
        gcm.pm = pm;
        gcm.LastCheckPoint = this;
        CallbackHandler.instance.UpdateClosestGrappleChallenge(gcm);
        AudioManager.instance.PlaySound("Checkpoint");
        Debug.Log("TriggeredCheckPoint");
        CallbackHandler.instance.ResetCheckpointDissolve();
        if (GetComponentInChildren<DissolveControl>())
            GetComponentInChildren<DissolveControl>().dissolve = false;
        //Ignore this checkpoint
        IgnoreCheckPoint();
    }
    
    #region Triggers
    /// <summary>
    /// Description: Gets reference to player to check if in range.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 08/06/2021 (By Jacob Gallagher)</br>
    /// </summary>
    /// <param name="other">Triggering Object</param>
    private void OnTriggerEnter(Collider other)
    {
        if (whaleCheckpoint)
        {
            if (!other.GetComponent<PlayerMovement>()) return;
            gcm.pm = other.GetComponent<PlayerMovement>();
            gcm.LastCheckPoint = this;
            CallbackHandler.instance.UpdateClosestGrappleChallenge(gcm);
            AudioManager.instance.PlaySound("Checkpoint");
            CallbackHandler.instance.ResetCheckpointDissolve();
            return;
        }
        var player = other.GetComponent<PlayerMovement>();
        if (!player) return;
        pm = player;
        if(gcm.LastCheckPoint == this) return;
        CallbackHandler.instance.CheckPointInRange(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if(whaleCheckpoint) return;
        var player = other.GetComponent<PlayerMovement>();
        if (!player) return;
        IgnoreCheckPoint();
    }

    private void IgnoreCheckPoint()
    {
        //Player has either left the checkpoints range or has set already set the checkpoint
        pm = null;
        CallbackHandler.instance.CheckPointOutOfRange();
    }

    #endregion Triggers
}
