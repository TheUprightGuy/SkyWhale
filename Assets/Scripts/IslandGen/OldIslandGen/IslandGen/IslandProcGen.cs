using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[ExecuteAlways]
public class IslandProcGen : MonoBehaviour
{
    //public float MeshWidth;
    //public float MeshHeight;

    [Range(2, 50)]
    public int Resolution = 5;

    public int octaves = 1;
    public int multiplier = 25;
    public float amplitude = 0.5f;
    public float lacunarity = 2;
    public float persistence = 0.9f;
    // Start is called before the first frame update
    public float MinVal = 0.0f;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;


    public List<GradientMask> PeakLists = new List<GradientMask>();


    private SimplexNoiseGenerator noiseGenerator;
    void Start()
    {
        localUp = transform.up;
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        noiseGenerator = new SimplexNoiseGenerator();

        GenQuad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public float NoiseMasked(float _x, float _y)
    {
        if (noiseGenerator == null)
        {
            noiseGenerator = new SimplexNoiseGenerator();
        }
        
        float noise = noiseGenerator.coherentNoise(_x, 1, _y, octaves, multiplier, amplitude, lacunarity, persistence);


        GradientMask testGrad = new GradientMask();
        testGrad.size = 1.0f;
        testGrad.Position = new Vector2(0.0f, 0.0f);

        Vector2 point = new Vector2(_x, _y);
        float dist = Vector2.Distance(point, testGrad.Position);
        float gradientVal = dist / testGrad.size; //0.0 full height, 1.0

        noise -= gradientVal;
        //foreach (var item in PeakLists)
        //{
        //    Vector2 point = new Vector2(_x, _y);
        //    float dist = Vector2.Distance(point, item.Position);

        //    float gradientVal = dist / item.size; //0.0 full height, 1.0

        //    noise -= gradientVal;

        //    //if gradient has range of 0.5, distance closer to that is a range 0 - 1
        //}
        return Mathf.Max(MinVal, noise);
    }

    private void OnValidate()
    {
        localUp = transform.up;
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
        GenQuad();


    }

    public void GenQuad()
    {
        MeshRenderer meshRenderer;
        if (!GetComponent<MeshRenderer>())
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));


        MeshFilter meshFilter;
        if (!GetComponent<MeshFilter>())
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        else
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[Resolution * Resolution];
        int[] triangles = new int[(Resolution - 1) * (Resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < Resolution; y++)
        {
            for (int x = 0; x < Resolution; x++)
            {
                int i = x + y * Resolution;
                Vector2 percent = new Vector2(x, y) / (Resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                pointOnUnitCube.y = 0;

                pointOnUnitCube.y += (NoiseMasked(pointOnUnitCube.x, pointOnUnitCube.z));

                vertices[i] = pointOnUnitCube;

                if (x != Resolution - 1 && y != Resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + Resolution + 1;
                    triangles[triIndex + 2] = i + Resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + Resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
