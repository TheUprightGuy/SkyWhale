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

    public float CameraSnapAcceleration = 1.0f;

    public float minYAngle = 0.0f;
    public float maxYAngle = 45.0f;

    public float defaultZoom = 15.0f;
    public float minZoomDist = 10.0f;
    public float maxZoomDist = 50.0f;

    [Header("Collisions")]
    public LayerMask CollisionLayers;
    public float camCollisionRadius = 1.0f;
    public float RayLength = 5.0f;
    public float CastWidth = 0.25f;
    public float CameraAcceleration = 0.5f;
    [Header("Debug")]
    [Tooltip("Press ` to enable/disable cursor locking.")]
    public bool UnlockCameraKeyBind = true;

    Vector3 lastCalculatedPos = Vector3.zero;
    Vector3 offset;
    Vector3 storedPos = Vector3.zero;
    bool waitingToReturn = false;
    
    void Start()
    {
        offset = target.position - transform.position;
        storedPos.x = PlayerObj.eulerAngles.y;
        storedPos.y = PlayerObj.eulerAngles.x;
        storedPos.z = defaultZoom;

        Cursor.lockState = CursorLockMode.Locked;

        lastCalculatedPos = transform.position;

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

           
            if (!Input.GetKey(KeyCode.LeftAlt) && !waitingToReturn) //Not holding rightclick, allow player rotation
            {
                PlayerObj.Rotate(0, horizontal, 0);//Rotate player along with the  camera
            }
            if (Input.GetKeyUp(KeyCode.LeftAlt))//If rightclick released, snap back to position
            {
                waitingToReturn = true;
                //storedPos.x = target.eulerAngles.y;
            }
            if (waitingToReturn)
            {
                if (storedPos.x != target.eulerAngles.y)
                {
                    storedPos.x = Mathf.MoveTowardsAngle(storedPos.x, target.eulerAngles.y, CameraSnapAcceleration * Time.deltaTime);
                }
                else
                {
                    waitingToReturn = false;
                }
            }
            Quaternion rotation = Quaternion.Euler(storedPos.y, storedPos.x, 0);


            //Get a new direction to orbit too
            Vector3 newDir = (rotation * offset).normalized;

            //ZOOM
            /************************/

            float newDist = 0.0f;
            //Vector3 camToTarget = Vector3.Normalize(newPos - target.position); //Create length of one


            //-1 to account for normalised length
            newDist = storedPos.z = Mathf.Clamp(storedPos.z - zoom, minZoomDist, maxZoomDist);

            //RAYCASTING
            /************************/

            RaycastHit fronthit;

            float currentDist = Vector3.Distance(target.position, transform.position);

            //If something ahead of you
            if (Physics.SphereCast(target.position, CastWidth, -newDir, out fronthit, Mathf.Min(newDist, RayLength), CollisionLayers.value))
            {
                
                newDist = Mathf.MoveTowards(currentDist , fronthit.distance + camCollisionRadius , CameraAcceleration * Time.deltaTime);
            }
            else
            {
                newDist = Mathf.MoveTowards(currentDist, newDist, CameraAcceleration * Time.deltaTime);
            }
            Vector3 newPos = target.position - (newDir * newDist);
            
            transform.position = newPos; //Set positions

            transform.LookAt(target); //Set rotations
        }
    }

    Vector3 speed;
    float previousDist;
    


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (Application.isPlaying)
        {

            Quaternion rotation = Quaternion.Euler(storedPos.y, storedPos.x, 0);
            Vector3 newDir = (rotation * offset).normalized;

            Gizmos.DrawLine(target.position, target.position - (newDir * storedPos.z));
        }
        else
        {
        }
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