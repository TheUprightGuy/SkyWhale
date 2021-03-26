using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGrappleHook : MonoBehaviour
{
    [Header("Setup Fields")]
    public float retractSpeed = 24.0f;
    public float shootSpeed = 24.0f;
    public GameObject smokePrefab;

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
    //[HideInInspector]
    public Transform pc;
    SphereCollider sc;
    Vector3 cachedPos;
    Vector3 forceDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponentInChildren<MeshRenderer>();
        lr = GetComponent<LineRenderer>();
        sc = GetComponent<SphereCollider>();
    }
    #endregion Setup

    public void Fire(Transform _player, Vector3 _dir)
    {
        pc = _player;
        transform.position = _player.position;

        GameObject temp = Instantiate(smokePrefab, pc);
        Destroy(temp, 2.0f);

        transform.LookAt(transform.position + _dir);
        forceDir = _dir * shootSpeed;
        enabled = true;
        flightTime = 0.0f;
        rb.velocity = Vector3.zero;
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
        lr.SetPosition(1, pc.position);//.GetComponent<GrappleScript>().shootPoint.position);
    }

    private void FixedUpdate()
    {
        if (enabled)
        {
            if (!retracting)
            {
                sc.enabled = true;
                flightTime += Time.fixedDeltaTime * TimeSlowDown.instance.timeScale;

                rb.AddForce((forceDir / flightTime) * 10.0f * Time.fixedDeltaTime, ForceMode.Acceleration);

                if (flightTime > 2.0f)
                {
                    enabled = false;
                    retracting = true;
                    connectedObj = null;
                }
            }
        }
        if (retracting)
        {
            rb.velocity = Vector3.zero;

            connectedObj = null;
            sc.enabled = false;
            flightTime = 0.0f;
            
            forceDir = Vector3.Normalize(pc.position - transform.position) * (manualRetract ? retractSpeed * 2 : retractSpeed * 2);
            rb.MovePosition(transform.position + (manualRetract ? forceDir * Time.fixedDeltaTime : forceDir * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale));

            if (Vector3.Distance(transform.position, pc.position) < Mathf.Max((forceDir.magnitude * Time.fixedDeltaTime), 0.3f))
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
            YeetPlayer(pc.GetComponentInParent<PlayerMovement>());
            retracting = true;
            manualRetract = false;
            connectedObj = null;
        }
    }

    public void YeetPlayer(PlayerMovement _player)
    {
        if (enabled || retracting)
            return;

        Rigidbody temp = _player.GetComponent<Rigidbody>();
        temp.AddForce(temp.velocity.magnitude * Vector3.Normalize((Vector3.Normalize(forceDir) + transform.up)), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject temp = Instantiate(smokePrefab, transform.position, Quaternion.identity);
        Destroy(temp, 2.0f);

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

        rb.velocity = Vector3.zero;
    }

    public bool InUse()
    {
        return (enabled || retracting || connected || manualRetract);
    }

    public bool GrappleAbleCheck(int layer)
    {
        return grappleableLayers == (grappleableLayers | (1 << layer));
    }
}
