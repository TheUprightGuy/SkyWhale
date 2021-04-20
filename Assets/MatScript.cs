using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MatScript : MonoBehaviour
{
    public Material modMat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        modMat.SetVector("Vector3_67E76D2C", transform.position);
    }
}
