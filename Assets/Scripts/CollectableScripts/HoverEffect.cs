using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    public float RotationSpeed = 1.0f;
    public float FloatSpeed = 1.0f;
    public float HeightRange = 1.0f;

    public Vector3 rotateAxis = Vector3.up;

    float time = 0.0f;
    // Update is called once per frame
    void Update()
    {

        transform.RotateAround(transform.position, rotateAxis, RotationSpeed * Time.deltaTime);

        time += Time.deltaTime;
        transform.position = transform.position + (rotateAxis * ((HeightRange * Mathf.Sin(time * FloatSpeed))) );
    }
}
