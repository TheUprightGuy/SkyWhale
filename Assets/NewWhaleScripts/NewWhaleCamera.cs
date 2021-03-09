using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewWhaleCamera : MonoBehaviour
{ 
    [Header("Required Fields")]
    public Transform basePosition;

    #region LocalVariables
    Transform target;
    float distance = 2.0f;
    float xSpeed = 10.0f, ySpeed = 2.0f;
    float yMinLimit = -60.0f, yMaxLimit = 60.0f;
    float distanceMin = 0.5f, distanceMax = 13.0f;
    float x = 0.0f, y = 0.0f;
    bool rotating = false;
    float timer = 0.0f, lerpTimer = 0.0f;
    #endregion LocalVariables
    #region Setup
    void Awake()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        distance = distanceMax;
        target = GetComponentInParent<NewWhaleMovement>().transform;
    }
    #endregion Setup

    void LateUpdate()
    {
        timer -= Time.deltaTime;
        lerpTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            rotating = true;
        }// (EventSystem.current.IsPointerOverGameObject()) ? false : true;}

        if (Input.GetMouseButtonUp(0))
        {
            rotating = false;
        }

        if (Input.GetMouseButton(0) && rotating)
        {
            timer = 1.5f;
            transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X") * xSpeed);
            transform.RotateAround(target.position, transform.right, -Input.GetAxis("Mouse Y") * ySpeed);
        }

        if (timer <= 0.0f)
        {
            float targetRotationAngle = basePosition.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            x = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, 5 * Time.deltaTime);
            targetRotationAngle = basePosition.eulerAngles.x;
            currentRotationAngle = transform.eulerAngles.x;
            if (currentRotationAngle > 90) { currentRotationAngle -= 360.0f; }

            y = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, 5 * Time.deltaTime);
            // Clamp Y Rotation
            y = ClampAngle(y, yMinLimit, yMaxLimit);
            // Set Camera Rotation
            Quaternion rotation = Quaternion.Euler(y, x, basePosition.eulerAngles.z);
            // Calc pos w/ new currentDistance
            Vector3 position = target.position - (rotation * Vector3.forward * distance);
            Vector3 adjust = Vector3.Lerp(transform.position, position, 5 * Time.deltaTime);
            // Set Position & Rotation
            transform.rotation = rotation;
            transform.position = (lerpTimer > 0) ? adjust : position;
        }
        

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}