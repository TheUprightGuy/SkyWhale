using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.UI;
using UnityEditor;
using System.Diagnostics;

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

    public GameObject prefabTemplate;

    public GrassContainer grassContainer;

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

    [Header("Distribuition Control")]
    //Defines size of radius
    public float BrushRadius = 5.0f;
    public float RefreshRateInMs = 10.0f;
    [Tooltip("The ammount of objects will be density squared")]
    public float density = 10;
    [Tooltip("The size in world units of each mesh square in Batched Render Mode")]
    public float CellSize = 5.0f;
    public int MaxPointsInChunk = 10000;
    [Tooltip("The maximum angle possible from the Transform up to the hit.normal")]
    public float MaxTerrainIncline = 20.0f;

    /// <summary>
    /// Size of each mesh chunk in world units
    /// </summary>
    public float ChunkSize = 12.0f;

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

    [Header("Debug")]
    public bool DrawBrush = false;
    public bool ShowChunkBorders = false;

    private void Awake()
    {

        //TODO: Remove IF public arraying success 
        switch (RenderType)
        {
            case RenderingMode.BATCHED:
                VerifyVariables();
                BuildMesh(true);
                //PlaceObjMesh();
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
        //foreach (var item in MeshCells)
        //{
        //    Graphics.DrawMesh(item, Vector3.zero, Quaternion.identity, prefabTemplate.GetComponentInChildren<MeshRenderer>().sharedMaterial, 0);
        //}

        if (grassContainer == null)
        {
            return;
        }
        foreach (var item in grassContainer.GrassChunks)
        {
            Graphics.DrawMesh(item.mesh, Vector3.zero, Quaternion.identity, prefabTemplate.GetComponentInChildren<MeshRenderer>().sharedMaterial, 0);
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

        Stopwatch st = new Stopwatch();
        
       //Whatever needs timing here

        List<Vector3> PointList = new List<Vector3>();

        UnityEngine.Debug.Log("Randomising Positions...");
        st.Start();
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

        st.Stop();
        UnityEngine.Debug.Log("Randomising positions done in " + st.ElapsedMilliseconds + "ms");
        st.Reset();

        PointList = RaycastPositions(PointList);


        switch (RenderType)
        {
            case RenderingMode.BATCHED:

                st.Start();
                VerifyVariables();
                st.Stop();
                UnityEngine.Debug.Log("VerifyVariables() done in " + st.ElapsedMilliseconds + "ms");
                st.Reset();

                st.Start();
                PointSet(PointList);
                st.Stop();
                UnityEngine.Debug.Log("PointSet() done in " + st.ElapsedMilliseconds + "ms");
                st.Reset();

                st.Start();
                BuildMesh();
                st.Stop();
                UnityEngine.Debug.Log("BuildMesh() done in " + st.ElapsedMilliseconds + "ms");
                st.Reset();

                break;
            case RenderingMode.INDIVIDUAL:
                IndividualMesh(PointList);
                break;
            default:
                break;
        }
    }

    public void VerifyVariables()
    {
        //If the grass container doesn't exist yet, add one
        if (grassContainer == null)
        {
            string fp = "Assets/" + transform.name + "GrassInstance.asset"; // Probs want to make this a public at some point
            GrassContainer newContainer = ScriptableObject.CreateInstance<GrassContainer>();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(newContainer);
#endif
            AssetDatabase.CreateAsset(newContainer, fp);
            AssetDatabase.SaveAssets();
            grassContainer = (GrassContainer)AssetDatabase.LoadAssetAtPath(fp, typeof(GrassContainer)); //Retrieve the new container 
        }

        if (grassContainer.GrassChunks == null)
        {
            grassContainer.GrassChunks = new List<MeshChunk>();
        }
    }

    public void PointGen(Vector3 point, float radius)
    {
        List<Vector3> newPoints = new List<Vector3>();

        for (int i = 0; i < density * radius; i++)
        {
            Vector2 randomPointxz = Random.insideUnitCircle * radius;
            Vector3 randomPoint = new Vector3(randomPointxz.x, transform.position.y/*point.y*/, randomPointxz.y);
            newPoints.Add(point + randomPoint);
        }
        newPoints = RaycastPositions(newPoints);
        PointSet(newPoints);
    }
    public void PointSet(List<Vector3> PointList)
    {
        foreach (var item in PointList) //Get the chunk for each point
        {

            Vector3Int ChunkIndex = GetChunk(item); //Get chunk index

            MeshChunk thisChunk;
            if (!grassContainer.GrassChunkAtIndex(ChunkIndex, out thisChunk)) //If the chunk has not been made yet
            {
                thisChunk = new MeshChunk(ChunkIndex); //Init the new meshchunk
                grassContainer.GrassChunks.Add(thisChunk); //Add the chunk
            }

            if (thisChunk.pointList.Count > MaxPointsInChunk)
            {
                return;
            }
            thisChunk.pointList.Add(item);
            thisChunk.Rebuild = true;
            grassContainer.GrassChunks[grassContainer.GrassChunkAtIndex(ChunkIndex)] = thisChunk; //Add this point to the chunk
        }
        

    }

    public void PointDelete(Vector3 point, float radius)
    {

        int chunkSpan = Mathf.CeilToInt(radius / ChunkSize); //Give the ammount of chunks this delete could effect
        Vector3Int hitChunk = GetChunk(point); //retrieve the confirm effected chunk

        for (int i = 0; i < grassContainer.GrassChunks.Count; i++)
        {
            if(grassContainer.GrassChunks[i].pointList.Count <= 0) { continue; }
            MeshChunk chunk = grassContainer.GrassChunks[i];
            //If within the effected chunk span
            List<int> indexDelete = new List<int>();
            for (int j = chunk.pointList.Count - 1; j >= 0; j--) //work downwards
            {
                float pointDist = Vector3.Distance(point, chunk.pointList[j]);
                if (pointDist <= radius) //check within radius
                {
                    indexDelete.Add(j);//Queue for deletion
                    
                }
            }

            if (indexDelete.Count == 0) //Don't bother with the rest
            {
                continue;
            }

            chunk.Rebuild = true; // Mark for next rebuild
            foreach (int index in indexDelete) //Delete all points within radius
            {
                chunk.pointList.RemoveAt(index);
                
            }

            chunk.mesh = null; //This is something to do with update refreshes and scriptable object memory. 


            grassContainer.GrassChunks[i] = chunk;
            //indexDelete.Clear(); //Clear memory
        }

    }
    public void BuildMesh(bool ForceFullRebuild = false)
    {
        //Todo: Rework this, MOSTLY
        //1: Get chunk position of each point
        //2: Check if chunk position already in array
        //IF CHUNK FOUND: 
        //Option A: Apply chunk mesh to front of CombineInstance Array at TRS 0 0 0 //PREFERRED
        //Option B: Apply positions within chunk to front of Positions Array
        //IF NOT:
        //Add new mesh to MeshCells array with the identifier of the chunk position and the mesh of the output mesh

        //Debug.Log("Batching Mesh...");

        Stopwatch st = new Stopwatch();

        st.Start();



        Mesh prefabMesh = new Mesh();
      

        List<CombineInstance> prefabList = new List<CombineInstance>();
        foreach (Transform item in prefabTemplate.transform) //combine all mesh in the prefab
        {
            CombineInstance newInstance = new CombineInstance();
            newInstance.mesh = item.GetComponent<MeshFilter>().sharedMesh; //Change rotation to collision normal of point + Vector3.up
            newInstance.transform = Matrix4x4.TRS(/*point +*/ /*prefabTemplate.transform.position +*/ item.transform.position, item.rotation, item.localScale);
            prefabList.Add(newInstance); //Apply mesh to the chunk instance mesh list
        }

        prefabMesh.CombineMeshes(prefabList.ToArray()); //Apply the new meshs

        //Create a combine instance array for each chunk
        Dictionary<Vector3Int, List<CombineInstance>> instanceChunks = new Dictionary<Vector3Int, List<CombineInstance>>();
        foreach (var chunk in grassContainer.GrassChunks)
        {
            //If not marked for rebuild or the mesh already exists
            if (!chunk.Rebuild && !ForceFullRebuild)
            {
                continue;
            }
            foreach (var point in chunk.pointList) //For each chunks points
            {
                List<CombineInstance> instanceList;
                if (!instanceChunks.TryGetValue(chunk.index, out instanceList)) //If no chunk found
                {
                    instanceList = new List<CombineInstance>();
                    instanceChunks.Add(chunk.index, instanceList); //Add if none found
                }

                float randScale = Random.Range(minScale, maxScale); //Apply a random scale to said prefab
      
                CombineInstance newInstance = new CombineInstance();
                newInstance.mesh = prefabMesh; //Change rotation to collision normal of point + Vector3.up
                newInstance.transform = Matrix4x4.TRS(point, Quaternion.identity,Vector3.one * randScale);

                instanceChunks[chunk.index].Add(newInstance); //Apply mesh to the chunk instance mesh list

                
                //Debug.Log("Batching Mesh Done");
            }

        }

        st.Stop();
        UnityEngine.Debug.Log("Instance building done " + st.ElapsedMilliseconds + "ms");
        st.Reset();
        //Debug.Log("Batching Mesh Done");


        st.Start();
        foreach (KeyValuePair<Vector3Int, List<CombineInstance>> kvp in instanceChunks)
        {
            MeshChunk thisChunk;
            if (!grassContainer.GrassChunkAtIndex(kvp.Key, out thisChunk)) //The chunk should be pre made in the point adding, but double check
            {
                continue;
            }
            thisChunk.mesh = null;
            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(kvp.Value.ToArray()); //Apply the new meshs
            
            thisChunk.mesh = newMesh;

            grassContainer.GrassChunks[grassContainer.GrassChunkAtIndex(kvp.Key)] = thisChunk; //Apply changes
        }


        for (int i = 0; i < grassContainer.GrassChunks.Count; i++)
        {
            MeshChunk thisChunk = grassContainer.GrassChunks[i];
            thisChunk.Rebuild = false;
            grassContainer.GrassChunks[i] = thisChunk; //FUCKING WHY????
        }
        st.Stop();
        UnityEngine.Debug.Log("Mesh building done " + st.ElapsedMilliseconds + "ms");
        st.Reset();
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
        
        int maxHits = 1;
        var results = new NativeArray<RaycastHit>(posList.Count * maxHits, Allocator.TempJob);
        var commands = new NativeArray<RaycastCommand>(posList.Count, Allocator.TempJob);


        Vector3 direction = Vector3.down;

        for (int i = 0; i < posList.Count; i++)
        {
            Vector3 normal = Vector3.down;
            Vector3 point = posList[i] + ((-normal) * 0.1f);
            commands[i] = new RaycastCommand(point, normal, Mathf.Infinity , RayCastToHit.value);
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
                    //break;
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

    //public void PlaceObjsGPUI()
    //{



    //    if (batches == null)
    //    {
    //        Debug.Log("Making Array");
    //        batches = new List<List<ObjData>>();
    //    }

    //    if (batches.Count > 0)
    //    {
    //        Debug.Log("Clearing Array");
    //        batches.Clear();
    //    }

    //    Debug.Log("Randomising Positions...");
    //    List<Vector3> PointList = ReRoll();
    //    Debug.Log("Randomising Positions Done");

    //    PointList = RaycastPositions(PointList);


    //    List<ObjData> currBatch = new List<ObjData>();


    //    int batchIndexNum = 0;
    //    for (int i = 0; i < PointList.Count; i++)
    //    {

    //        float randScale = Random.Range(minScale, maxScale);
    //        foreach (Transform item in prefabTemplate.transform)
    //        {

    //            //Apply prefabs positions AFTER raycast for offset
    //            currBatch.Add(new ObjData(PointList[i] + prefabTemplate.transform.position, item.localScale * randScale, item.rotation)); ;
    //            batchIndexNum++;
    //        }

    //        if (batchIndexNum >= 1000)
    //        {
    //            batches.Add(currBatch);
    //            currBatch = new List<ObjData>();
    //            batchIndexNum = 0;
    //        }

    //    }
    //    if (batchIndexNum < 1000)
    //    {
    //        batches.Add(currBatch);
    //    }


    //    RedrawGPUI();
    //}

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
