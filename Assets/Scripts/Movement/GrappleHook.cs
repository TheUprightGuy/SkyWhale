using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [Header("Dependencies")]
    public GameObject Hook;
    public Transform CamTarget;
    public Camera CamToShootFrom;

    [Header("Grapple")]
    public LayerMask GrappleableLayers;
    public float MaxGrappleDist = 10.0f;

    [Header("Camera")]
    public Vector3 AimOffset;
    public float CamTransitionTime = 0.5f;

    [HideInInspector]
    public bool GrappleActive = false;
    float storedSpringVal;
    Rigidbody storedRB;
    SpringJoint storedSJ;

    Vector3 cachedTargetPos;
    Vector3 targetStartOffset;
    Vector3 calcGotoPos;
    // Start is called before the first frame update
    void Start()
    {
        targetStartOffset = ( CamTarget.position - transform.position);
        cachedTargetPos = CamTarget.position;
        calcGotoPos = transform.position + AimOffset;
        storedSJ = Hook.GetComponent<SpringJoint>();
        SpringJointActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GrappleActive)//If no grapple is there
        {
            if (Input.GetMouseButtonDown(1))//Aiming with right click
            {
                ToggleAimPos(true);
            }
            if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1) && LerpDone) //Aim done, rightclick held, and left click pressed
            {
                FireHook();
            }
            if (Input.GetMouseButtonUp(1))//Stopping aiming
            {
                ToggleAimPos(false);
            }
        }
        else//If grapple is already there
        {
            if (collidedObj != null)
            {
                Vector3 offset = GetCollidedFrameOffset();
                Hook.GetComponent<Rigidbody>().MovePosition(Hook.transform.position + offset); //apply movement with any moving objects latched too
            }

            if (Input.GetMouseButtonDown(0))//On left click
            {
                RetractHook();//Release
                
            }
        }

        if (collidedObj != null) //If attached to something
        {
            collidedprevPos = collidedObj.position;
        }
    }

    void FireHook()
    {
        Ray screenRay = CamToShootFrom.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

        RaycastHit hit;
        if (Physics.Raycast(CamToShootFrom.transform.position, CamToShootFrom.transform.forward, out hit, MaxGrappleDist, GrappleableLayers.value))
        {
            SpringJointActive(true);
            Hook.transform.position = hit.point;
            storedSJ.maxDistance = Vector3.Distance(transform.position, Hook.transform.position);//Distance for the hook to go from player to hook
            GrappleActive = true;
            collidedObj = hit.transform;
            collidedprevPos = collidedObj.position;
        }
    }

    void RetractHook()
    {
        SpringJointActive(false);
        GrappleActive = false;
    }

    public void ApplyForces()
    {

    }

    Vector3 collidedprevPos = Vector3.zero;
    Transform collidedObj = null; //latched onto obj
    Vector3 GetCollidedFrameOffset() //The movement vector since the last frame;
    {
        return (collidedObj == null) ? (Vector3.zero) :
            (collidedObj.position - collidedprevPos);
    }
    void SpringJointActive(bool _active)
    {
        Hook.SetActive(_active);
    }


    bool LerpDone = true;
    bool MovingToAim = true;
    //No one look in here
    //It works okay?
    IEnumerator MoveTargetToPos()
    {
        LerpDone = false;
        float elapsedTime = 0;
        float waitTime = CamTransitionTime;
        Vector3 startPos = CamTarget.position;
        while (elapsedTime < waitTime)
        {
            Vector3 initPos = transform.position
                + (transform.right * targetStartOffset.x)
                + (transform.forward * targetStartOffset.z)
                + (transform.up * targetStartOffset.y);

            Vector3 aimPos = transform.position 
                + (transform.right * AimOffset.x) 
                + (transform.forward * AimOffset.z) 
                + (transform.up * AimOffset.y);
            Vector3 endPos = (MovingToAim) ? (aimPos) : (initPos);

            CamTarget.position = Vector3.Lerp(startPos, endPos, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
        LerpDone = true;
    }

    /// <summary>
    /// Lerps the camera target from cachedStartPos to AimOffset or visa versa
    /// </summary>
    /// <param name="_MovingToAim">if true goes to aimoffset, if false goes back to start</param>
    void ToggleAimPos(bool _MovingToAim)
    {
        MovingToAim = _MovingToAim;

        IEnumerator cor = MoveTargetToPos();
        StartCoroutine(cor);
    }

    //private void OnDisable()
    //{
    //    Hook.SetActive(false);
    //}
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Ray screenRay = CamToShootFrom.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));


        RaycastHit hit;
        if (Physics.Raycast(CamToShootFrom.transform.position,CamToShootFrom.transform.forward, out hit, MaxGrappleDist, GrappleableLayers.value))
        {
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }
}
