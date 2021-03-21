using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform targetTrans;
    public Transform PlayerTrans;

    [Header("Settings")]
    public Vector3 startOffset = Vector3.zero;

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

    public float smoothTime = 0.1f;
    [Header("Collisions")]
    public LayerMask CollisionLayers;
    public float camCollisionRadius = 1.0f;
    public float RayLength = 5.0f;
    public float CastWidth = 0.25f;
    public float CameraAcceleration = 0.5f;

    public bool RaycastToPlayer = true;
    [Header("Debug")]
    [Tooltip("Press ` to enable/disable cursor locking.")]
    public bool UnlockCameraKeyBind = true;

    Vector3 lastCalculatedPos = Vector3.zero;
    Vector3 offset;
    Vector3 storedPos = Vector3.zero;
    bool waitingToReturn = false;

    void Start()
    {
        offset = startOffset; // targetTrans.position - transform.position;
        storedPos.x = PlayerTrans.eulerAngles.y;
        storedPos.y = PlayerTrans.eulerAngles.x;
        storedPos.z = defaultZoom;

        Cursor.lockState = CursorLockMode.Locked;

        lastCalculatedPos = transform.position;

    }

    private void LateUpdate()
    {
        //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, PlayerTrans.rotation.eulerAngles.z));

        /*if (Input.GetKey(KeyCode.W))
        {
            if (storedPos.x != targetTrans.eulerAngles.y)
            {
                storedPos.x = Mathf.MoveTowardsAngle(storedPos.x, targetTrans.eulerAngles.y, CameraSnapAcceleration * Time.deltaTime);
            }
        }*/


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

            float desiredAngleY = targetTrans.eulerAngles.y;

            //Get the current stored Y angle and add mouse axis to it
            storedPos.y += vertical;
            storedPos.y = Mathf.Clamp(storedPos.y, minYAngle, maxYAngle);

            storedPos.x += horizontal;


            /*if (!Input.GetKey(KeyCode.LeftAlt) && !waitingToReturn) //Not holding rightclick, allow player rotation
            {
                PlayerTrans.Rotate(0, horizontal, 0);//Rotate player along with the  camera
            }*/
            if (Input.GetKeyUp(KeyCode.LeftAlt))//If rightclick released, snap back to position
            {
                waitingToReturn = true;
                //storedPos.x = target.eulerAngles.y;
            }
            if (waitingToReturn)
            {
                if (storedPos.x != targetTrans.eulerAngles.y)
                {
                    storedPos.x = Mathf.MoveTowardsAngle(storedPos.x, targetTrans.eulerAngles.y, CameraSnapAcceleration * Time.deltaTime);
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

            float currentDist = Vector3.Distance(targetTrans.position, transform.position);


            Vector3 newPosBeforCollision = targetTrans.position - (newDir * newDist);
            Vector3 playerToNewPos = newPosBeforCollision - PlayerTrans.position;

            Vector3 origin = (RaycastToPlayer) ? (PlayerTrans.position) : (targetTrans.position);
            Vector3 dir = (RaycastToPlayer) ? (playerToNewPos) : (-newDir);

            //If something ahead of you
            if (Physics.SphereCast(origin, CastWidth, dir, out fronthit, Mathf.Min(newDist, RayLength), CollisionLayers.value))
            {
                newDist = Mathf.MoveTowards(currentDist, fronthit.distance + camCollisionRadius, CameraAcceleration * Time.deltaTime);
            }
            else
            {
                newDist = Mathf.MoveTowards(currentDist, newDist, CameraAcceleration * Time.deltaTime);
            }
            Vector3 newPos = targetTrans.position - (newDir * newDist);

            //transform.position = newPos; //Set positions

            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
            transform.LookAt(targetTrans); //Set rotations
        }
        //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, PlayerTrans.rotation.eulerAngles.z));
    }

    Vector3 velocityAngle;
    float previousDist;

    Vector3 velocity = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(PlayerTrans.position - startOffset, 0.1f);
        if (Application.isPlaying)
        {

            Quaternion rotation = Quaternion.Euler(storedPos.y, storedPos.x, 0);
            Vector3 newDir = (rotation * offset).normalized;

            Gizmos.DrawLine(targetTrans.position, targetTrans.position - (newDir * storedPos.z));


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