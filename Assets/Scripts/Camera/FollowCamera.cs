using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FollowCamera : MonoBehaviour
{
    Transform currentCam;
    [Header("Required Fields")]
    public Transform target;
    public Transform playerCam;
    public Transform whaleCam;
    public Transform whale;

    #region LocalVariables
    float distance = 2.0f;
    float xSpeed = 10.0f, ySpeed = 2.0f;
    float yMinLimit = -60.0f, yMaxLimit = 60.0f;
    float distanceMin = 0.5f, distanceMax = 13.0f;
    float x = 0.0f, y = 0.0f;
    public bool zoomIn = false, zoomOut = false;
    public bool rotating = false;
    float timer = 0.0f, lerpTimer = 0.0f;
    #endregion LocalVariables
    #region Setup
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        distance = distanceMax;
        currentCam = whaleCam;
    }
    #endregion Setup

    void LateUpdate()
    {
        timer -= Time.deltaTime;
        lerpTimer -= Time.deltaTime;

        if (zoomIn)
        {
            currentCam = playerCam;
            transform.position = Vector3.Lerp(transform.position, playerCam.position, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, playerCam.rotation, Time.deltaTime);
            if (Vector3.Distance(transform.position, playerCam.position) < 0.5f)
            {
                zoomIn = false;
                target = playerCam;
                lerpTimer = 1.0f;
            }
            distance = distanceMin;
        }
        else if (zoomOut)
        {
            currentCam = whaleCam;
            transform.position = Vector3.Lerp(transform.position, whaleCam.position, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, whaleCam.rotation, Time.deltaTime);
            if (Vector3.Distance(transform.position, whaleCam.position) < 0.5f)
            {
                zoomOut = false;
                target = whale;
                lerpTimer = 1.0f;
            }
            distance = distanceMax;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                rotating = true;
            }// (EventSystem.current.IsPointerOverGameObject()) ? false : true;}

            if (Input.GetMouseButtonUp(0)){
                rotating = false;}

            if (Input.GetMouseButton(0) && rotating)
            {  
                timer = 1.5f;
                transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X") * xSpeed);
                transform.RotateAround(target.position, transform.right, -Input.GetAxis("Mouse Y") * ySpeed);       
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && !EventHandler.instance.gameState.gamePaused){
                zoomIn = true;
                zoomOut = false;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f && !EventHandler.instance.gameState.gamePaused){
                zoomOut = true;
                zoomIn = false;
            }

            if (timer <= 0.0f)
            {
                float targetRotationAngle = currentCam.eulerAngles.y;
                float currentRotationAngle = transform.eulerAngles.y;
                x = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, 5 * Time.deltaTime);
                targetRotationAngle = currentCam.eulerAngles.x;
                currentRotationAngle = transform.eulerAngles.x;
                if (currentRotationAngle > 90){ currentRotationAngle -= 360.0f;}

                y = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, 5 * Time.deltaTime);
                // Clamp Y Rotation
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                // Set Camera Rotation
                Quaternion rotation = Quaternion.Euler(y, x, currentCam.eulerAngles.z);
                // Calc pos w/ new currentDistance
                Vector3 position = target.position - (rotation * Vector3.forward * distance);
                Vector3 adjust = Vector3.Lerp(transform.position, position, 5 * Time.deltaTime);
                // Set Position & Rotation
                transform.rotation = rotation;
                transform.position = (lerpTimer > 0) ? adjust : position;
            }
        }

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
