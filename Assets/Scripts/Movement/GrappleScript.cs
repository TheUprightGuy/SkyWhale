/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   GrappleScript.cs
  Description :   Handles the character movement side of the grapple hook. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
  
  Also slightly worked on by Jacob Gallagher when first working on dismount by grapple.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Cinemachine;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    [Header("Required Fields")]
    public NewGrappleHook hook;
    public GameObject GrappleUI;
    public LayerMask grappleableLayers;
    public float pullSpeed = 8.0f;

    public bool grapplingFromWhale = false;
    // temp
    public Transform gunContainer;
    public GunMeshSwitch shootPoint;
    new public bool enabled;
    bool pause;

    #region Setup
    // Local Variables
    public Transform camToShootFrom;
    GameObject grappleReticule;
    private const float MinimumDistanceToGrappleableTarget = 1.5f;

    UnityEngine.UI.Image grapplePoint;
    PlayerMovement pm;
    Rigidbody rb;

    /// <summary>
    /// Description: Get Component References.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>()!=null? GetComponent<PlayerMovement>():null;
        shootPoint = GetComponentInChildren<GunMeshSwitch>();

        grapplePoint = (GrappleUI != null) ? 
            (GrappleUI.GetComponent<UnityEngine.UI.Image>()) : 
            (GetComponentInChildren<UnityEngine.UI.Image>());

        grappleReticule = grapplePoint.gameObject;
    }

    /// <summary>
    /// Description: Sets inputs and Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void Start()
    {
        // Setup Hook
        hook.grappleableLayers = grappleableLayers;
        ToggleAim(false);

        // Input Management
        VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleAim").MethodToCall.AddListener(GrappleAim);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").MethodToCall.AddListener(Grapple);

        // Trigger to enable Grapple Tool
        EventManager.StartListening("EnableGrapple", EnableGrapple);

        // Callback to swap between Whale and Player
        EntityManager.instance.toggleControl += ToggleGrapple;
        CallbackHandler.instance.pause += Pause;
    }
    private void OnDestroy()
    {
        // End Callback
        EntityManager.instance.toggleControl -= ToggleGrapple;
        CallbackHandler.instance.pause -= Pause;
    }

    /// <summary>
    /// Description: Pause callback.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_pause">Pause State</param>
    void Pause(bool _pause)
    {
        pause = _pause;
    }
    #endregion Setup

    /// <summary>
    /// Description: Event Trigger to enable grapple hook.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void EnableGrapple()
    {
        enabled = true;
    }

    /// <summary>
    /// Description: Handles Grapple Shoot/Retract.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="type">Input Type (Down/Held/Release)</param>
    void Grapple(InputState type)
    {
        if (!enabled || pause)
            return;

        switch (type)
        {
            case InputState.KEYDOWN:
                    if (AbleToRetract() || aim)
                        FireHook();
                break;
            case InputState.KEYHELD:
                break;
            case InputState.KEYUP:
                break;
            default:
                break;
        }
    }


    bool aim;
    /// <summary>
    /// Description: Toggles ADS with the Grapple.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="type">Input Type (Down/Held/Release)</param>
    void GrappleAim(InputState type)
    {
        if (!enabled || pause)
            return;

        switch (type)
        {
            case InputState.KEYDOWN:
                ToggleAim(true);
                TimeSlowDown.instance.SlowDown();
                break;
            case InputState.KEYHELD:
                break;
            case InputState.KEYUP:
                ToggleAim(false);
                TimeSlowDown.instance.SpeedUp();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Description: Handles forces applied to player while grappling.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void FixedUpdate()
    {
        // Check if in use
        if (!enabled || pause)
            return;

        // Ensure a player is referenced (safety check)
        if (pm)       
            pm.haveControl = !hook.connected;

        if (hook.connected)
        {
            // Check as used by both mc and whale grapple
            if (!grapplingFromWhale)
            {
                Vector3 moveDir = Vector3.Normalize(hook.transform.position - transform.position) * pullSpeed;
                rb.AddForce(moveDir, ForceMode.Acceleration);
                transform.LookAt(hook.transform);
                return;
            }
        }

        // Safety check before applying rotation
        if (!pm || pm.GLIDINGCheck())
            return;

        transform.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.eulerAngles.y, 0.0f));
    }

    public LayerMask raycastTargets;
    /// <summary>
    /// Description: Checks if target is grappleable.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <returns>Reference for Reticule</returns>
    Vector3 RaycastToTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(camToShootFrom.transform.position, camToShootFrom.transform.forward, out hit, Mathf.Infinity, raycastTargets, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(camToShootFrom.transform.position, camToShootFrom.transform.forward * hit.distance, Color.yellow);

            if (grappleableLayers == (grappleableLayers | (1 << hit.transform.gameObject.layer)))
            {
                grapplePoint.color = Color.red;

                if (pm && pm.playerState == PlayerMovement.PlayerStates.IDLE)
                    CallbackHandler.instance.DisplayHotkey(InputType.PLAYER, "Grapple", "");

                return hit.point;
            }

            grapplePoint.color = Color.white;
            CallbackHandler.instance.HideHotkey("Grapple");
            return hit.point;
        }

        grapplePoint.color = Color.white;
        CallbackHandler.instance.HideHotkey("Grapple");
        return Vector3.zero;
    }

    float floatTimer;
    /// <summary>
    /// Description: Handles grapple gun rotation, gravity and loaded mesh states.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Update()
    {
        // Check available to use
        if (!enabled || pause)
            return;

        if (hook.connected)
            floatTimer = 0.0f;

        // Safety check as script is used for mc and whale grapple hook
        if (floatTimer > 0 && !grapplingFromWhale)
        {
            floatTimer -= Time.deltaTime;
            if (rb.velocity.y < 0)
                rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

            rb.useGravity = floatTimer > 0;
        }

        // Set mesh based on loaded state
        shootPoint.Loaded(!hook.InUse());

        // Grapple Loaded
        if (!hook.InUse())
        {
            // Rotate Gun to face aim direction while ADS
            if (aim)
            {
                if (Vector3.Distance(RaycastToTarget(), transform.position) < MinimumDistanceToGrappleableTarget)
                {
                    Debug.Log("Too close to wall");
                    gunContainer.rotation = Quaternion.LookRotation(camToShootFrom.transform.forward, camToShootFrom.transform.up);
                }
                else
                {
                    gunContainer.rotation = RaycastToTarget() == Vector3.zero ? Quaternion.LookRotation(camToShootFrom.transform.forward, camToShootFrom.transform.up) : Quaternion.LookRotation(RaycastToTarget() - transform.position);   
                }
            }
            // Rotate Gun to MCs forward direcction
            else
            {
                gunContainer.rotation = Quaternion.Lerp(gunContainer.rotation, Quaternion.LookRotation(transform.forward, transform.up), Time.deltaTime);
            }
        }
        // In flight
        else
        {
            gunContainer.LookAt(hook.transform);
        }

        if (!aim)
            return;

        // Firehook if player clicked while already attached
        if (cachedShoot)
            FireHook();

        RaycastToTarget();
    }


    bool cachedShoot = false;
    /// <summary>
    /// Description: Fires/Retracts the Grapple Hook.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void FireHook()
    {
        if (!active || pause)
            return;

        //Fire hook without aiming while currently grappling (Author: Jacob Gallagher) (Also removed a few lines to enable fire hook to be called in this case)
        /*if (IsConnected() && !aim)
        {
            hook.retracting = false;
            hook.connected = false;
            hook.manualRetract = false;
            hook.Fire(shootPoint.shootPoint, Vector3.Normalize(RaycastToTarget() - transform.position));
            cachedShoot = false;
            floatTimer = 1.0f;
            return;
        }*/
        
        if(!AbleToRetract() && !aim) return;

        CallbackHandler.instance.HideHotkey("Grapple");

        // Available To Use
        if (!HookInUse() && (pm ? !pm.GLIDINGCheck() : grapplingFromWhale))
        {
            if (RaycastToTarget() != Vector3.zero)
            {
                if (Vector3.Distance(RaycastToTarget(), transform.position) < MinimumDistanceToGrappleableTarget)
                {
                    AudioManager.instance.PlaySound("GrappleFail");
                    return;
                }
                hook.Fire(shootPoint.shootPoint, Vector3.Normalize(RaycastToTarget() - transform.position));
                cachedShoot = false;
                ToggleAim(false);
                floatTimer = 1.0f;
                return;
            }

            hook.Fire(shootPoint.shootPoint, camToShootFrom.transform.forward);
            cachedShoot = false;
            ToggleAim(false);
        }
        // Availabe to Retract
        else if (AbleToRetract())
        {
            // Start retracting

            hook.retracting = true;
            hook.connected = false;
            hook.manualRetract = true;

            if (aim)
            {
                hook.retracting = false;
                hook.connected = false;
                hook.manualRetract = false;

                hook.Fire(shootPoint.shootPoint, Vector3.Normalize(RaycastToTarget() - transform.position));
                cachedShoot = false;
                ToggleAim(false);
                floatTimer = 1.0f;
                return;
            }
        }
    }

    public void YeetPlayer()
    {
        if (!grapplingFromWhale)
            hook.YeetPlayer(this.GetComponent<PlayerMovement>());
    }

    public bool active;
    /// <summary>
    /// Description: Toggles whether Grapple is able to be used or not.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_toggle">Toggle Grapple On/Off</param>
    public void ToggleGrapple(bool _toggle)
    {
        active = grapplingFromWhale ? !_toggle : _toggle;
    }

    /// <summary>
    /// Description: Toggles Aim Reticule.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_startAim">ADS</param>
    void ToggleAim(bool _startAim)
    {
        CallbackHandler.instance.HideHotkey("GrappleAim");

        // Toggle Reticule
        aim = _startAim;
        if (grappleReticule != null)
        {
            grappleReticule.SetActive(_startAim);
        }

        // Switch Whale Cameras on right Click
        if (grapplingFromWhale && active)
        {
            CameraManager.instance.SwitchCamera(_startAim ? CameraType.WhaleGrappleCamera : CameraType.WhaleCamera);
            return;
        }
    }

    /// <summary>
    /// Description: Retracts hook upon player collision.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="collision">Colliding Object</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled || pause)
            return;

        if (hook.connected)
        {
            hook.connected = false;
            hook.retracting = true;

            if(!grapplingFromWhale) 
                hook.YeetPlayer(this.GetComponent<PlayerMovement>());
        }
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
