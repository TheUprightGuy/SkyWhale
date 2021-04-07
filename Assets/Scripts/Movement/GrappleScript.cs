using System;
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
    // temp
    public Transform gunContainer;
    public GunMeshSwitch shootPoint;
    new public bool enabled;
    bool pause;

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

        grappleReticule = grapplePoint.gameObject;
    }

    // Start is called before the first frame update
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

    void Pause(bool _pause)
    {
        pause = _pause;
    }
    #endregion Setup



// Function to run on trigger
    void EnableGrapple()
    {
        enabled = true;
    }

    void Grapple(InputState type)
    {
        if (!enabled || pause)
            return;

        switch (type)
        {
            case InputState.KEYDOWN:
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!enabled || pause)
            return;

        if (pm)       
            pm.haveControl = !hook.connected;

        if (hook.connected)
        {
            if (!grapplingFromWhale)
            {
                Vector3 moveDir = Vector3.Normalize(hook.transform.position - transform.position) * pullSpeed;
                rb.AddForce(moveDir * TimeSlowDown.instance.timeScale, ForceMode.Acceleration);
                transform.LookAt(hook.transform);
                return;
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
        if (!enabled || pause)
            return;

        if (hook.connected)
            floatTimer = 0.0f;

        if (floatTimer > 0 && !grapplingFromWhale)
        {
            floatTimer -= Time.deltaTime;
            if (rb.velocity.y < 0)
                rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

            rb.useGravity = floatTimer > 0;
        }

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
        if (!active || pause)
            return;

        if (IsConnected() && !aim)
        {
            hook.retracting = false;
            hook.connected = false;
            hook.manualRetract = false;
            hook.Fire(shootPoint.shootPoint, Vector3.Normalize(RaycastToTarget() - transform.position));
            cachedShoot = false;
            floatTimer = 1.0f;
            return;
        }
        
        if(!AbleToRetract() && !aim) return;
        
        if (!HookInUse() && (pm ? !pm.GLIDINGCheck() : grapplingFromWhale))
        {
            if (RaycastToTarget() != Vector3.zero)
            {
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
        // This can probably be cleaned up
        else if (AbleToRetract())
        {
            // Start retracting
            //if(!grapplingFromWhale) 
                //hook.YeetPlayer(this.GetComponent<PlayerMovement>());

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

    public bool active;
    public void ToggleGrapple(bool _toggle)
    {
        active = grapplingFromWhale ? !_toggle : _toggle;
    }


    void ToggleAim(bool _startAim)
    {
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
