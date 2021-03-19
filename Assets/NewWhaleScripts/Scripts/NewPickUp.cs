using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPickUp : MonoBehaviour
{
    [Header("Required Setup Fields")]
    public float approachDistance;

    // Local References
    NewOrbitScript orbit;
    NewWhaleMovement whale;
    GameObject heightRef;
    Rigidbody rb;
    bool homing;
    Vector3 target;
    [HideInInspector] new public bool enabled;

    private void Awake()
    {
        orbit = GetComponent<NewOrbitScript>();
        whale = GetComponent<NewWhaleMovement>();
        rb = GetComponent<Rigidbody>();
    }

    #region Callbacks
    private void Start()
    {
        NewCallbackHandler.instance.setDestination += SetDestination;
    }
    private void OnDestroy()
    {
        NewCallbackHandler.instance.setDestination -= SetDestination;
    }
    #endregion Callbacks

    private void Update()
    {
        if (enabled)
        {
            // get direction to height reference
            Vector3 dir = target - transform.position;

            if (orbit.dist < 0.1f)
            {

                Debug.DrawRay(transform.position, dir, Color.black);
                // Check if hit anything between here and pickup position
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, GetComponent<CapsuleCollider>().radius * 2.0f, dir, out hit, Vector3.Distance(transform.position, target) + 3.0f))
                {
                    if (hit.transform.gameObject != orbit.orbit)
                    {
                        homing = false;
                        return;
                    }
                    else
                    {
                        homing = true;
                        orbit.enabled = false;
                    }
                }
            }

            if (homing)
            {
                float dist = Vector3.Distance(transform.position, target);
                whale.moveSpeed = 5.0f * Mathf.Clamp01(dist/approachDistance);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * orbit.rotSpeed));

                if (dist < 2.0f)
                {
                    enabled = false;
                    homing = false;
                }
            }
        }
    }

    public void SetDestination(GameObject _target)
    {
        heightRef = orbit.orbit;
        target = new Vector3(_target.transform.position.x, heightRef.transform.position.y + 20.0f, _target.transform.position.z);
        enabled = true;
    }
}
