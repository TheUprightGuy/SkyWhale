using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWhaleMovement : MonoBehaviour
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

    NewPickUp pickUp;
    NewOrbitScript orbit;
    NewCharacter cc;

    GameObject cachedHeightRef;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        orbit = GetComponent<NewOrbitScript>();
        pickUp = GetComponent<NewPickUp>();
        cc = GetComponentInChildren<NewCharacter>();
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

        Invoke("AddIsland", 0.1f);
    }

    // temp
    void AddIsland()
    {
        NewCallbackHandler.instance.SpawnCollectableIsland();
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
            myTurn = Mathf.Lerp(myTurn, 0, Time.deltaTime * turnSpeed);
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

        EventManager.TriggerEvent("Crash");
    }

    private void FixedUpdate()
    {
        desiredRoll = new Vector3(body.transform.eulerAngles.x, body.transform.eulerAngles.y, myRoll);
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.Euler(desiredRoll), Time.deltaTime * rotationSpeed);
        // Rot

        desiredVec = new Vector3(myPitch, transform.eulerAngles.y + myTurn, transform.eulerAngles.z);
        Vector3 temp = new Vector3(myPitch + transform.eulerAngles.x, myTurn + transform.eulerAngles.y, 0);

        if (!orbit.enabled)
        {
            //rb.angularVelocity = Quaternion.Slerp(transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed).eulerAngles;
            //rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed));


            //rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(path), Time.deltaTime * rotSpeed));

            Debug.DrawRay(transform.position, transform.forward * 1000.0f, Color.blue);
            Debug.DrawRay(transform.position, temp * 1000.0f, Color.black);

            float angleDiff = Vector3.Angle(transform.forward, Vector3.Normalize(temp));
            Vector3 cross = Vector3.Cross(transform.forward, Vector3.Normalize(temp));
            Vector3 rotVec = Vector3.ClampMagnitude(cross * angleDiff, 0.1f);
            //rb.angularVelocity = rotVec;

            /*float angleDiff = Vector3.Angle(transform.forward, path);
            Vector3 cross = Vector3.Cross(transform.forward, path);
            Vector3 rotVec = Vector3.ClampMagnitude(cross * angleDiff, rotSpeed);
            rb.angularVelocity = rotVec;*/

        }
        //rb.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0)), Time.deltaTime * 10.0f));

        if (tooClose)
        {
            //rb.MovePosition(transform.position - transform.forward * maxSpeed * Time.deltaTime);
            rb.velocity = -transform.forward * maxSpeed;
            buckTimer -= Time.deltaTime;
            CatapultPlayer();

            if (buckTimer <= 0)
            {
                tooClose = false;
                orbit.SetOrbit(cachedHeightRef);
            }
        }
        else
        {
            //rb.velocity = transform.forward * currentSpeed;// * Time.deltaTime;
            //rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.deltaTime);
        }
    }

    public void CatapultPlayer()
    {
        if (cc)
        {
            cc.transform.parent = null;
            cc.Punt();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<NewIslandScript>())
        {
            Crash(other.gameObject);
            Destroy(other.gameObject);
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
