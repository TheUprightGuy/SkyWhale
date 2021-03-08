using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBobbing : MonoBehaviour
{
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = startPos;
        newPos.y += Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time / 10, 1));
        transform.position = newPos;
    }
}
