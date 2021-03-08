using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform target;
    public Transform PlayerObj;

    [Header("Settings")]
    public bool invertX = false;
    public bool invertY = false;

  
    public float rotateSpeed = 5;
    public float zoomSpeed = 5;


    public float minYAngle = 0.0f;
    public float maxYAngle = 45.0f;

    public float defaultZoom = 15.0f;
    public float minZoomDist = 10.0f;
    public float maxZoomDist = 50.0f;
    
    [Header("Collisions")]
    public float camCollisionRadius = 1.0f;
    public float WhiskerFieldAngle = 45.0f;
    public uint WhiskerAmount = 5;

    [Header("Debug")]
    [Tooltip("Press ` to enable/disable cursor locking.")]
    public bool UnlockCameraKeyBind = true;

    Vector3 offset;
    Vector3 storedPos = Vector3.zero;

    Ray[] Whiskers;
    void Start()
    {
        offset = target.position - transform.position;
        storedPos.x = PlayerObj.eulerAngles.y;
        storedPos.y = PlayerObj.eulerAngles.x;
        storedPos.z = defaultZoom;

        Cursor.lockState = CursorLockMode.Locked;

        Whiskers = new Ray[WhiskerAmount];

        //for (int i = 0; i < Whiskers.Length; i++)
        //{
        //    float angle = (WhiskerFieldAngle / WhiskerAmount) * (i + 1); //dividing by 0 = bad
        //    Vector3 dir = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
        //    Whiskers[i].direction = dir;
        //}

    }

    private void Update()
    {
        if (Input.GetKeyDown("`"))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            //Get Mouse Axis
            /*********************/
            float vertical = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
            float horizontal = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;

            //Invert if setting applied
            if (invertX) { horizontal *= -1; }
            if (invertY) { vertical *= -1; }
            /*********************/

            //Rotate Target

            float desiredAngleY = target.eulerAngles.y;

            //Get the current stored Y angle and add mouse axis to it
            storedPos.y += vertical;
            storedPos.y = Mathf.Clamp(storedPos.y, minYAngle, maxYAngle);

            storedPos.x += horizontal;

            if (!Input.GetKey(KeyCode.Mouse1)) //Not holding rightclick, allow player rotation
            {
                PlayerObj.Rotate(0, horizontal, 0);//Rotate player along with the  camera
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))//If rightclick released, snap back to position
            {
                storedPos.x = target.eulerAngles.y;
            }

            Quaternion rotation = Quaternion.Euler(storedPos.y, storedPos.x, 0);


            //Get a new position based on target rotation, offset, and stored y rotation
            Vector3 newPos = target.position - ((rotation * offset).normalized);

            //ZOOM
            /************************/

            Vector3 nextPos = transform.position;

            Vector3 camToTarget = Vector3.Normalize(newPos - target.position); //Create length of one

                                                          //-1 to account for normalised length
            storedPos.z = Mathf.Clamp(storedPos.z - zoom, minZoomDist - 1, maxZoomDist);


            newPos = newPos + (camToTarget * (storedPos.z));
            //RAYCASTING
            /************************/

            //Setup Raycast from target to camera
            float rayDist = Vector3.Distance(newPos, target.position);
            RaycastHit hit;

            /*If there is an object between the target and the camera, 
	         set the position to the closest point with nothing between the two*/

            
            if (Physics.Raycast(target.position, camToTarget, out hit, rayDist))
            {
                //collidePoint = hit.point;


            }
            //else //If nothing, just set the point to the newPos
            //{
            //    overrideZoom = false;
            //    nextPos = newPos;
            //}

            transform.position = newPos;// Vector3.SmoothDamp(transform.position, newPos, ref CamMoveVel, CamSmoothing);

            transform.LookAt(target);
        }
    }

    //private void SlerpRot()
    //{
    //    var target_rot = Quaternion.LookRotation(target.transform.position - transform.position);
    //    var delta = Quaternion.Angle(transform.rotation, target_rot);
    //    if (delta > 0.0f)
    //    {
    //        var t = Mathf.SmoothDampAngle(delta, 0.0f, ref AngularVelocity, RotateSmoothTime);
    //        t = 1.0f - t / delta;
    //        transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, t);
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        //float halfAngle = (WhiskerFieldAngle / 2);
        //Vector3 origin = Vector3.left;
        //Vector3 dir = Vector3.RotateTowards(transform.forward, transform.right, WhiskerFieldAngle * Mathf.Deg2Rad, 0.0f);
        //Gizmos.DrawLine(transform.position, transform.position + (dir));


        if (Application.isPlaying)
        {
            float angle = -(WhiskerFieldAngle / 2); //start here;
            float divided = (WhiskerFieldAngle / WhiskerAmount);
            for (int i = 0; i <= WhiskerAmount; i++)
            {
                Vector3 dir = Vector3.RotateTowards(transform.forward, transform.right, (angle) * Mathf.Deg2Rad, 0.0f).normalized;
                float playerToCam = Vector3.Distance(transform.position, target.position);
                float dist = playerToCam / Mathf.Cos(Vector3.Angle(transform.forward, dir) * Mathf.Deg2Rad);

                Gizmos.DrawLine(transform.position, transform.position + (dir * dist));
                
                angle += divided;
            }
        }
        else
        {

        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal = false)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}