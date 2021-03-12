using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralNoiseProject;

//This gen uses Scrawks marching cubes implementation
//https://github.com/Scrawk/Marching-Cubes

[ExecuteInEditMode][SelectionBase]
public class IslandGen : MonoBehaviour
{
    public Material m_material;

    public enum MARCHING_MODE { CUBES, TETRAHEDRON };

    [SerializeField]
    List<GameObject> meshes = new List<GameObject>();

    [Header("Noise Settings")]
    public int seed = 0;
    public int octaves = 3;
    [InspectorName("Frequency (Perlin)")]
    public float PerlinFreq = 2.0f;
    [InspectorName("Frequency (Fractal)")]
    public float FractalFreq = 1.0f;
    [Header("Marching Cube Settings")]
    public MARCHING_MODE mode = MARCHING_MODE.TETRAHEDRON;
    public Vector3Int VoxelSize;

    [Header("Island Settings")]
    public float IslandSize = 0.5f;
    public float IslandMidY = 0.8f;
    public float IslandMaxLowY = 0.0f;
    public int NumOfGradients = 1;
    public float MinGradientSize = 0.1f;
    public float MaxGradientSize = 0.5f;
    [Header("Brush")]
    public float BrushSize = 1.0f;
    public float BrushDensity = 1.0f;

    [HideInInspector]
    public Vector3 Hitpoint = Vector3.positiveInfinity;


    //The size of voxel array.
    int width = 32;
    int height = 32;
    int length = 32;

    float[] voxels;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnEnable()
    {
        
    }

    public bool ModifyVoxels(float digVal = 0.5f)
    {
        width = VoxelSize.x;
        height = VoxelSize.y;
        length = VoxelSize.z;

        bool returnVal = false; ;
        if (Hitpoint == Vector3.positiveInfinity)
        {
            return returnVal;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    Vector3 voxelPos = new Vector3((-width / 2) + x, (-height / 2) + y, (-length / 2) + z);

                    float fx = x / (width - 1.0f);
                    float fy = y / (height - 1.0f);
                    float fz = z / (length - 1.0f);

                    int idx = x + y * width + z * width * height;

                    Vector3 newVec = new Vector3(fx, fy, fz);
                    Vector3 test = new Vector3(-width / 2, -height / 2, -length / 2);
                    float dist = Vector3.Distance(Hitpoint, voxelPos);
                    if (dist < BrushSize)
                    {
                        voxels[idx] += digVal * (1 - (dist / BrushSize));
                        returnVal = true;
                    }
                    //voxels[idx] = weight * ((fractal.Sample3D(fx, fy, fz) + 1) / 2);
                    voxels[idx] = Mathf.Clamp(voxels[idx], -2, 2);
                }
            }
        }
        return returnVal;
    }
    public void ReGen()
    {
        width = VoxelSize.x;
        height = VoxelSize.y;
        length = VoxelSize.z;

        if (voxels == null || voxels.Length != (width * height * length))
        {
            voxels = new float[width * height * length];
        }
        INoise perlin = new PerlinNoise(seed, PerlinFreq);
        FractalNoise fractal = new FractalNoise(perlin, octaves, FractalFreq);

        //Z is mapped to size
        List<Vector3> grads = new List<Vector3>();

        for (int i = 0; i < NumOfGradients; i++)
        {
            Vector3 newGrad = new Vector3(Random.Range(MaxGradientSize / 2, 1.0f - MaxGradientSize / 2),
                                          Random.Range(MaxGradientSize / 2, 1.0f - MaxGradientSize / 2),
                                          Random.Range(MinGradientSize, MaxGradientSize));
            grads.Add(newGrad);
        }

        //Fill voxels with values. Im using perlin noise but any method to create voxels will work.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    
                    float fx = x / (width - 1.0f);
                    float fy = y / (height - 1.0f);
                    float fz = z / (length - 1.0f);

                    int idx = x + y * width + z * width * height;

                    Vector2 pos2 = new Vector2(fx, fz);


                    //Vector2 gradPos = new Vector2(grads[0].x, grads[0].y);
                    //float distFromCenter = Vector2.Distance(pos2, gradPos);
                    //float weight = (grads[0].z / distFromCenter);



                    float weight = 0.0f;

                    foreach (Vector3 item in grads)
                    {
                        Vector2 gradPos = new Vector2(item.x, item.y);
                        float gradSize = item.z;

                        float distFromGrad = Vector2.Distance(pos2, gradPos);

                        float distPercent = (gradSize / distFromGrad); //Furthest is 1, closest is 0

                        weight = weight + distPercent;
                    }
                    if (fy > IslandMidY)
                    {
                        weight = 0.0f;
                    }

                    weight *= ((fy + IslandMaxLowY) / IslandMidY);
                    voxels[idx] = weight * ((fractal.Sample3D(fx, fy, fz) + 1) / 2);
                    voxels[idx] = Mathf.Clamp(voxels[idx], -2, 2);

                }
            }
        }
    }

    public void ReBuild()
    {
        width = VoxelSize.x;
        height = VoxelSize.y;
        length = VoxelSize.z;

        INoise perlin = new PerlinNoise(seed, 2.0f);
        FractalNoise fractal = new FractalNoise(perlin, 3, 1.0f);

        //Set the mode used to create the mesh.
        //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
        MarchingCubesProject.Marching marching = null;
        if (mode == MARCHING_MODE.TETRAHEDRON)
            marching = new MarchingCubesProject.MarchingTertrahedron();
        else
            marching = new MarchingCubesProject.MarchingCubes();

        //Surface is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
        //The target value does not have to be the mid point it can be any value with in the range.
        marching.Surface = 1.0f;



        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(voxels, width, height, length, verts, indices);

        //A mesh in unity can only be made up of 65000 verts.
        //Need to split the verts between multiple meshes.

        int maxVertsPerMesh = 60000; //must be divisible by 3, ie 3 verts == 1 triangle
        int numMeshes = verts.Count / maxVertsPerMesh + 1;
        meshes.Clear();
        for (int i = 0; i < numMeshes; i++)
        {

            List<Vector3> splitVerts = new List<Vector3>();
            List<int> splitIndices = new List<int>();

            for (int j = 0; j < maxVertsPerMesh; j++)
            {
                int idx = i * maxVertsPerMesh + j;

                if (idx < verts.Count)
                {
                    splitVerts.Add(verts[idx]);
                    splitIndices.Add(j);
                }
            }

            if (splitVerts.Count == 0) continue;

            Mesh mesh = new Mesh();
            mesh.SetVertices(splitVerts);
            mesh.SetTriangles(splitIndices, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.AddComponent<MeshCollider>();
            go.GetComponent<Renderer>().material = m_material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.GetComponent<MeshCollider>().sharedMesh = mesh;
            go.transform.localPosition = new Vector3(-width / 2, -height / 2, -length / 2);
            
            go.layer = 8;
            
            meshes.Add(go);
        }

    }

    private void OnDrawGizmos()
    {
        if (Hitpoint != Vector3.positiveInfinity)
        {
            Gizmos.DrawWireSphere(Hitpoint, BrushSize);
        }

        Vector3 offset = new Vector3(width / 2, height / 2, length / 2);

        Gizmos.DrawSphere(Vector3.zero + transform.position, 1.0f);

        Gizmos.DrawSphere(offset + transform.position, 1.0f);
        Gizmos.DrawSphere(-offset + transform.position, 1.0f);
        //Gizmos.DrawSphere(Vector3.zero + transform.position, 1.0f);
        //Gizmos.DrawSphere(Vector3.zero + transform.position, 1.0f);

    }
}
