using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class MiniMapCamController : MonoBehaviour
{
    [Header("Dependencies")]
    public LayerMask IslandLayers;
  
    private GameObject CurrentTrackingObj => (TrackingObj.activeSelf) ? (TrackingObj) : (WhaleTrackingObj); //Set tracking object to whale if playercontainer is turned off
    private GameObject IconCurrentTrackingObj => (IconTrackingObj.activeSelf) ? (IconTrackingObj) : (IconWhaleTrackingObj); //Set tracking object to whale if playercontainer is turned off
    [Space]
    public GameObject TrackingObj;
    public GameObject WhaleTrackingObj;

    [Space]
    public GameObject IconTrackingObj;
    public GameObject IconWhaleTrackingObj;

    private GameObject CurrentObj = null;
    private List<SphereCollider> IslandObjColliders = new List<SphereCollider>();

    private Camera thisCam;

    public GameObject MiniMapUI;
    public GameObject FullMapUI;
    [Space]
    public GameObject MinimapIcon;
    public GameObject ObjectiveIcon;
    public Vector3 IconOffset = Vector3.zero;
    [Header("MiniMap")]
    public float DefaultMapZoom = 175.0f;

    public float ZoomSteps = 25.0f;
    public float ClosestZoom = 10.0f;
    public float FurthestZoom = 300.0f;

    bool doWeHaveFogInScene;
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
        MiniMapUI.SetActive(true);
        FullMapUI.SetActive(false);

        VirtualInputs.GetInputListener(InputType.PLAYER, "Map").MethodToCall.AddListener(ToggleFullMap);
        VirtualInputs.GetInputListener(InputType.PLAYER, "MapPlus").MethodToCall.AddListener(MapPlus);
        VirtualInputs.GetInputListener(InputType.PLAYER, "MapMinus").MethodToCall.AddListener(MapMinus);

        doWeHaveFogInScene = RenderSettings.fog;
        //RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        //RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    //public void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    //{
    //    if (camera.gameObject != this.gameObject)
    //    {
    //        return;
    //    }
    //    RenderSettings.fog = false;
    //}
    //public void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    //{
    //    if (camera.gameObject != this.gameObject)
    //    {
    //        return;
    //    }
    //    RenderSettings.fog = /*doWeHaveFogInScene*/false;
    //}

    // Update is called once per frame
    void Update()
    {
        CurrentObj = CheckOnIsland(); //Update the current stood on island

        transform.forward = CurrentTrackingObj.transform.forward;
        transform.position = CurrentObj.transform.position - (transform.forward * DefaultMapZoom);
        thisCam.orthographicSize = DefaultMapZoom;
        Vector3 Objectiveloc = QuestManager.instance.activeQuests[0].objectives[QuestManager.instance.activeQuests[0].index].location;
        ObjectiveIcon.SetActive(!(Objectiveloc == Vector3.zero));
        ObjectiveIcon.transform.position = Objectiveloc + IconOffset;
        if (MinimapIcon != null)
        {
            MinimapIcon.transform.position = IconCurrentTrackingObj.transform.position + IconOffset;
            MinimapIcon.transform.forward = IconCurrentTrackingObj.transform.forward;
        }
    }

    GameObject CheckOnIsland()
    {
        GameObject returnObj = CurrentTrackingObj; //If no island is stood on, we want to track the map to the tracking obj

        foreach (var item in IslandObjColliders)
        {
            float distToObj = Vector3.Distance(item.gameObject.transform.position, CurrentTrackingObj.transform.position);
            if (distToObj < (item.radius * item.gameObject.transform.localScale.magnitude)) //Check if the current tracking obj is within the radius of the sphere collider, account for scale
            {
                return (item.gameObject);
            }
        }
        return (returnObj);
    }


    void ToggleFullMap(InputState inputState)
    {
        bool miniActive = MiniMapUI.activeSelf;

        MiniMapUI.SetActive(!miniActive);
        FullMapUI.SetActive(miniActive);
    }

    void MapPlus(InputState inputState)
    {
        DefaultMapZoom = Mathf.Clamp(DefaultMapZoom - ZoomSteps, ClosestZoom, FurthestZoom);
    }

    void MapMinus(InputState inputState)
    {
        DefaultMapZoom = Mathf.Clamp(DefaultMapZoom + ZoomSteps, ClosestZoom, FurthestZoom);
    }
}
