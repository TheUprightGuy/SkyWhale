using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleMovement : MonoBehaviour
{
    [Header("Setup Fields")]
    public GameObject body;
    private Rigidbody rb;
    public Animator animator;
    [Header("Movement")]
    public float currentSpeed = 0.0f;
    public float moveSpeed = 1;
    public float accelSpeed = 1;
    public float maxSpeed = 5.0f;
    public float minimumDistance = 15.0f;
    [Header("Dismount")]
    private Transform dismountPosition; 
    private Transform grapplePos; 

    #region Local Variables
    //[HideInInspector] 
    public bool tooClose;
    //[HideInInspector]
    public bool control = true;
    float buckTimer = 0.0f;
    float distance;
    float rotationSpeed = 0.2f;
    Vector3 desiredVec;
    Vector3 desiredRoll;
    float myRoll = 0.0f;
    public float myTurn = 0.0f;
    public float myPitch = 0.0f;
    float turnSpeed = 40;
    float liftSpeed = 20;
    float rollSpeed = 20;
    #endregion Local Variables

    PickUp pickUp;
    OrbitScript orbit;
    NewCharacter cc;

    GameObject cachedHeightRef;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        orbit = GetComponent<OrbitScript>();
        pickUp = GetComponent<PickUp>();
        cc = GetComponentInChildren<NewCharacter>();
        dismountPosition = GameObject.Find("DismountPos").transform;
        grapplePos = GameObject.Find("GrapplePos").transform; 
    }

    // Start is called before the first frame update
    void Start()
    {
        //orbit.enabled = false;
        desiredVec = body.transform.eulerAngles;

        VirtualInputs.GetInputListener(InputType.WHALE, "YawLeft").MethodToCall.AddListener(YawLeft);
        VirtualInputs.GetInputListener(InputType.WHALE, "YawRight").MethodToCall.AddListener(YawRight);
        VirtualInputs.GetInputListener(InputType.WHALE, "PitchDown").MethodToCall.AddListener(PitchDown);
        VirtualInputs.GetInputListener(InputType.WHALE, "PitchUp").MethodToCall.AddListener(PitchUp);
        VirtualInputs.GetInputListener(InputType.WHALE, "Thrust").MethodToCall.AddListener(Thrust);
        VirtualInputs.GetInputListener(InputType.WHALE, "Dismount").MethodToCall.AddListener(Dismount);
        VirtualInputs.GetInputListener(InputType.WHALE, "Grapple").MethodToCall.AddListener(Grapple);
        CallbackHandler.instance.grappleHitFromWhale += transform1 => control = false; 

        Invoke("AddIsland", 0.1f);
    }

    // temp
    void AddIsland()
    {
        CallbackHandler.instance.SpawnCollectableIsland();
    }
    
    private void Grapple(InputState arg0) 
    { 
        if (!control) return; 
        CallbackHandler.instance.GrappleFromWhale(grapplePos); 
    }
    
    private void Dismount(InputState arg0)
    {
        if (!control) return;
        control = false;
        CallbackHandler.instance.DismountPlayer(dismountPosition);
    }

    bool yawChange = false;
    void YawRight(InputState type)
    {
        if (orbit.enabled)
            return;

        if (myTurn + Time.deltaTime * turnSpeed < 40)
        {
            myTurn += Time.deltaTime * turnSpeed;
        }

        if (myRoll - Time.deltaTime * rollSpeed > -10)
        {
            myRoll -= Time.deltaTime * rollSpeed;
        }
        yawChange = true;
    }
    void YawLeft(InputState type)
    {
        if (orbit.enabled)
            return;

        if (myTurn - Time.deltaTime * turnSpeed > -40)
        {
            myTurn -= Time.deltaTime * turnSpeed;
        }

        if (myRoll + Time.deltaTime * rollSpeed < 10)
        {
            myRoll += Time.deltaTime * rollSpeed;
        }
        yawChange = true;
    }

    bool pitchChange = false;
    void PitchDown(InputState type)
    {
        if (orbit.enabled)
            return;

        if (myPitch + Time.deltaTime * liftSpeed < 30)
        {
            myPitch += Time.deltaTime * liftSpeed;
        }
        pitchChange = true;
    }
    void PitchUp(InputState type)
    {
        if (orbit.enabled)
            return;

        if (myPitch - Time.deltaTime * liftSpeed > -30)
        {
            myPitch -= Time.deltaTime * liftSpeed;
        }
        pitchChange = true;
    }

    bool thrustChange = false;
    private void Thrust(InputState type)
    {
        if (orbit.enabled)
            return;

        if (moveSpeed < maxSpeed)
        {
            moveSpeed += accelSpeed * Time.deltaTime;
        }

        thrustChange = true;
    }

    public void MovementCorrections()
    {
        if (!yawChange)
        {
            myTurn = Mathf.Lerp(myTurn, 0, Time.deltaTime * turnSpeed / 8.0f);
            myRoll = Mathf.Lerp(myRoll, 0, Time.deltaTime * rollSpeed * 5);
        }
        if (!pitchChange)
        {
            myPitch = Mathf.Lerp(myPitch, 0.0f, Time.deltaTime);
        }
        if (!thrustChange)
        {
            if (moveSpeed > maxSpeed / 2)
            {
                moveSpeed -= accelSpeed * Time.deltaTime;
            }
            else if (moveSpeed > maxSpeed / 3)
            {
                moveSpeed -= accelSpeed * (Time.deltaTime / 2);
            }
            else if (moveSpeed > 1.0f)
            {
                moveSpeed -= accelSpeed * Time.deltaTime * 0.1f;
            }
        }

        yawChange = false;
        pitchChange = false;
        thrustChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        float movement = currentSpeed / 2;
        float f = body.transform.rotation.eulerAngles.z;
        f = (f > 180) ? f - 360 : f;
        animator.SetFloat("Turning", f / 10.0f);
        animator.SetFloat("Movement", movement);
        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * accelSpeed);

        if (control)
        {
            MovementCorrections();
        }
        GetDistance();

        Movement();
    }

    public void GetDistance()
    {
        Debug.DrawRay(transform.position, transform.forward * Mathf.Infinity, Color.green);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, GetComponent<CapsuleCollider>().radius, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Island"))
            {
                distance = hit.distance;
                if (distance < minimumDistance && buckTimer <= 0.0f)
                {
                    Crash(hit.transform.gameObject);
                }
            }
        }
    }

    void Crash(GameObject hit)
    {
        tooClose = true;
        control = false;
        buckTimer = 2.0f;

        NewIslandScript temp = (hit.GetComponent<NewIslandScript>()) ? hit.GetComponent<NewIslandScript>() : hit.GetComponentInParent<NewIslandScript>();
        cachedHeightRef = temp.heightRef;

        // Quest Example
        //EventManager.TriggerEvent("Crash");
    }

    private void FixedUpdate()
    {
        if (tooClose)
        {
            rb.MovePosition(transform.position - transform.forward * maxSpeed * Time.deltaTime);
            buckTimer -= Time.deltaTime;
            CatapultPlayer();

            if (buckTimer <= 0)
            {
                tooClose = false;
                moveSpeed = maxSpeed / 2.0f;
                orbit.SetOrbit(cachedHeightRef);
            }
        }
        else
        {
            rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.deltaTime * TimeSlowDown.instance.timeScale);
        }

        if (!control)
            return;

        Quaternion temp = Quaternion.Slerp(transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed * TimeSlowDown.instance.timeScale);
        rb.MoveRotation(temp);
    }

    void Movement()
    {
        if (!control)
            return;

        desiredRoll = new Vector3(body.transform.eulerAngles.x, body.transform.eulerAngles.y, myRoll);
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.Euler(desiredRoll), Time.deltaTime * rotationSpeed * TimeSlowDown.instance.timeScale);
        // Rot
        desiredVec = new Vector3(myPitch, transform.eulerAngles.y + myTurn, 0);
    }

    public void CatapultPlayer()
    {
        if (cc)
        {
            cc.transform.parent = null;
            cc.Punt();
        }
    }

    void ComeToHalt()
    {
        moveSpeed = 0.0f;
        currentSpeed = 0.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject.GetComponentInParent<NewIslandScript>())
        {
            Crash(other.gameObject);
            Destroy(other.gameObject);
        }*/

        PlayerMovement pc = other.GetComponent<PlayerMovement>();
        NewGrappleHook gh = other.GetComponent<NewGrappleHook>(); //Temp
        if (pc || gh)
        {
            if (gh)
            {
                gh.ResetHook();
                gh.flightTime = 0f;
                gh.transform.position = new Vector3(0,1000f,0f);
            }
            ComeToHalt();
            control = true;
            CallbackHandler.instance.MountWhale();
        }
    }

    public void GiveControl()
    {
        orbit.enabled = false;
        pickUp.enabled = false;
        control = true;
        tooClose = false;
    }
}
