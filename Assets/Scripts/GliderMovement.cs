using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderMovement : MonoBehaviour
{
    [Header("Setup Fields")]
    public Transform playerRot;
    public float maxSpeed = 5.0f;
    public float recenterFactor = 5.0f;
    public GameObject glider;
    new public bool enabled;

    public Cinemachine.CinemachineVirtualCamera mainCam;
    public Cinemachine.CinemachineVirtualCamera glideCam;
    DumbCamera dc;

    #region Local Variables
    public float currentSpeed = 0.0f;
    private Rigidbody rb;
    float rotationSpeed = 1.0f;
    Vector3 desiredVec;
    Vector3 desiredRoll;
    public float myRoll = 0.0f;
    public float myTurn = 0.0f;
    public float myPitch = 0.0f;
    float turnSpeed = 40;
    float liftSpeed = 20;
    float rollSpeed = 20;

    float parabolLerp;
    public float moveSpeed = 1;
    public float baseSpeed = 5.0f;
    float gravScale = 1.0f;
    #endregion Local Variables

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        dc = glideCam.GetComponent<DumbCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        desiredVec = transform.transform.eulerAngles;
        VirtualInputs.GetInputListener(InputType.WHALE, "YawLeft").MethodToCall.AddListener(YawLeft);
        VirtualInputs.GetInputListener(InputType.WHALE, "YawRight").MethodToCall.AddListener(YawRight);
        VirtualInputs.GetInputListener(InputType.WHALE, "PitchDown").MethodToCall.AddListener(PitchDown);
        VirtualInputs.GetInputListener(InputType.WHALE, "PitchUp").MethodToCall.AddListener(PitchUp);

        baseSpeed = maxSpeed * 0.4f;
        moveSpeed = baseSpeed;
        currentSpeed = 0.0f;
    }

    public void Toggle()
    {
        enabled = !enabled;
        glider.SetActive(enabled);
        myRoll = 0.0f;
        myPitch = 0.0f;
        myTurn = 0.0f;
        currentSpeed = baseSpeed;

        CameraManager.instance.SwitchCamera((enabled) ? CameraType.GlideCamera : CameraType.PlayerCamera);

        mainCam.GetComponent<ThirdPersonCamera>().SetRotation(glideCam.transform);
        rb.angularVelocity = Vector3.zero;

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }


    bool yawChange = false;
    void YawRight(InputState type)
    {
        if (myTurn + Time.deltaTime * turnSpeed < 40)
        {
            if (myTurn + Time.deltaTime * turnSpeed < 0)
            {
                myTurn += Time.deltaTime * turnSpeed;
            }
            myTurn += Time.deltaTime * turnSpeed;
        }

        if (myRoll - Time.deltaTime * rollSpeed > -20)
        {
            myRoll -= Time.deltaTime * rollSpeed;
        }
        yawChange = true;
        parabolLerp = 0.0f;
    }
    void YawLeft(InputState type)
    {
        if (myTurn - Time.deltaTime * turnSpeed > -40)
        {
            if (myTurn - Time.deltaTime * turnSpeed > 0)
            {
                myTurn -= Time.deltaTime * turnSpeed;
            }
            myTurn -= Time.deltaTime * turnSpeed;
        }

        if (myRoll + Time.deltaTime * rollSpeed < 20)
        {
            myRoll += Time.deltaTime * rollSpeed;
        }
        yawChange = true;
        parabolLerp = 0.0f;
    }

    bool pitchChange = false;
    void PitchDown(InputState type)
    {
        if (myPitch + Time.deltaTime * liftSpeed < 45)
        {
            myPitch += Time.deltaTime * liftSpeed;
        }
        pitchChange = true;
        parabolLerp = 0.0f;
    }
    void PitchUp(InputState type)
    {
        if (myPitch - Time.deltaTime * liftSpeed > -45)
        {
            myPitch -= Time.deltaTime * liftSpeed;
        }
        pitchChange = true;
        parabolLerp = 0.0f;
    }


    public void MovementCorrections()
    {
        parabolLerp += Time.deltaTime * Time.deltaTime * recenterFactor;
        parabolLerp = Mathf.Clamp01(parabolLerp);

        if (!yawChange)
        {              
            myTurn = Mathf.Lerp(myTurn, 0, Time.deltaTime * turnSpeed * parabolLerp);
            myRoll = Mathf.Lerp(myRoll, 0, Time.deltaTime * rollSpeed * 5 * parabolLerp);            
        }
        if (!pitchChange)
        {      
            myPitch = Mathf.Lerp(myPitch, 0.0f, Time.deltaTime * parabolLerp);
            moveSpeed = Mathf.Lerp(moveSpeed, baseSpeed, Time.deltaTime * 0.5f);                     
        }

        yawChange = false;
        pitchChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Toggle();
        }*/

        if (!enabled) 
            return;

        dc.SetDistance(currentSpeed / maxSpeed);

        if (base.transform.forward.y < 0)
        {
            moveSpeed += (base.transform.forward.y * -(maxSpeed / 1.5f)) * Time.fixedDeltaTime;
        }
        else
        {
            moveSpeed += (base.transform.forward.y * -(maxSpeed / 3.0f)) * Time.fixedDeltaTime;
        }

        moveSpeed = Mathf.Clamp(moveSpeed, 1.0f, maxSpeed);

        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime);
        MovementCorrections();

        RotatePlayer();

        // Rot
        desiredVec = new Vector3(myPitch, base.transform.eulerAngles.y + myTurn, myRoll);

        base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed);
    }

    public void RotatePlayer()
    {
        float absAngle = Mathf.Abs(base.transform.forward.y) * 90.0f;

        playerRot.localRotation = Quaternion.Euler(Vector3.right * absAngle);
    }

    private void FixedUpdate()
    {
        if (!enabled)
            return;

       /* // Rot
        desiredVec = new Vector3(myPitch, base.transform.eulerAngles.y + myTurn, myRoll);

        base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed);*/
        
        gravScale = Mathf.Clamp01(0.7f - currentSpeed / maxSpeed) * 40.0f;
         Vector3 movementGrav = base.transform.forward * currentSpeed + gravScale * Physics.gravity * Time.deltaTime;

        rb.velocity = movementGrav; 
    }
}
