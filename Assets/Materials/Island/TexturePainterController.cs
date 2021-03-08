using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TexturePainterController : MonoBehaviour
{
    public enum ColorDefaults
    {
        SOLID,
        GRADIENT
    }
    
    [HideInInspector]
    public Vector3 Hitpoint = Vector3.positiveInfinity;


    public ColorDefaults ColorDefault;
    public Color SolidColor;

    public Texture2D DefaultTexture;

    public bool Red;
    public Texture2D RedTexture;
    public float RedStartPoint = 0.1f;
    public float RedFadeLength = 0.2f;

    public bool Green;
    public Texture2D GreenTexture;
    public float GreenStartPoint = 0.3f;
    public float GreenFadeLength = 0.2f;


    public bool Blue;
    public Texture2D BlueTexture;
    public float BlueStartPoint = 0.5f;
    public float BlueFadeLength = 0.2f;

    public float Gloss = 0.0f;

    [Header("Brush")]
    public Color BrushColor;
    public float BrushSize = 1.0f;
    public float BrushDensity = 1.0f;
    // Start is called before the first frame update

    [SerializeField]
    private Mesh SavedMesh;

    private void Awake()
    {
        if (SavedMesh != null && SavedMesh.colors.Length > 0)
        {
            GetComponent<MeshFilter>().sharedMesh = SavedMesh;
        }
        else
        {
            //UnityEditor.EditorUtility.SetDirty(this);
            UpdateDefaultColors();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        //UpdateDefaultColors();
    }

    public void UpdateDefaultColors()
    {
        if (!GetComponent<MeshFilter>() || !GetComponent<MeshRenderer>())
        {
            Debug.LogError("MeshFilter not found");
            return;
        }

        //UnityEditor.EditorUtility.SetDirty(gameObject.GetComponent<MeshRenderer>().material);
        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_DEFAULT", DefaultTexture);
        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_RED", RedTexture);
        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_GREEN", GreenTexture);
        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_BLUE", BlueTexture);

        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        int verticeCount = vertices.Length;
        Color[] colors = new Color[verticeCount];
        switch (ColorDefault)
        {
            case ColorDefaults.SOLID:
                for (int i = 0; i < verticeCount; i++)
                {
                    colors[i] = SolidColor;
                }
                break;
            case ColorDefaults.GRADIENT:
                for (int i = 0; i < verticeCount; i++)
                {

                    /*
                     * 0.2 start
                     * length 0.4
                     * 
                     * endpoint = start + length = 0.6
                     * 
                     * y = 0.4 //desired percentage should be 50% - 0.2 0.3 [0.4] 0.5 0.6
                     * 
                     * zero out start point, and apply same math to all other vars
                     * 
                     * 0.2 - 0.2 = 0.0
                     * 
                     * y - 0.2 = 0.2
                     * 
                     * 0.6 - 0.2 = 0.4
                     * 
                     * zeroed EndPoint / zeroed YPoint = 0.4 / 0.2 = 0.5 = 50%
                     */
                    float red = Mathf.Clamp01( (vertices[i].y - RedStartPoint) / RedFadeLength);
                    if (!Red) { red = 0.0f; }

                    float green = Mathf.Clamp01( (vertices[i].y - GreenStartPoint) / GreenFadeLength);
                    if (!Green) { green = 0.0f; }

                    float blue = Mathf.Clamp01((vertices[i].y - BlueStartPoint) / BlueFadeLength);
                    if (!Blue){ blue = 0.0f;}

                    colors[i] = Color.Lerp(Color.clear, Color.Lerp(Color.red, Color.Lerp(Color.green, Color.blue, blue), green), red);
                    colors[i].a = Gloss;
                }
                break;
            default:
                break;
        }

        GetComponent<MeshFilter>().sharedMesh.colors = colors;
        SavedMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    public void UpdateBrush()
    {

        if (Hitpoint == Vector3.positiveInfinity)
        {
            return;
        }

        if (!GetComponent<MeshFilter>() || !GetComponent<MeshRenderer>())
        {
            Debug.LogError("MeshFilter not found");
            return;
        }

        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_DEFAULT", DefaultTexture);
        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_RED", RedTexture);
        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_GREEN", GreenTexture);
        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_BLUE", BlueTexture);

        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        int verticeCount = vertices.Length;
        Color[] colors = GetComponent<MeshFilter>().sharedMesh.colors;
        
        for (int i = 0; i < verticeCount; i++)
        {
            Vector3 worldPt = transform.TransformPoint(vertices[i]);
            float dist = Vector3.Distance(Hitpoint, worldPt);
            if (dist < BrushSize)
            {
                colors[i] = Color.Lerp(colors[i], BrushColor, BrushDensity * (1 - (dist / BrushSize)));
            }
        }

        GetComponent<MeshFilter>().sharedMesh.colors = colors;
        SavedMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    private void OnDrawGizmos()
    {
        if (Hitpoint != Vector3.positiveInfinity)
        {
            //Gizmos.DrawWireSphere(Hitpoint, BrushSize);
        }

    }
}
