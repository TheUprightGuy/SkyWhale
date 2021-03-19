using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGrappleHook : MonoBehaviour
{
    Rigidbody rb;
    Vector3 hitPos;
    public Vector3 forceDir;
    //[HideInInspector]
    new public bool enabled;
    public bool retracting;
    public float flightTime;
    public LayerMask grappleableLayers;
    public Transform pc;
    public bool connected;
    public bool manualRetract;
    MeshRenderer mr;
    LineRenderer lr;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        lr = GetComponent<LineRenderer>();
    }

    public void Fire(Transform _player, Vector3 _dir)
    {
        pc = _player;
        transform.position = _player.position;
        forceDir = _dir * 24.0f;
        enabled = true;
        flightTime = 0.0f;
        TimeSlowDown.instance.SpeedUp();
    }

    private void Update()
    {
        mr.enabled = enabled || retracting || connected;

        UpdateLR();
    }

    void UpdateLR()
    {
        if (mr.enabled)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, pc.position);
        }
        else
        {
            lr.positionCount = 0;
        }
    }

    private void FixedUpdate()
    {
        if (enabled)
        {
            if (!retracting)
            {
                GetComponent<SphereCollider>().enabled = true;
                flightTime += Time.fixedDeltaTime * TimeSlowDown.instance.timeScale;
                if (flightTime > 2.0f)
                {
                    forceDir += (transform.up * -Time.fixedDeltaTime * TimeSlowDown.instance.timeScale);
                }
                rb.MovePosition(transform.position + forceDir * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale);

                if (flightTime > 4.0f)
                {
                    enabled = false;
                    retracting = true;
                }
            }
        }
        if (retracting)
        {
            GetComponent<SphereCollider>().enabled = false;
            flightTime = 0.0f;
            
            forceDir = Vector3.Normalize(pc.position - transform.position) * (manualRetract ? 48.0f : 24.0f);
            rb.MovePosition(transform.position + forceDir * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale);

            if (Vector3.Distance(transform.position, pc.position) < (forceDir.magnitude * Time.fixedDeltaTime))
            {
                enabled = false;
                retracting = false;
                GetComponent<SphereCollider>().enabled = true;
                connected = false;
                manualRetract = false;
            }
        }

        if (!pc)
            return;

        if (Vector3.Distance(transform.position, pc.position) < 0.2f && connected)
        {
            YeetPlayer(pc);
            retracting = true;
            manualRetract = false;
        }
    }

    public void YeetPlayer(Transform _player)
    {
        if (enabled || retracting)
            return;

        pc = _player;
        pc.GetComponent<Rigidbody>().AddForce(pc.GetComponent<Rigidbody>().velocity.magnitude * Vector3.Normalize((Vector3.Normalize(forceDir) + transform.up)), ForceMode.Impulse);
    }

    void Retract()
    {
        GetComponent<SphereCollider>().enabled = false;
    }

    // Hit Something
    private void OnCollisionEnter(Collision collision)
    {
        enabled = false;
        // Check if we can Grabble to It
        if (GrappleAbleCheck(collision.gameObject.layer))
        {
            Debug.Log("Correct Layer");
            connected = true;
            // need to fix the hook to this point
        }   
        // Retract
        else
        {
            retracting = true;
            // Start Retracting
        }
    }

    public bool GrappleAbleCheck(int layer)
    {
        return grappleableLayers == (grappleableLayers | (1 << layer));
    }
}
