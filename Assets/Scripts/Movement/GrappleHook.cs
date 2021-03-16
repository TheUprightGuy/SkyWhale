using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [Header("Dependencies")]
    public GameObject Hook;
    public Transform CamTarget;
    public Camera CamToShootFrom;
    public GameObject GrappleReticule;
    
    [Header("Grapple")]
    public LayerMask GrappleableLayers;
    public float SwingForce = 8.0f;
    public float YeetForce = 5.0f;
    public float RetractExtendSpeed = 5.0f;
    public float MinGrappleDist = 0.5f;
    public float MaxGrappleDist = 10.0f;

    [Header("Camera")]
    public Vector3 AimOffset;
    public float CamTransitionTime = 0.5f;


    [Header("Debug")]
    public float AngleToForward = 0.0f;
    public float AngleToLeft = 0.0f;

    public float PercentageFToCenter = 0.0f;
    [HideInInspector]
    public bool GrappleActive = false;
    float storedSpringVal;
    Rigidbody cachedRB;
    SpringJoint hookSJ;
    LineRenderer hookLR;

    Vector3 cachedTargetPos;
    Vector3 targetStartOffset;
    Vector3 calcGotoPos;

    Vector3 collidedprevPos = Vector3.zero;
    Transform collidedObj = null; //latched onto obj
    //The movement vector since the last frame;
    Vector3 CollidedFrameOffset => (collidedObj == null) ? (Vector3.zero) : (collidedObj.position - collidedprevPos);

    // Start is called before the first frame update
    void Start()
    {
        targetStartOffset = ( CamTarget.position - transform.position);
        cachedTargetPos = CamTarget.position;
        calcGotoPos = transform.position + AimOffset;
        hookSJ = Hook.GetComponent<SpringJoint>();
        cachedRB = GetComponent<Rigidbody>();
        hookLR = Hook.GetComponent<LineRenderer>();

        VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleRetract").MethodToCall.AddListener(GrappleRetract);
        VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleExtend").MethodToCall.AddListener(GrappleExtend);
        
        SpringJointActive(false);
        ToggleAim(false);
    }

    private void Update()
    {
        if (hookLR != null)//Got a line renderer
        {
            UpdateLine();
        }

        if (Input.GetMouseButtonDown(1))//Aiming with right click
        {
            ToggleAim(true);
        }

        if (Input.GetMouseButtonUp(1))//Stopping aiming
        {
            ToggleAim(false);
        }


        if (collidedObj != null)
        {
            Vector3 offset = CollidedFrameOffset;
        }

        if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1) /*&& LerpDone*/) //Aim done, rightclick held, and left click pressed
        {
            FireHook();
        }
        if (Input.GetMouseButtonUp(1))//On right click release
        {
            RetractHook();//Release

        }

        //if (!GrappleActive)//If no grapple is there
        //{
            
        //}
        //else//If grapple is already there
        //{
           
        //}

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (collidedObj != null) //If attached to something
        {
            Vector3 offset = CollidedFrameOffset;
            Hook.GetComponent<Rigidbody>().MovePosition(Hook.transform.position + offset); //apply movement with any moving objects latched too
            collidedprevPos = collidedObj.position;
        }
    }

    void FireHook()
    {
        //Ray screenRay = CamToShootFrom.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

        RaycastHit hit;
        if (Physics.Raycast(CamToShootFrom.transform.position, CamToShootFrom.transform.forward, out hit, MaxGrappleDist, GrappleableLayers.value))
        {
            SpringJointActive(true);
            Hook.transform.position = hit.point;
            hookSJ.maxDistance = Vector3.Distance(transform.position, Hook.transform.position);//Distance for the hook to go from player to hook
            GrappleActive = true;
            collidedObj = hit.transform;
            collidedprevPos = collidedObj.position;
        }
    }

    void RetractHook()
    {
        YeetPlayer();
        SpringJointActive(false);
        GrappleActive = false;
    }

    public void ApplyForces(Vector3 inputAxis)
    {
        Vector3 playerToHook = (Hook.transform.position - transform.position).normalized;
        //Place all forces on the place of the rope

        //Fully forward = 0.0f, hanging = 90.0f, fully back = 0.0f;
        AngleToForward = Vector3.Angle(transform.forward, playerToHook);
                                            //Difference
        PercentageFToCenter = 1.0f - (Mathf.Abs(90.0f  - AngleToForward) / 90.0f);
        Vector3 xAxis = transform.forward;//Vector3.ProjectOnPlane(transform.forward, playerToHook).normalized;
        xAxis *= inputAxis.x;

        //Fully left = 0.0f, hanging = 90.0f, fully back = 0.0f;
        AngleToLeft = Vector3.Angle(transform.right, playerToHook);
        PercentageFToCenter = 1.0f - (Mathf.Abs(90.0f - AngleToForward) / 90.0f);
        Vector3 zAxis = -transform.right; //Vector3.ProjectOnPlane(-transform.right, playerToHook).normalized;
        zAxis *= inputAxis.z;

        Vector3 inputDir = (xAxis + zAxis).normalized * SwingForce;
        //Ensure this can never have a force greater than gravity.
        //inputDir = Vector3.ClampMagnitude(inputDir, Physics.gravity.magnitude * 0.95f);
        cachedRB.AddForce(inputDir);
    }

    public void YeetPlayer()
    {
        //Make sure can't multiply the impulse and create a railgun
        float velMag = Mathf.Clamp01(cachedRB.velocity.magnitude);

        //if not moving at all, doesn't yeet.
        //If moving, yeets hard
        float yeetForce = YeetForce * velMag;


        Vector3 playerToHook = (Hook.transform.position - transform.position).normalized;
        Vector3 velDir = cachedRB.velocity.normalized;//Direction moving
        Vector3 yeetDir = Vector3.ProjectOnPlane(velDir, playerToHook).normalized; //Project along arc of swing

        Vector3 yeetImpulse = yeetDir * yeetForce;
        cachedRB.AddForce(yeetImpulse, ForceMode.Impulse); 
    }
    void SpringJointActive(bool _active)
    {
        Hook.SetActive(_active);
    }

    void UpdateLine()
    {
        hookLR.SetPosition(0, Hook.transform.position);
        hookLR.SetPosition(1, transform.position);
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
    /// <param name="_startAim">if true goes to aimoffset, if false goes back to start</param>
    void ToggleAim(bool _startAim)
    {
        if (GrappleReticule != null)
        {
            GrappleReticule.SetActive(_startAim);
        }

        
        MovingToAim = _startAim;

        //Aim disabled
        //IEnumerator cor = MoveTargetToPos();
        //StartCoroutine(cor);
    }


    void GrappleRetract(InputState _event)
    {
        hookSJ.maxDistance = Mathf.Max(hookSJ.maxDistance - (RetractExtendSpeed * Time.deltaTime), MinGrappleDist);
    }

    void GrappleExtend(InputState _event)
    {
        hookSJ.maxDistance = Mathf.Min(hookSJ.maxDistance + (RetractExtendSpeed * Time.deltaTime), MaxGrappleDist);
    }
    //private void OnDisable()
    //{
    //    Hook.SetActive(false);
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + AimOffset, 0.1f);
        //Ray screenRay = CamToShootFrom.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        RaycastHit hit;
        if (Physics.Raycast(CamToShootFrom.transform.position,CamToShootFrom.transform.forward, out hit, MaxGrappleDist, GrappleableLayers.value))
        {
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }
}
