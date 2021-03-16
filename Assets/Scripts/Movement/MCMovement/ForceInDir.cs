using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceInDir : MonoBehaviour
{
     public Vector3 dir;
    public float force;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity = ( dir * force);
    }
}
