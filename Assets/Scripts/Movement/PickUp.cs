/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   PickUp.cs
  Description :   Handles the homing aspect of the whale to pickup the player. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [Header("Required Setup Fields")]
    public float approachDistance;

    // Local References
    OrbitScript orbit;
    WhaleMovement whale;
    GameObject heightRef;
    Rigidbody rb;
    bool homing;
    Vector3 target;
    [HideInInspector] new public bool enabled;

    public Transform[] cinematicPoints;

    #region Setup
    /// <summary>
    /// Description: Gets component references.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        orbit = GetComponent<OrbitScript>();
        whale = GetComponent<WhaleMovement>();
        rb = GetComponent<Rigidbody>();
    }
    #endregion Setup
    #region Callbacks
    private void Start()
    {
        CallbackHandler.instance.setDestination += SetDestination;

        GoToPoint(cinematicPoints[tracking]);
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.setDestination -= SetDestination;
    }
    #endregion Callbacks

    /// <summary>
    /// Description: Handles pathfinding and rotation for the whale during pickup sequence.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Update()
    {
        if (enabled)
        {
            // get direction to height reference
            Vector3 dir = target - transform.position;

            if (cinematic)
            {
                float dist = Vector3.Distance(transform.position, target);
                whale.moveSpeed = 5.0f * Mathf.Clamp01(dist / approachDistance);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * orbit.rotSpeed));

                if (dist < 20.0f)
                {

                    if (cinematic)
                    {
                        if ((tracking < cinematicPoints.Length - 1))
                        {
                            tracking++;
                            GoToPoint(cinematicPoints[tracking]);
                        }
                        else
                        {
                            orbit.SetOrbit(cinematicPoints[cinematicPoints.Length - 1].gameObject);
                            //homing = true;
                        }
                    }
                }
                return;
            }


            // If at orbit, proceed to pickup
            if (orbit.dist < 0.1f)
            {
                Debug.DrawRay(transform.position, dir, Color.black);
                // Check if hit anything between here and pickup position
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, GetComponent<CapsuleCollider>().radius * 2.0f, dir, out hit, Vector3.Distance(transform.position, target) + 3.0f))
                {
                    // Invalid Path
                    if (hit.transform.gameObject != orbit.orbit)
                    {
                        homing = false;
                        return;
                    }
                    // Valid path
                    else
                    {
                        homing = true;
                        orbit.enabled = false;
                    }
                }
            }

            // Has a direct path to above the player - start homing
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

    /// <summary>
    /// Description: Sets destination to above the target.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_target">Player to collect</param>
    public void SetDestination(GameObject _target)
    {
        heightRef = orbit.orbit;
        target = new Vector3(_target.transform.position.x, heightRef.transform.position.y + 20.0f, _target.transform.position.z);
        enabled = true;
    }

    bool cinematic;
    int tracking = 0;
    public void GoToPoint(Transform _tar)
    {
        target = _tar.position;
        enabled = true;
        homing = true;
        cinematic = true;
    }
}
