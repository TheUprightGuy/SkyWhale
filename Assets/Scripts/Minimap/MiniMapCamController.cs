using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamController : MonoBehaviour
{
    [Header("Dependencies")]
    public LayerMask IslandLayers;

    private GameObject CurrentTrackingObj => (TrackingObj.activeSelf) ? (TrackingObj) : (WhaleTrackingObj); //Set tracking object to whale if playercontainer is turned off
    public GameObject TrackingObj;
    public GameObject WhaleTrackingObj;

    private GameObject CurrentObj = null;
    private List<SphereCollider> IslandObjColliders = new List<SphereCollider>();

    private Camera thisCam;

    public GameObject MinimapIcon;
    public Vector3 IconOffset = Vector3.zero;
    [Header("MiniMap")]
    public float MiniMapZoom = 100.0f;
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
        thisCam = GetComponent<Camera>();
        MinimapIcon.SetActive(true); 
    }

  
    
    // Update is called once per frame
    void Update()
    {
        CurrentObj = CheckOnIsland(); //Update the current stood on island

        transform.forward = CurrentTrackingObj.transform.forward;
        transform.position = CurrentObj.transform.position - (transform.forward * MiniMapZoom);
        thisCam.orthographicSize = MiniMapZoom;

        if (MinimapIcon != null)
        {
            MinimapIcon.transform.position = CurrentTrackingObj.transform.position + IconOffset;
        }
    }

    GameObject CheckOnIsland()
    {
        GameObject returnObj = CurrentTrackingObj; //If no island is stood on, we want to track the map to the tracking obj

        foreach (var item in IslandObjColliders)
        {
            float distToObj = Vector3.Distance(item.gameObject.transform.position, CurrentTrackingObj.transform.position);
            if (distToObj < item.radius) //Check if the current tracking obj is within the radius of the sphere collider
            {
                return (item.gameObject);
            }
        }
        return (returnObj);
    }
}
