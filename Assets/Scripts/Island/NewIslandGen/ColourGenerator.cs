using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColourGenerator : MonoBehaviour {
    public Material mat;
    public Gradient gradient;
    public float normalOffsetWeight;

    Texture2D texture;
    const int textureResolution = 50;

    private void Start()
    {
        UpdateColors();
    }
    void Init () {
        if (texture == null || texture.width != textureResolution) {
            texture = new Texture2D (textureResolution, 1, TextureFormat.RGBA32, false);
        }
    }

    void OnValidate () {
        UpdateColors();
    }

    void UpdateColors()
    {
        Init();
        UpdateTexture();

        MeshRenderer m = GetComponent<MeshRenderer>();
        MeshFilter mf = GetComponent<MeshFilter>();

        if (m == null || mf == null)
        {
            return;
        }
        float boundsY = mf.sharedMesh.bounds.size.y * transform.position.y;
        mat = m.sharedMaterial;
        mat.SetFloat("boundsY", boundsY);
        mat.SetFloat("normalOffsetWeight", normalOffsetWeight);
        mat.SetVector("pos", transform.position);
        mat.SetTexture("ramp", texture);
        m.sharedMaterial = mat;
    }
    void UpdateTexture () {
        if (gradient != null) {
            Color[] colours = new Color[texture.width];
            for (int i = 0; i < textureResolution; i++) {
                Color gradientCol = gradient.Evaluate (i / (textureResolution - 1f));
                colours[i] = gradientCol;
            }

            texture.SetPixels (colours);
            texture.Apply ();
        }
    }
}