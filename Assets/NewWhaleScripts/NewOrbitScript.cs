using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOrbitScript : MonoBehaviour
{
    new public bool enabled;
    Vector3 objToIsland;
    Vector3 path;
    int orbitDirection = 1;
    [HideInInspector] public GameObject orbit;
    float orbitDistance;
    public float rotSpeed;

    [HideInInspector] public float currentDistance;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enabled)
        {
            objToIsland = orbit.transform.position - transform.position;
            Vector3 islandToObj = transform.position - orbit.transform.position;

            Vector3 orbitPos = new Vector3(orbit.transform.position.x, 0, orbit.transform.position.z);
            Vector3 whalePos = new Vector3(transform.position.x, 0, transform.position.z);

            currentDistance = Vector3.Distance(orbitPos, whalePos);

            objToIsland *= orbitDistance / currentDistance;

            Vector3 desiredPos = orbit.transform.position + islandToObj * orbitDistance / currentDistance;
            Vector3 vecToDesired = desiredPos - transform.position;

            vecToDesired = Vector3.Normalize(vecToDesired);

            float distanceToDesired = Vector3.Distance(transform.position, desiredPos);
            float dist = distanceToDesired / orbitDistance;

            vecToDesired *= dist;

            path = Vector3.Cross(objToIsland, Vector3.up);

            Debug.DrawRay(transform.position, vecToDesired * 1000.0f, Color.blue);        

            path.y = orbit.transform.position.y - transform.position.y;
            path = Vector3.Normalize(path);
            path = Vector3.Normalize(path + vecToDesired);

            Debug.DrawRay(transform.position, path * 1000.0f, Color.green);

            path = new Vector3(path.x * orbitDirection, path.y, path.z * orbitDirection);
            //transform.rotation = Quaternion.LookRotation(path);// Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(path), Time.deltaTime * 5);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(path), Time.deltaTime * rotSpeed);
        }
    }

    public void SetupOrbit(GameObject _orbit)
    {
        if (!enabled)
        {
            orbit = _orbit;
            orbitDistance = orbit.GetComponent<SphereCollider>().radius;
        }
    }
}