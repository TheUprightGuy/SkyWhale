using System.Collections;
using System.Collections.Generic;
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

    #region Setup
    // Local Variables
    Camera camToShootFrom;
    GameObject grappleReticule;

    UnityEngine.UI.Image grapplePoint;
    PlayerMovement pm;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        grapplePoint = (GrappleUI != null) ? 
            (GrappleUI.GetComponent<UnityEngine.UI.Image>()) : 
            (GetComponentInChildren<UnityEngine.UI.Image>());

        //grapplePoint = GetComponentInChildren<UnityEngine.UI.Image>();

        grappleReticule = grapplePoint.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        camToShootFrom = Camera.main;
        hook.grappleableLayers = grappleableLayers;
        ToggleAim(false);

        VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleAim").MethodToCall.AddListener(GrappleAim);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").MethodToCall.AddListener(Grapple);
    }
    #endregion Setup

    void Grapple(InputState type)
    {
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
        switch (type)
        {
            case InputState.KEYDOWN:

                ToggleAim(true);
                aim = true;
                TimeSlowDown.instance.SlowDown();
                break;
            case InputState.KEYHELD:
                break;
            case InputState.KEYUP:
                ToggleAim(false);
                aim = false;
                TimeSlowDown.instance.SpeedUp();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pm.enabled = !hook.connected;

        if (hook.connected)
        {
            if(grapplingFromWhale)
            {
                CallbackHandler.instance.GrappleHitFromWhale(transform);
                gameObject.SetActive(false);
                return;
            }

            Vector3 moveDir = Vector3.Normalize(hook.transform.position - transform.position) * pullSpeed;
            rb.AddForce(moveDir * TimeSlowDown.instance.timeScale, ForceMode.Acceleration);
            transform.LookAt(hook.transform);
            return;
        }

        if (pm.GLIDINGCheck())
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


    private void Update()
    {
        if (!aim)
            return;

        if (cachedShoot)
            FireHook();

        RaycastToTarget();
    }


    bool cachedShoot = false;
    public void FireHook()
    {
        if (!HookInUse() && !pm.GLIDINGCheck())
        {
            if (RaycastToTarget() != Vector3.zero)
            {
                hook.Fire(this.transform, Vector3.Normalize(RaycastToTarget() - transform.position));
                shotGrapple = true;
                cachedShoot = false;
                ToggleAim(false);
                return;
            }

            hook.Fire(this.transform, camToShootFrom.transform.forward);
            shotGrapple = true;
            cachedShoot = false;
            ToggleAim(false);
        }
        else if (AbleToRetract())
        {
            // Start retracting
            hook.YeetPlayer(this.transform);
            hook.retracting = true;
            hook.connected = false;
            hook.manualRetract = true;
            if (aim)
                cachedShoot = true;
        }
    }

    void ToggleAim(bool _startAim)
    {
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
