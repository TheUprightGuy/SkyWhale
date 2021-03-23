using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGrappleHook : MonoBehaviour
{
    [Header("Debug Fields")]
    new public bool enabled;
    public bool connected;
    public bool manualRetract;
    public bool retracting;
    public float flightTime;

    [HideInInspector] public LayerMask grappleableLayers;

    #region Setup
    // Local References
    Rigidbody rb;
    MeshRenderer mr;
    LineRenderer lr;
    GameObject connectedObj;
    [HideInInspector] public Transform pc;
    SphereCollider sc;
    Vector3 cachedPos;
    Vector3 forceDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        lr = GetComponent<LineRenderer>();
        sc = GetComponent<SphereCollider>();
    }
    #endregion Setup

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

        if (connectedObj)
        {
            Vector3 shift = connectedObj.transform.position - cachedPos;
            cachedPos = connectedObj.transform.position;
            transform.position += shift;
        }

        UpdateLR();
    }

    void UpdateLR()
    {
        lr.positionCount = mr.enabled ? 2 : 0;
        if (!mr.enabled)
            return;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, pc.position);
    }

    private void FixedUpdate()
    {
        if (enabled)
        {
            if (!retracting)
            {
                sc.enabled = true;
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
                    connectedObj = null;
                }
            }
        }
        if (retracting)
        {
            connectedObj = null;
            sc.enabled = false;
            flightTime = 0.0f;
            
            forceDir = Vector3.Normalize(pc.position - transform.position) * (manualRetract ? 48.0f : 24.0f);
            rb.MovePosition(transform.position + forceDir * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale);

            if (Vector3.Distance(transform.position, pc.position) < (forceDir.magnitude * Time.fixedDeltaTime))
            {
                enabled = false;
                retracting = false;
                sc.enabled = true;
                connected = false;
                manualRetract = false;
                connectedObj = null;
            }
        }

        if (!pc)
            return;

        if (Vector3.Distance(transform.position, pc.position) < 0.3f && connected)
        {
            YeetPlayer(pc);
            retracting = true;
            manualRetract = false;
            connectedObj = null;
        }
    }

    public void YeetPlayer(Transform _player)
    {
        if (enabled || retracting)
            return;

        pc = _player;
        Rigidbody temp = pc.GetComponent<Rigidbody>();
        temp.AddForce(temp.velocity.magnitude * Vector3.Normalize((Vector3.Normalize(forceDir) + transform.up)), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        enabled = false;
        if (GrappleAbleCheck(collision.gameObject.layer))
        {
            connected = true;
            connectedObj = collision.gameObject;
            cachedPos = connectedObj.transform.position;
        }   
        else
        {
            connectedObj = null;
            retracting = true;
        }
    }

    public bool GrappleAbleCheck(int layer)
    {
        return grappleableLayers == (grappleableLayers | (1 << layer));
    }
}
