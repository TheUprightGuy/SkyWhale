using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    [Header("Required Fields")]
    public NewGrappleHook hook;
    public LayerMask grappleableLayers;

    // Local Variables
    Camera camToShootFrom;
    GameObject grappleReticule;

    // Start is called before the first frame update
    void Start()
    {
        camToShootFrom = Camera.main;
        grappleReticule = GetComponentInChildren<UnityEngine.UI.Image>().gameObject;
        hook.grappleableLayers = grappleableLayers;
        ToggleAim(false);
    }

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

        if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
        {
            FireHook();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            hook.YeetPlayer(this.transform);
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveDir = Vector3.Normalize(hook.transform.position - transform.position) * 8.0f;

        if (hook.connected)
        {
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<Rigidbody>().AddForce(moveDir * TimeSlowDown.instance.timeScale, ForceMode.Acceleration);
        }
        else
        {
            GetComponent<PlayerMovement>().enabled = true;
        }
    }

    void FireHook()
    {
        if (!hook.connected && !hook.retracting && hook.flightTime <= 0.0f)
        {
            hook.Fire(this.transform, camToShootFrom.transform.forward);
        }
        else if ((hook.connected && !hook.retracting) || (hook.enabled && hook.flightTime > 0.0f))
        {
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

    public bool InUse()
    {
        return (hook.connected);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hook.connected)
        {
            hook.connected = false;
            hook.retracting = true;
        }
    }
}
