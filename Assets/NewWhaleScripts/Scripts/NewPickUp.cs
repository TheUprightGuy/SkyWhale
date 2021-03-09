using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPickUp : MonoBehaviour
{
    public GameObject orbit;
    public GameObject pickUpTarget;
    new public bool enabled;
    public bool homing;
    public Vector3 target;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetDestination();
        }

        if (enabled)
        {
            // get direction to height reference
            Vector3 dir = target - transform.position;

            if (GetComponent<NewOrbitScript>().dist < 0.1f)
            {
                // Check if hit anything between here and pickup position
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, GetComponent<CapsuleCollider>().radius, dir, out hit, Mathf.Infinity))
                {
                    homing = false;
                    return;
                }
                else
                {
                    homing = true;
                    GetComponent<NewOrbitScript>().enabled = false;
                }
            }

            if (homing)
            {
                GetComponent<NewWhaleMovement>().moveSpeed = 5.0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * GetComponent<NewOrbitScript>().rotSpeed);
            }

            if (Vector3.Distance(transform.position, target) < 2.0f)
            {
                enabled = false;
                homing = false;
                GetComponent<NewWhaleMovement>().moveSpeed = 0.0f;
            }
        }
    }

    public void SetDestination()
    {
        orbit = GetComponent<NewOrbitScript>().orbit;
        target = new Vector3(pickUpTarget.transform.position.x, orbit.transform.position.y + 20.0f, pickUpTarget.transform.position.z);
        enabled = true;
    }
}
