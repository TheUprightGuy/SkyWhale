using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitScript : MonoBehaviour
{
    Vector3 objToIsland;
    Vector3 path;
    Quaternion lookRot;
    WhaleInfo whaleInfo;
    //[HideInInspector] 
    public GameObject leashObject = null;
    [HideInInspector] public MeshCollider islandBase;
    [HideInInspector] public float initialSlerp = 0.0f;
    [HideInInspector] public int orbitDirection = 1;

    #region Setup
    private void Start()
    {
        whaleInfo = CallbackHandler.instance.whaleInfo;
        WhaleHandler.instance.shiftWhale += ShiftWhale;
    }

    private void OnDestroy()
    {
        WhaleHandler.instance.shiftWhale -= ShiftWhale;
    }
    #endregion Setup

    public void SetOrbitDirection()
    {
        initialSlerp = 2.1f;
        // Direction from pos to island
        Vector3 dir = (leashObject.transform.position - transform.position);
        Vector3 path = Vector3.Normalize(Vector3.Cross(dir, Vector3.up));

        if (Vector3.Dot(transform.forward, path) >= 0.0f)
        {
            orbitDirection = 1;
        }
        else
        {
            orbitDirection = -1;
        }

        gameObject.GetComponent<HomingScript>().pickupHeight = leashObject.GetComponent<IslandSlowDown>().colliderHeight;
    }

    public void ShiftWhale()
    {
        float rad = leashObject.GetComponent<SphereCollider>().radius / 2;
        transform.position += new Vector3(rad * orbitDirection, 0, rad * orbitDirection);
    }

    // Update is called once per frame
    void Update()
    {
        if (whaleInfo.leashed && leashObject)
        {
            objToIsland = leashObject.transform.position - transform.position;
            path = Vector3.Normalize(Vector3.Cross(objToIsland, Vector3.up));
            path = new Vector3(path.x * orbitDirection, 0, path.z * orbitDirection);

            if (initialSlerp > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(path), Time.deltaTime);
                initialSlerp -= Time.deltaTime;
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(path), Time.deltaTime * 5);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!leashObject)
        {
            whaleInfo.leashed = false;
        }
    }
}
