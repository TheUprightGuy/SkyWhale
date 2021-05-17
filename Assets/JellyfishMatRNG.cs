using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishMatRNG : MonoBehaviour
{
    Material mat1;
    Material mat2;

    private void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();

        mat1 = mr.materials[0];
        mat2 = mr.materials[1];

        Vector3 top = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        Vector3 bot = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        mat1.SetVector("_Vector3", top);
        mat2.SetVector("_Vector3", bot);
    }
}
