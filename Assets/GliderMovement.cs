using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderMovement : MonoBehaviour
{
    [Header("Setup Fields")]
    public GameObject body;
    private Rigidbody rb;
    [Header("Movement")]
    public float currentSpeed = 0.0f;
    public float maxSpeed = 5.0f;


    #region Local Variables
    float rotationSpeed = 1.0f;
    Vector3 desiredVec;
    Vector3 desiredRoll;
    float myRoll = 0.0f;
    float myTurn = 0.0f;
    float myPitch = 0.0f;
    float turnSpeed = 40;
    public float liftSpeed = 20;
    float rollSpeed = 20;

    float moveSpeed = 1;
    float baseSpeed = 5.0f;
    float gravScale = 1.0f;
    #endregion Local Variables


    [Header("Testing")]
    public Vector3 position;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        position = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        desiredVec = body.transform.eulerAngles;
        VirtualInputs.GetInputListener(InputType.WHALE, "YawLeft").MethodToCall.AddListener(YawLeft);
        VirtualInputs.GetInputListener(InputType.WHALE, "YawRight").MethodToCall.AddListener(YawRight);
        VirtualInputs.GetInputListener(InputType.WHALE, "PitchDown").MethodToCall.AddListener(PitchDown);
        VirtualInputs.GetInputListener(InputType.WHALE, "PitchUp").MethodToCall.AddListener(PitchUp);

        baseSpeed = maxSpeed * 0.4f;
        moveSpeed = baseSpeed;
        currentSpeed = moveSpeed;
    }

    bool yawChange = false;
    void YawRight(InputState type)
    {
        if (myTurn + Time.deltaTime * turnSpeed < 40)
        {
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

    public float lerpDelay;
    public float lerpMultiplier = 1.0f;
    public float parabolLerp;

    public void MovementCorrections()
    {
        lerpDelay -= Time.fixedDeltaTime;

        float myLerpAmount = 0.0f;

        if (lerpTest)
        {
            parabolLerp += Time.deltaTime * Time.deltaTime * lerpMultiplier;
            parabolLerp = Mathf.Clamp01(parabolLerp);
            myLerpAmount = parabolLerp;
        }

        if (!yawChange)
        {
            if (lerpTest)
            {
                if (lerpDelay <= 0)
                {
                    
                    myTurn = Mathf.Lerp(myTurn, 0, Time.deltaTime * turnSpeed * myLerpAmount);
                    myRoll = Mathf.Lerp(myRoll, 0, Time.deltaTime * rollSpeed * 5 * myLerpAmount);
                }
            }
        }
        if (!pitchChange)
        {
            if (lerpTest)
            {
                if (lerpDelay <= 0)
                {
                    myPitch = Mathf.Lerp(myPitch, 0.0f, Time.deltaTime * myLerpAmount);
                    moveSpeed = Mathf.Lerp(moveSpeed, baseSpeed, Time.deltaTime * 0.5f);
                }
            }
        }

        yawChange = false;
        pitchChange = false;
    }

    bool lerpTest = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            lerpTest = !lerpTest;
            transform.position = position;
            moveSpeed = baseSpeed;
            currentSpeed = moveSpeed;
        }

        if (transform.forward.y < 0)
        {
            moveSpeed += (transform.forward.y * -20) * Time.fixedDeltaTime;
        }
        else
        {
            moveSpeed += (transform.forward.y * -7.0f) * Time.fixedDeltaTime;
        }

        moveSpeed = Mathf.Clamp(moveSpeed, 1.0f, maxSpeed);

        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime);
        MovementCorrections();

        RotatePlayer();

        lerping.SetText("LERPING: " + lerpTest.ToString());
        speed.SetText("SPEED: " + currentSpeed.ToString());
        gravity.SetText("GRAVITY: " + gravScale.ToString());
    }
    public TMPro.TextMeshProUGUI lerping;
    public TMPro.TextMeshProUGUI speed;
    public TMPro.TextMeshProUGUI gravity;



    public Transform playerRot;
    public void RotatePlayer()
    {
        float absAngle = Mathf.Abs(transform.forward.y) * 90.0f;

        playerRot.localRotation = Quaternion.Euler(Vector3.right * absAngle);
    }


    private void FixedUpdate()
    {
        desiredRoll = new Vector3(body.transform.eulerAngles.x, body.transform.eulerAngles.y, myRoll);
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.Euler(desiredRoll), Time.deltaTime * rotationSpeed);
        // Rot
        desiredVec = new Vector3(myPitch, transform.eulerAngles.y + myTurn, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0)), Time.deltaTime * 10.0f);

        gravScale = Mathf.Clamp01((0.5f - currentSpeed / maxSpeed) * 4.0f);
        Vector3 movementGrav = transform.forward * currentSpeed + gravScale * Physics.gravity;

        rb.MovePosition(transform.position + movementGrav * Time.deltaTime);       
    }
}
