/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   OrbitScript.cs
  Description :   Handles movement and rotation for the Whale. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitScript : MonoBehaviour
{
    new public bool enabled;
    Vector3 objToIsland;
    Vector3 path;
    int orbitDirection = 1;
    //[HideInInspector]
    public GameObject orbit;
    float orbitDistance;
    public float rotSpeed;
    public bool atOrbit;
    public float dist;
    Rigidbody rb;
    [HideInInspector] public float currentDistance;

    #region Setup
    /// <summary>
    /// Description: Get Component References.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    #endregion Setup
    #region Callbacks
    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Start()
    {
        CallbackHandler.instance.setOrbit += SetOrbit;
        CallbackHandler.instance.setNewOrbitRef += SetNewOrbitRef;
        // testing purposes
        if (enabled)
        {
            rb.velocity = Vector3.forward;

            SetOrbit();// orbit);
            if (GetComponent<WhaleMovement>())
              GetComponent<WhaleMovement>().moveSpeed = 2.5f;
        }
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.setOrbit -= SetOrbit;
        CallbackHandler.instance.setNewOrbitRef -= SetNewOrbitRef;
    }
    #endregion Callbacks

    /// <summary>    
    /// Description: Handles path and rotation when in orbit (no control).
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void FixedUpdate()
    {
        if (enabled)
        {
            rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime);

            objToIsland = orbit.transform.position - transform.position;
            Vector3 islandToObj = transform.position - orbit.transform.position;

            Vector3 orbitPos = new Vector3(orbit.transform.position.x, 0, orbit.transform.position.z);
            Vector3 whalePos = new Vector3(transform.position.x, 0, transform.position.z);

            currentDistance = Vector3.Distance(orbitPos, whalePos);

            objToIsland *= orbitDistance / currentDistance;

            Vector3 desiredPos = orbit.transform.position + islandToObj * (orbitDistance / currentDistance);
            Vector3 vecToDesired = desiredPos - transform.position;

            vecToDesired = Vector3.Normalize(vecToDesired);

            float distanceToDesired = Vector3.Distance(transform.position, desiredPos);
            dist = distanceToDesired / orbitDistance;

            vecToDesired *= dist;

            path = Vector3.Cross(objToIsland, Vector3.up);

            Debug.DrawRay(transform.position, vecToDesired * 1000.0f, Color.blue);

            path.y = orbit.transform.position.y - transform.position.y;
            path = Vector3.Normalize(path);
            path = Vector3.Normalize(path + vecToDesired);

            Debug.DrawRay(transform.position, path * 1000.0f, Color.green);

            path = new Vector3(path.x * orbitDirection, path.y, path.z * orbitDirection);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(path), Time.deltaTime * rotSpeed));
        }
    }

    /// <summary>
    /// Description: Gets island reference to orbit.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_orbit">Object to Orbit</param>
    public void SetOrbit()//GameObject _orbit)
    {
        //if (!enabled)
        {
            //orbit = _orbit;
            orbitDistance = orbit.GetComponent<SphereCollider>().radius;
            enabled = true;
        }
    }

    public void SetNewOrbitRef(GameObject _ref)
    {
        orbit = _ref;
    }
}