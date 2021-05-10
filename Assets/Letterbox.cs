using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letterbox : MonoBehaviour
{
    public Vector2 startPos;
    public bool top;
    private void Start()
    {
        startPos = transform.localPosition;
        top = startPos.y > 0;
    }

    private void Update()
    {
        float yPos = top ? -1.0f * CameraManager.instance.y * 1440.0f : CameraManager.instance.y * 1440.0f;
        transform.localPosition = new Vector3(startPos.x, startPos.y + yPos);
    }
}
