using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamController : MonoBehaviour
{
    public LayerMask IslandLayers;

    private GameObject CurrentTrackingObj;
    public GameObject TrackingObj;
    public GameObject WhaleTrackingObj;

    private GameObject CurrentObj = null;
    private List<SphereCollider> IslandObjColliders = new List<SphereCollider>();


    private void Start()
    {
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in gos)
        {
            if ((IslandLayers == (IslandLayers | (1 << go.layer))) && go.GetComponent<SphereCollider>()) //Check if given object is within layer and has a spher collider to interface with
            {
                IslandObjColliders.Add(go.GetComponent<SphereCollider>());
            }
        }
    }

  

    // Update is called once per frame
    void Update()
    {
        CurrentTrackingObj = (TrackingObj != null) ? (TrackingObj) : (WhaleTrackingObj); //Set tracking object to whale if playercontainer is null
    }

    GameObject CheckOnIsland()
    {
        GameObject returnObj = null;

        foreach (var item in IslandObjColliders)
        {

            if (true)
            {

            }
        }
        return (returnObj);
    }
}
