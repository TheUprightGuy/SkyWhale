using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGrappleScript : MonoBehaviour
{
    [Header("Dependencies")]
    public GameObject hook;
    public Camera camToShootFrom;
    public GameObject grappleReticule;

    [Header("Grapple")]
    public LayerMask grappleableLayers;

    // Start is called before the first frame update
    void Start()
    {
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
            hook.GetComponent<NewGrappleHook>().YeetPlayer(this.transform);
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        NewGrappleHook temp = hook.GetComponent<NewGrappleHook>();
        Vector3 moveDir = Vector3.Normalize(hook.transform.position - transform.position) * 8.0f;

        if (temp.connected)
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
        NewGrappleHook temp = hook.GetComponent<NewGrappleHook>();

        if (!temp.connected && !temp.retracting && temp.flightTime <= 0.0f)
        {
            temp.Fire(this.transform, camToShootFrom.transform.forward);
        }
        else if ((temp.connected && !temp.retracting) || (temp.enabled && temp.flightTime > 0.0f))
        {
            temp.YeetPlayer(this.transform);
            temp.retracting = true;
            temp.connected = false;
            temp.manualRetract = true;
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
        NewGrappleHook temp = hook.GetComponent<NewGrappleHook>();
        if (temp.connected)
        {
            temp.connected = false;
            temp.retracting = true;
        }
    }
}
