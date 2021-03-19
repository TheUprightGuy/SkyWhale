using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGrappleScript : MonoBehaviour
{
    [Header("Dependencies")]
    public GameObject hook;
    public Transform camTarget;
    public Camera camToShootFrom;
    public GameObject grappleReticule;

    [Header("Grapple")]
    public LayerMask grappleableLayers;
    public float swingForce = 8.0f;
    public float yeetForce = 5.0f;
    public float retractExtendSpeed = 5.0f;
    public float minGrappleDist = 0.5f;
    public float maxGrappleDist = 10.0f;

    [Header("Camera")]
    public Vector3 aimOffset;
    public float camTransitionTime = 0.5f;


    [Header("Debug")]
    public float angleToForward = 0.0f;
    public float angleToLeft = 0.0f;

    public float percentageFToCenter = 0.0f;
    [HideInInspector]
    public bool grappleActive = false;
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
    Vector3 collidedFrameOffset => (collidedObj == null) ? (Vector3.zero) : (collidedObj.position - collidedprevPos);

    // Start is called before the first frame update
    void Start()
    {
        targetStartOffset = (camTarget.position - transform.position);
        cachedTargetPos = camTarget.position;
        calcGotoPos = transform.position + aimOffset;
        hookSJ = hook.GetComponent<SpringJoint>();
        cachedRB = GetComponent<Rigidbody>();
        hookLR = hook.GetComponent<LineRenderer>();

        //VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleRetract").MethodToCall.AddListener(GrappleRetract);
        //VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleExtend").MethodToCall.AddListener(GrappleExtend);

        ToggleAim(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))//Aiming with right click
        {
            ToggleAim(true);
        }

        if (Input.GetMouseButtonUp(1))//Stopping aiming
        {
            ToggleAim(false);
        }

        if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
        {
            FireHook();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            hook.GetComponent<NewGrappleHook>().YeetPlayer(this.transform);
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        NewGrappleHook temp = hook.GetComponent<NewGrappleHook>();
        Vector3 moveDir = Vector3.Normalize(hook.transform.position - transform.position) * 8.0f;

        if (temp.connected)
        {
            GetComponent<PlayerMovement>().enabled = false;
            //GetComponent<Rigidbody>().MovePosition(transform.position + moveDir * Time.fixedDeltaTime);
            GetComponent<Rigidbody>().AddForce(moveDir, ForceMode.Acceleration);
        }
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            GetComponent<PlayerMovement>().enabled = true;

        }

    }

    void FireHook()
    {
        NewGrappleHook temp = hook.GetComponent<NewGrappleHook>();

        //Ray screenRay = CamToShootFrom.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        if (!temp.connected && !temp.retracting && temp.flightTime <= 0.0f)
        {
            temp.Fire(this.transform, camToShootFrom.transform.forward);
        }
        else if ((temp.connected && !temp.retracting) || (temp.enabled && temp.flightTime > 0.0f))
        {
            temp.YeetPlayer(this.transform);
            temp.retracting = true;
            temp.connected = false;
        }
    }

    bool lerpDone = true;
    bool movingToAim = true;


    void ToggleAim(bool _startAim)
    {
        if (grappleReticule != null)
        {
            grappleReticule.SetActive(_startAim);
        }
        movingToAim = _startAim;
    }

    private void OnCollisionEnter(Collision collision)
    {
        NewGrappleHook temp = hook.GetComponent<NewGrappleHook>();
        if (temp.connected)
        {
            temp.connected = false;
            temp.retracting = true;
        }
    }
}
