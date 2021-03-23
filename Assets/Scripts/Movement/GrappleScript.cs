using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    [Header("Required Fields")]
    public NewGrappleHook hook;
    public LayerMask grappleableLayers;
    public float pullSpeed = 8.0f;

    public bool grapplingFromWhale = false;
    public bool shotGrapple = false;

    #region Setup
    // Local Variables
    Camera camToShootFrom;
    GameObject grappleReticule;
    PlayerMovement pm;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        grappleReticule = GetComponentInChildren<UnityEngine.UI.Image>().gameObject;
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

    public void FireHook()
    {
        if (!HookInUse() && !pm.GLIDINGCheck())
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
