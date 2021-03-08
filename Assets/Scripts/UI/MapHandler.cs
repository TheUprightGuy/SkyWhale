using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class ObjectiveTrackers
{
    public string ObjectiveName;
    public Transform ObjectiveTransformInWorld;
    public GameObject ObjectivePointerOnRing;
    public GameObject ObjectiveMarkerOnMap;
}
public class MapHandler : MonoBehaviour
{
    #region Singleton
    public static MapHandler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Map Handler Exists!");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion Singleton

    [Header("World Corners")]
    public Vector2 WorldTopLeft = Vector3.zero;
    public Vector2 WorldBottomRight = Vector2.one;

    private Vector2 percentPos = Vector2.zero;

    [Header("MiniMap Tracker")]
    public Transform trackingTransform;

    [Header("Pointer Fade Settings")]
    public float MiniMapWorldSize = 100.0f;
    public float fadeStartDistance = 80.0f;

    private int ActiveObjectiveIndex = 0;
    [Space(10)]
    public ObjectiveTrackers[] Objectives;

    void Start()
    {
        for (int i = 0; i < Objectives.Length; i++)
        {
            if (i != ActiveObjectiveIndex)
            {
                Objectives[i].ObjectiveMarkerOnMap.active = false;
                Objectives[i].ObjectivePointerOnRing.active = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        float lengthX = WorldBottomRight.x - WorldTopLeft.x;
        float lengthY = WorldTopLeft.y - WorldBottomRight.y;

        float transformLengthX = trackingTransform.position.x - WorldTopLeft.x;
        float transformLengthY = trackingTransform.position.z - WorldBottomRight.y;

        float percentX = transformLengthX / lengthX;
        float percentY = transformLengthY / lengthY;

        percentPos.x = percentX;
        percentPos.y = percentY;

        Vector3 startPos = new Vector3(GetComponent<RectTransform>().rect.width / 2, (GetComponent<RectTransform>().rect.height / 2));
        Vector3 trackingPos = new Vector3(GetComponent<RectTransform>().rect.width * percentX, GetComponent<RectTransform>().rect.height * percentY); ;
        Vector3 pos = new Vector3(startPos.x - trackingPos.x, (startPos.y - trackingPos.y), 0.0f);
        GetComponent<RectTransform>().localPosition = pos;


        float dist = Vector3.Distance(Objectives[ActiveObjectiveIndex].ObjectiveTransformInWorld.position, trackingTransform.position);
        Objectives[ActiveObjectiveIndex].ObjectivePointerOnRing.active = (dist) > (fadeStartDistance);

        if (Objectives[ActiveObjectiveIndex].ObjectivePointerOnRing.GetComponent<Image>())
        {
            Color newCol = Objectives[ActiveObjectiveIndex].ObjectivePointerOnRing.GetComponent<Image>().color;
            newCol.a = (dist - fadeStartDistance) / (MiniMapWorldSize - fadeStartDistance);
            Objectives[ActiveObjectiveIndex].ObjectivePointerOnRing.GetComponent<Image>().color = newCol;
        }

    }

    /// <summary>
    /// Sets the current active objective.
    /// </summary>
    /// <param name="ObjectiveIndex">Index for objective to set active in the objectives array.</param>
    /// <returns>Returns false if an invalid Index is passed, and true if objective setting is succesful.</returns>
    public bool SetActiveObjective(int ObjectiveIndex)
    {
        if (ObjectiveIndex > 0 && ObjectiveIndex < Objectives.Length)
        {
            //Deactivate the previous marks
            Objectives[ActiveObjectiveIndex].ObjectiveMarkerOnMap.active = false;
            Objectives[ActiveObjectiveIndex].ObjectivePointerOnRing.active = false;

            ActiveObjectiveIndex = ObjectiveIndex;

            Objectives[ActiveObjectiveIndex].ObjectiveMarkerOnMap.active = true;
            Objectives[ActiveObjectiveIndex].ObjectivePointerOnRing.active = true;

            Transform GoalTransform = Objectives[ActiveObjectiveIndex].ObjectiveTransformInWorld;
            CompassRotation.instance.goal = GoalTransform;
            
            StartCoroutine(EventHandler.instance.HighlightObjective(GoalTransform.gameObject));
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(WorldTopLeft.x, 0, WorldTopLeft.y), 10.0f);
        Gizmos.DrawSphere(new Vector3(WorldBottomRight.x, 0, WorldBottomRight.y), 10.0f);

        Gizmos.DrawWireSphere(trackingTransform.position, MiniMapWorldSize);
    }
}
