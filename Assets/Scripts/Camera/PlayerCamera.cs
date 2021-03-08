using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity = 5.0f;
    public Transform target;
    public Transform character;
    public float offset = 2.0f;
    public Vector2 pitchMinMax = new Vector2(-40.0f, 85.0f);

    public float rotationSmoothTime = 0.12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float yaw, pitch;
    public float timer = 0;
    
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check Current Rotation
            currentRotation = transform.eulerAngles;
            pitch = currentRotation.x;
            yaw = currentRotation.y;
        }

        if (Input.GetMouseButton(0))
        {
            // Rotate w/ Mouse Drag
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            // Smooth Rotation
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;
            // Lerp Timer
            timer = 1.5f;
        }

        // Hide Cursor on Drag
        Cursor.visible = !Input.GetMouseButton(0);

        timer -= Time.deltaTime;
        // Begin Lerp
        if (timer <= 0)
        {
            // Lerp Between Current & Character Rotations
            Vector3 adj = Vector3.Lerp(transform.rotation.eulerAngles, character.rotation.eulerAngles, Time.deltaTime);
            adj = new Vector3(Mathf.Clamp(adj.x, pitchMinMax.x, pitchMinMax.y), adj.y, 0.0f);
            transform.rotation = Quaternion.Euler(adj);
        }

        // Set Position
        transform.position = target.position - transform.forward * offset;
    }
}
