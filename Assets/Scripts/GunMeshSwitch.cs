using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMeshSwitch : MonoBehaviour
{
    MeshFilter mf;
    MeshRenderer mr;
    private void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
    }

    public Transform shootPoint;


    public Mesh loadedMesh;
    public Mesh emptyMesh;
    public Material loadedMat;
    public Material emptyMat;

    public void Loaded(bool _toggle)
    {
        mf.mesh = _toggle ? loadedMesh : emptyMesh;
        mr.material = _toggle ? loadedMat : emptyMat;
    }
}
