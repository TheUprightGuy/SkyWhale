using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbCamera : MonoBehaviour
{
    Vector3 offset;
    public Transform target;
    Quaternion rotOffset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
        rotOffset = Quaternion.Euler(transform.rotation.eulerAngles - target.rotation.eulerAngles);
    }

    public Vector3 mod;
    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + target.rotation * offset;
        transform.LookAt(target);
        //transform.rotation = target.rotation;
    }
}
