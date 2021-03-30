﻿using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool shotGrapple = false;
    public bool enabled;
    // temp
    public Transform gunContainer;
    public GunMeshSwitch shootPoint;

    #region Setup
    // Local Variables
    public Transform camToShootFrom;
    GameObject grappleReticule;

    UnityEngine.UI.Image grapplePoint;
    PlayerMovement pm;
    Rigidbody rb;
    private Transform whaleGrapplePos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>()!=null? GetComponent<PlayerMovement>():null;
        shootPoint = GetComponentInChildren<GunMeshSwitch>();

        grapplePoint = (GrappleUI != null) ? 
            (GrappleUI.GetComponent<UnityEngine.UI.Image>()) : 
            (GetComponentInChildren<UnityEngine.UI.Image>());

        //grapplePoint = GetComponentInChildren<UnityEngine.UI.Image>();

        grappleReticule = grapplePoint.gameObject;
        
        whaleGrapplePos = GameObject.Find("GrapplePos").transform;
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

        if (!grapplingFromWhale)
        {
            VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleAim").MethodToCall.AddListener(GrappleAim);
            VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").MethodToCall.AddListener(Grapple);
            EventManager.StartListening("EnableGrapple", EnableGrapple);
        }
        else
        {
            VirtualInputs.GetInputListener(InputType.WHALE, "GrappleAim").MethodToCall.AddListener(GrappleAim);
            VirtualInputs.GetInputListener(InputType.WHALE, "Grapple").MethodToCall.AddListener(Grapple);
        }
    }
    #endregion Setup

    void EnableGrapple()
    {
        enabled = true;
    }

    void Grapple(InputState type)
    {
        if(!gameObject.activeInHierarchy) return;
        if (!enabled)
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
    void GrappleAim(InputState type)
    {
        if(!gameObject.activeInHierarchy) return;
        if (grapplingFromWhale)
        {
            CallbackHandler.instance.GrappleAim(whaleGrapplePos);
            enabled = type == InputState.KEYDOWN;
        }
        if (!enabled)
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!enabled)
            return;
        if (!grapplingFromWhale)
        {
            pm.enabled = !hook.connected;
        }

        if (hook.connected)
        {
            switch (grapplingFromWhale)
        {
            case true when shotGrapple:
                if(hook.connectedObj.layer == 10) return;
                CallbackHandler.instance.GrappleHitFromWhale(transform);
                gameObject.SetActive(false);
                return;
            case false:
            {
                Vector3 moveDir = Vector3.Normalize(hook.transform.position - transform.position) * pullSpeed;
                rb.AddForce(moveDir * TimeSlowDown.instance.timeScale, ForceMode.Acceleration);
                transform.LookAt(hook.transform);
                return;
            }
        }
        }

        if (!pm || pm.GLIDINGCheck())
            return;

        transform.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.eulerAngles.y, 0.0f));
    }

    public LayerMask raycastTargets;
    Vector3 RaycastToTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(camToShootFrom.transform.position, camToShootFrom.transform.forward, out hit, Mathf.Infinity, raycastTargets))
        {
            Debug.DrawRay(camToShootFrom.transform.position, camToShootFrom.transform.forward * hit.distance, Color.yellow);

            if (grappleableLayers == (grappleableLayers | (1 << hit.transform.gameObject.layer)))
            {
                grapplePoint.color = Color.red;
                return hit.point;
            }

            grapplePoint.color = Color.white;
            return hit.point;
        }

        grapplePoint.color = Color.white;
        return Vector3.zero;
    }

    float floatTimer;
    private void Update()
    {
        if (!enabled)
            return;

        // NOT SURE IF THIS IS BETTER OR WORSE
        //
        if (hook.connected)
            floatTimer = 0.0f;

        if (floatTimer > 0)
        {
            floatTimer -= Time.deltaTime;
            if (rb.velocity.y < 0)
                rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

            rb.useGravity = floatTimer > 0;
        }
        //
        // COMMENT OUT IF REQUIRED

        shootPoint.Loaded(!hook.InUse());
        if (!hook.InUse())
        {
            if (aim)
            {
                gunContainer.rotation = RaycastToTarget() == Vector3.zero ? Quaternion.LookRotation(camToShootFrom.transform.forward, camToShootFrom.transform.up) : Quaternion.LookRotation(RaycastToTarget() - transform.position);
            }
            else
            {
                gunContainer.rotation = Quaternion.Lerp(gunContainer.rotation, Quaternion.LookRotation(transform.forward, transform.up), Time.deltaTime);
            }
        }
        else
        {
            gunContainer.LookAt(hook.transform);
        }

        if (!aim)
            return;

        if (cachedShoot)
            FireHook();

        RaycastToTarget();
    }


    bool cachedShoot = false;
    public void FireHook()
    {
        if (!HookInUse() && (!pm.GLIDINGCheck() || grapplingFromWhale))
        {
            if (RaycastToTarget() != Vector3.zero)
            {
                Debug.Log(grapplingFromWhale);
                Debug.Log(shootPoint.name);
                
                hook.Fire(shootPoint.shootPoint, Vector3.Normalize(RaycastToTarget() - transform.position));
                shotGrapple = true;
                cachedShoot = false;
                ToggleAim(false);
                floatTimer = 1.0f;
                return;
            }

            hook.Fire(shootPoint.shootPoint, camToShootFrom.transform.forward);
            shotGrapple = true;
            cachedShoot = false;
            ToggleAim(false);
        }
        else if (AbleToRetract())
        {
            // Start retracting
            if(!grapplingFromWhale) hook.YeetPlayer(this.GetComponent<PlayerMovement>());
            hook.retracting = true;
            hook.connected = false;
            hook.manualRetract = true;

            if (aim)
            {
                hook.retracting = false;
                hook.connected = false;
                hook.manualRetract = false;

                hook.Fire(shootPoint.shootPoint, Vector3.Normalize(RaycastToTarget() - transform.position));
                shotGrapple = true;
                cachedShoot = false;
                ToggleAim(false);
                floatTimer = 1.0f;
                return;
                //cachedShoot = true;
            }
        }
    }

    void ToggleAim(bool _startAim)
    {
        if(grapplingFromWhale) 
            return;
        
        aim = _startAim;
        if (grappleReticule != null)
        {
            grappleReticule.SetActive(_startAim);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled)
            return;

        if (hook.connected && shotGrapple)
        {
            hook.connected = false;
            hook.retracting = true;
            if(!grapplingFromWhale) hook.YeetPlayer(this.GetComponent<PlayerMovement>());
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
