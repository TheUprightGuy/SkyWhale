﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.UI;

public class ObjData
{

    public Vector3 pos;

    public Vector3 scale;

    public Quaternion rot;

   
    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rot, scale);
        }
    }

    public ObjData(Vector3 pos, Vector3 scale, Quaternion rot)
    {
        this.pos = pos;
        this.scale = scale;
        this.rot = rot;
    }

}
    [ExecuteAlways]
    public class ObjDistribuition : MonoBehaviour
    {
    public float debugValue = 5.0f;

    public GameObject prefabTemplate;

    [Header("Randomiser")]
    public int Seed = 69;
    public float minScale = 1.0f;
    public float maxScale = 1.0f;


    public enum RandomiserType
    {
        CELLED,
        PURE
    }

    public RandomiserType Randomiser = RandomiserType.CELLED;

    //Defines size of radius
    [Tooltip("The ammount of objects will be density squared")][Header("Distribuition Control")]
    public float density = 10;
    [Tooltip("The size in world units of each mesh square in Batched Render Mode")]
    public float CellSize = 5.0f;
    [Tooltip("The maximum angle possible from the Transform up to the hit.normal")]
    public float MaxTerrainIncline = 20.0f;

    public enum RenderingMode
    {
        BATCHED,
        INDIVIDUAL
    }

    public RenderingMode RenderType;

    [Header("Raycast Options")]
    public string ExclusionTag;
    public bool CastForTriggers = false;
    public LayerMask RayCastToHit;

    
    [SerializeField]
    private List<List<ObjData>> batches;

    private List<Mesh> MeshCells;
    [HideInInspector]
    public Bounds BoundingBox; //TODO: Change to brush



    /// <summary>
    /// Size of each mesh chunk in world units
    /// </summary>
    const float ChunkSize = 8.0f;

    //List<Vector3> PointList = new List<Vector3>();


    private void Awake()
    {

        //TODO: Remove IF public arraying success 
        switch (RenderType)
        {
            case RenderingMode.BATCHED:
                PlaceObjMesh();
                break;
            case RenderingMode.INDIVIDUAL:
                break;
            default:
                break;
        }
    }



    private void Start()
    {
        ///PlaceObjMesh();
    }

    private void Update()
    {
        switch (RenderType)
        {
            case RenderingMode.BATCHED:
                RedrawMesh();
                break;
            case RenderingMode.INDIVIDUAL  :
                break;
            default:
                break;
        }
        
    }

    

    /// <summary>
    /// Draws each mesh in <see cref="MeshCells"/> using <see cref="Graphics.DrawMesh(Mesh, Vector3, Quaternion, Material, int)"/>
    /// <br>Author:Jack Belton</br>
    /// </summary>
    public void RedrawMesh()
    {
        foreach (var item in MeshCells)
        {
            Graphics.DrawMesh(item, Vector3.zero, Quaternion.identity, prefabTemplate.GetComponentInChildren<MeshRenderer>().sharedMaterial, 0);
        }
    }

    /// <summary>
    /// Places the grass and processes the mesh depending on the <see cref="RenderType"/>
    /// <br>Author:Jack Belton</br>
    /// </summary>
    public void PlaceObjMesh()
    {

        if (RenderType == RenderingMode.INDIVIDUAL) //Destroy before hand to avoid raycast issues
        {
            for (int i = transform.childCount; i > 0; --i)
                DestroyImmediate(transform.GetChild(0).gameObject);

        }

        List<Vector3> PointList = new List<Vector3>();
        Debug.Log("Randomising Positions...");
        switch (Randomiser) //Todo: expose the pointlist as public so these random points can be set by a brush from editor
        {
            case RandomiserType.CELLED:
                PointList = ReRollCelled();
                break;
            case RandomiserType.PURE:
                PointList = ReRollPure();
                break;
            default:
                break;
        }

        Debug.Log("Randomising Positions Done");

        PointList = RaycastPositions(PointList);


        switch (RenderType)
        {
            case RenderingMode.BATCHED:
                BatchMesh(PointList);
                break;
            case RenderingMode.INDIVIDUAL:
                IndividualMesh(PointList);
                break;
            default:
                break;
        }
    }


    public void IndividualMesh(List<Vector3> PointList)
    {

        Vector2 startPos = new Vector2(transform.position.x - (transform.localScale.x / 2), transform.position.z - (transform.localScale.z / 2));

    

        for (int i = 0; i < PointList.Count; i++)
        {
            float xLocal = PointList[i].x - startPos.x;
            int col = Mathf.FloorToInt(xLocal / CellSize);

            float yLocal = PointList[i].z - startPos.y;
            int row = Mathf.FloorToInt(yLocal / CellSize);

            float randScale = Random.Range(minScale, maxScale);

            GameObject newObj = Instantiate(prefabTemplate);
            newObj.transform.localScale = prefabTemplate.transform.localScale * randScale;
            newObj.transform.position = PointList[i] + prefabTemplate.transform.position;

            newObj.transform.parent = this.transform;

        }
    }

    public void BatchMesh(List<Vector3> PointList)
    {
        if (MeshCells == null)
        {
            Debug.Log("Making Array");
            MeshCells = new List<Mesh>();
        }

        if (MeshCells.Count > 0)
        {
            Debug.Log("Clearing Array");
            MeshCells.Clear();
        }

        //Todo: Rework this, MOSTLY
        //1: Get chunk position of each point
        //2: Check if chunk position already in array
            //IF CHUNK FOUND: 
                //Option A: Apply chunk mesh to front of CombineInstance Array at TRS 0 0 0 //PREFERRED
                //Option B: Apply positions within chunk to front of Positions Array
            //IF NOT:
                //Add new mesh to MeshCells array with the identifier of the chunk position and the mesh of the output mesh


        int numOfColumns = Mathf.CeilToInt(transform.localScale.x / CellSize);
        int numOfRows = Mathf.CeilToInt(transform.localScale.z / CellSize);
        Vector2 startPos = new Vector2(transform.position.x - (transform.localScale.x / 2), transform.position.z - (transform.localScale.z / 2));


        List<List<CombineInstance>> MeshCellInstances = new List<List<CombineInstance>>();
        for (int i = 0; i < numOfColumns * numOfRows; i++)
        {
            List<CombineInstance> temp = new List<CombineInstance>();
            MeshCellInstances.Add(temp);
        }

        Debug.Log("Batching Mesh...");
        for (int i = 0; i < PointList.Count; i++)
        {
            float xLocal = PointList[i].x - startPos.x;
            int col = Mathf.FloorToInt(xLocal / CellSize);

            float yLocal = PointList[i].z - startPos.y;
            int row = Mathf.FloorToInt(yLocal / CellSize);

            int iIndex = row * numOfColumns + col; //Get the current grid index

            float randScale = Random.Range(minScale, maxScale); //Apply a random scale to said prefab

            //Todo: Combine the prefab mesh before loop //LOW PRIORITY
            foreach (Transform item in prefabTemplate.transform) //combine all mesh in the prefab
            {
                CombineInstance newInstance = new CombineInstance();
                newInstance.mesh = item.GetComponent<MeshFilter>().sharedMesh; //Change rotation to collision normal of point + Vector3.up
                newInstance.transform = Matrix4x4.TRS(PointList[i] + prefabTemplate.transform.position + item.transform.position, item.rotation, item.localScale * randScale);
                MeshCellInstances[iIndex].Add(newInstance);
            }
        }
        Debug.Log("Batching Mesh Done");

        foreach (var item in MeshCellInstances)
        {
            if (item.Count < 1)
            {
                continue;
            }

            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(item.ToArray());
            MeshCells.Add(newMesh);
        }
    }

    /// <summary>
    /// Randomises a random position within a cell in a grid, for a more even distribution
    /// <br>Author:Jack Belton</br>
    /// </summary>
    /// <returns>List of points for each foliage prefab to be placed</returns>
    public List<Vector3> ReRollCelled()
    {
        List<Vector3> PointList = new List<Vector3>();

        float stepSizeX = transform.localScale.x / density;
        float stepSizeZ = transform.localScale.z / density;

        Random.InitState(Seed);
        for (float i = -(density / 2); i < density / 2; i++) //have inital position been in center, rather than up and left
        {
            for (float j = -(density / 2); j < density / 2; j++)
            {
                //Get random point in cell.
                float randX = Random.Range(i * stepSizeX, (i + 1) * stepSizeX);
                float randZ = Random.Range(j * stepSizeZ, (j + 1) * stepSizeZ);

                Vector3 point = new Vector3(randX, 0, randZ);

                //Apply local position
                PointList.Add(transform.position + point);

            }
        }

        return PointList;
    }


    /// <summary>
    /// Randomises a random position within the bounding box
    /// <br>Author:Jack Belton</br>
    /// </summary>
    /// <returns>List of points for each foliage prefab to be placed</returns>
    public List<Vector3> ReRollPure()
    {
        List<Vector3> PointList = new List<Vector3>();

        //float stepSizeX = transform.localScale.x / density;
        //float stepSizeZ = transform.localScale.z / density;

        Random.InitState(Seed);
        for (float i = -(density / 2); i < density / 2; i++) //have inital position been in center, rather than up and left
        {
            for (float j = -(density / 2); j < density / 2; j++)
            {
                //Get random point in cell.
                float randX = Random.Range(-(transform.localScale.x / 2), (transform.localScale.x / 2));
                float randZ = Random.Range(-(transform.localScale.z / 2), (transform.localScale.z / 2));

                Vector3 point = new Vector3(randX, 0, randZ);

                //Apply local position
                PointList.Add(transform.position + point);

            }
        }

        return PointList;
    }



    //Todo: Attempt to reuse this by inputing a normal list as well as a position so can raycast to any surface
    public List<Vector3> RaycastPositions(List<Vector3> posList)
    {



        //// Perform a single raycast using RaycastCommand and wait for it to complete
        //// Setup the command and result buffers
        int maxHits = 4;
        var results = new NativeArray<RaycastHit>(posList.Count * maxHits, Allocator.TempJob);
        var commands = new NativeArray<RaycastCommand>(posList.Count, Allocator.TempJob);


        Vector3 direction = Vector3.down;

        for (int i = 0; i < posList.Count; i++)
        {
            commands[i] = new RaycastCommand(posList[i], Vector3.down,Mathf.Infinity , RayCastToHit.value);
        }

        //Debug.Log("Scheduling Raycasts...");
        //// Schedule the batch of raycasts
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, maxHits, default(JobHandle));

        //// Wait for the batch processing job to complete
        handle.Complete();
        //Debug.Log("RayCasts Complete");

        List<Vector3> returnList = new List<Vector3>();

        
        for (int i = 0; i < commands.Length; i++)
        {
            for (int j = 0; j < maxHits; j++)
            {
                if (results[i + j].collider == null) //Any null in the group should break the loop cause the later nulls will not be null
                {
                    break;
                }

                if (CastForTriggers != results[i + j].collider.isTrigger) //If trigger just pass on through
                {
                    continue;
                }


                if (results[i + j].collider.tag == ExclusionTag)//Make sure not hitting exclusion zone
                {
                    break;
                }

                float hitIncline = Vector3.Angle(results[i + j].collider.gameObject.transform.up, results[i + j].normal);

                if (hitIncline > MaxTerrainIncline) //If this hit point is greater than the maximum possible incline set by user
                {
                    break;
                }

                returnList.Add(results[i + j].point);
                break; //Break from group so doesn't double add

            }
        }

        results.Dispose();
        commands.Dispose();

        return returnList;
    }

    /// <summary>
    /// Calculates the chunk of size <see cref="ChunkSize"/> of some position.
    /// </summary>
    /// <param name="_pos">The position inside the chunk</param>
    /// <returns>The chunk index</returns>
    public Vector3Int GetChunk(Vector3 _pos)
    {
        //Vector3Int returnVec = -Vector3Int.one;
        Vector3Int returnVec = new Vector3Int(
            Mathf.FloorToInt(_pos.x / ChunkSize),
            Mathf.FloorToInt(_pos.y / ChunkSize),
            Mathf.FloorToInt(_pos.z / ChunkSize));
        return returnVec;
    }

    //DEFUNCT PLS NO USE TY
    #region DEFUNCT
    public void PlaceObjsGPUI()
    {



        if (batches == null)
        {
            Debug.Log("Making Array");
            batches = new List<List<ObjData>>();
        }

        if (batches.Count > 0)
        {
            Debug.Log("Clearing Array");
            batches.Clear();
        }

        Debug.Log("Randomising Positions...");
        List<Vector3> PointList = ReRoll();
        Debug.Log("Randomising Positions Done");

        PointList = RaycastPositions(PointList);


        List<ObjData> currBatch = new List<ObjData>();


        int batchIndexNum = 0;
        for (int i = 0; i < PointList.Count; i++)
        {

            float randScale = Random.Range(minScale, maxScale);
            foreach (Transform item in prefabTemplate.transform)
            {

                //Apply prefabs positions AFTER raycast for offset
                currBatch.Add(new ObjData(PointList[i] + prefabTemplate.transform.position, item.localScale * randScale, item.rotation)); ;
                batchIndexNum++;
            }

            if (batchIndexNum >= 1000)
            {
                batches.Add(currBatch);
                currBatch = new List<ObjData>();
                batchIndexNum = 0;
            }

        }
        if (batchIndexNum < 1000)
        {
            batches.Add(currBatch);
        }


        RedrawGPUI();
    }

    public List<Vector3> ReRoll()
    {
        List<Vector3> PointList = new List<Vector3>();

        float stepSizeX = transform.localScale.x / density;
        float stepSizeZ = transform.localScale.z / density;

        Random.InitState(Seed);
        for (float i = -(density / 2); i < density / 2; i++) //have inital position been in center, rather than up and left
        {
            for (float j = -(density / 2); j < density / 2; j++)
            {
                //Get random point in cell.
                float randX = Random.Range(i * stepSizeX, (i + 1) * stepSizeX);
                float randZ = Random.Range(j * stepSizeZ, (j + 1) * stepSizeZ);

                Vector3 point = new Vector3(randX, 0, randZ);

                //Apply local position
                PointList.Add(transform.position + point);

            }
        }

        return PointList;
    }

    /// <summary>
    /// Utilises GPU instancing in shader, for all mesh with the same 
    /// <br>DEFUNCT</br>
    /// <br><see cref="RedrawMesh()">RedrawMesh()</see> is up to date.</br>
    /// <br>Author:Jack Belton</br>
    /// </summary>
    public void RedrawGPUI()
    {
        if (batches != null)
        {
            foreach (var batch in batches)
            {
                Graphics.DrawMeshInstanced(prefabTemplate.GetComponentInChildren<MeshFilter>().sharedMesh, 0, prefabTemplate.GetComponentInChildren<MeshRenderer>().sharedMaterial, batch.Select((a) => a.matrix).ToList());
            }
        }

    }

    #endregion


    public RaycastHit gizmoRayHit;
    private void OnDrawGizmos()
    {
       
    }
}
