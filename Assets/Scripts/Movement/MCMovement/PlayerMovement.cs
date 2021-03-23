using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerStates
    {
        IDLE,
        MOVING,
        JUMPING,
        FALLING,
        CLIMBING,
        GRAPPLE,
        GLIDING
    }

    Rigidbody RB;
    Animator anims;
    GliderMovement glider;
    [Tooltip("This can't be set, and sets to IDLE on Start call, so no touchy")]
    public PlayerStates PlayerState;

    [Header("Forces")]
    public float walkSpeed = 2.0f;
    public float maxWalkAcceleration = 10f;
    [Space(20.0f)]

    public float runSpeed = 6.0f;
    public float maxRunAcceleration = 20f;

    public float currentSpeed;

    [Space(20.0f)]
    private float setSpeed;
    private float setAccel;

    [Space(20.0f)]
    public float inAirSpeed = 1.0f;
    public float maxAirAcceleration = 10.0f;
    [Space(20.0f)]

    public float climbSpeed = 1.0f;
    public float maxClimbAcceleration = 10.0f;
    public float climbGripForce = 1.0f;
    [Space(20.0f)]
    public float groundjumpHeight = 5.0f;
    public float wallJumpHeight = 5.0f;

    float maxGroundAngle = 40.0f;
    float maxClimbAngle = 60.0f;

    [Header("GroundChecks")]
    public LayerMask GroundLayers;
    public LayerMask ClimbLayers;
    public float GroundCheckDistance = 1.0f;
    public float ClimbCheckDistance = 1.0f;
    public float CheckRadius = 0.1f;
    public Vector3 GroundCheckStartOffset = Vector3.zero;
    public Vector3 ClimbCheckStartOffset = Vector3.zero;
    [Header("Misc")]
    public bool LocalToGround = true;

    public GameObject collidedObj = null;

    new public bool enabled;

    void Start()
    {
        setSpeed = walkSpeed;
        setAccel = maxWalkAcceleration;

        PlayerState = PlayerStates.IDLE;

        RB = GetComponent<Rigidbody>();
        anims = GetComponentInChildren<Animator>();
        glider = GetComponent<GliderMovement>();
        VirtualInputs.GetInputListener(InputType.PLAYER, "Forward").MethodToCall.AddListener(Forward);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Back").MethodToCall.AddListener(Back);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Left").MethodToCall.AddListener(Left);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Right").MethodToCall.AddListener(Right);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Run").MethodToCall.AddListener(Run);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Jump").MethodToCall.AddListener(Jump);

        //RB.useGravity = false;
        OnValidate();
    }

    private void Update()
    {
        SetAnimations();

        if (!enabled)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && (!IsGrounded() || glider.enabled))
        {
            glider.Toggle();
        }

        Vector3 camForwardRelativeToPlayerRot = Vector3.Normalize(Vector3.ProjectOnPlane(cam.forward, transform.up));
        rot = Quaternion.FromToRotation(transform.forward, camForwardRelativeToPlayerRot);


        if ((inputAxis.magnitude > 0.1f && !glider.enabled) || Input.GetKey(KeyCode.O))
        {
            transform.Rotate(rot.eulerAngles, Space.World);
        }

        Debug.DrawRay(transform.position, camForwardRelativeToPlayerRot, Color.black);
    }

    public Quaternion rot;

    public Transform cam;
    public Vector3 inputAxis = Vector3.zero;
    void FixedUpdate()
    {
        SetCurrentPlayerState();

        if (!enabled)
            return;

        HandleMovement();
        /*if (inputAxis != Vector3.zero)
        {
            inputAxis = Vector3.zero;
        }*/
        
        //groundContactNormal = climbContactNormal = Vector3.zero;
        //groundContactCount = climbContactCount = 0;
    }

    float minGroundDotProduct;
    float minClimbDotProduct;
    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }

    void SetCurrentPlayerState()
    {
        if (MOVINGCheck())
        {
            PlayerState = PlayerStates.MOVING;
        }
        else if (IDLECheck())
        {
            PlayerState = PlayerStates.IDLE;
        }

        if (FALLINGCheck())
        {
            PlayerState = PlayerStates.FALLING;
        }

        if (JUMPINGCheck())
        {
            PlayerState = PlayerStates.JUMPING;
        }
        if (GRAPPLECheck())
        {
            PlayerState = PlayerStates.GRAPPLE;
        }
        if (GLIDINGCheck())
        {
            PlayerState = PlayerStates.GLIDING;

        }
        if (CLIMBINGCheck())
        {
            PlayerState = PlayerStates.CLIMBING;
        }
    }


    #region PlayerStateChecks
    bool IDLECheck()
    {
        return (IsGrounded() && inputAxis.magnitude <= 0.0f);
        //return RB.velocity.magnitude <= 0.1f;
    }

    bool MOVINGCheck()
    {
        return (IsGrounded() && inputAxis.magnitude > 0.0f);
        //return RB.velocity.magnitude > 0.1f;
    }

    bool GRAPPLECheck()
    {
        //Only check if available, currently hooked, and on the ground to allow grapple movement
        //return (gHook != null && gHook.enabled && gHook.GrappleActive && !IsGrounded());
        return false;
    }
    bool CLIMBINGCheck()//ToDo Later when implementing climbing
    {
        return (!IsGrounded() && IsClimbing());
    }
    bool JUMPINGCheck()
    {
        float currenty = Vector3.Dot(RB.velocity, transform.up);
        return currenty > 0.0f && !IsGrounded() && !IsClimbing();
    }
    bool FALLINGCheck()
    {


        float currenty = Vector3.Dot(RB.velocity, transform.up);
        return currenty < 0.0f && !IsGrounded() && !IsClimbing();
    }

    bool GLIDINGCheck()
    {
        return (glider != null && glider.enabled && !IsGrounded() &&
                PlayerState != PlayerStates.CLIMBING && PlayerState != PlayerStates.GRAPPLE);
    }
    #endregion


    void HandleMovement()
    {
                //RB.useGravity = true;
        if (inputAxis == Vector3.zero)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, maxWalkAcceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, setSpeed, maxWalkAcceleration * Time.deltaTime);
        }




        switch (PlayerState)
        {
            case PlayerStates.IDLE:
            case PlayerStates.MOVING:
                MoveOnXZ(currentSpeed, setAccel);
                if (inputAxis.y > 0)
                {
                    Jump(groundContactNormal + Vector3.up, groundjumpHeight);
                }
                break;
            case PlayerStates.GRAPPLE:
                {
                    //gHook.ApplyForces(inputAxis);
                }
                break;
            case PlayerStates.CLIMBING:
                

                if (inputAxis.y > 0)
                {
                    Jump(climbContactNormal + Vector3.up, wallJumpHeight);
                }
                break;
            case PlayerStates.GLIDING:
                {
                    //glider.ApplyForces(inputAxis);
                }
                break;
            case PlayerStates.JUMPING:
            case PlayerStates.FALLING:
                MoveOnXZ(inAirSpeed, maxAirAcceleration);
                break;
            default:
                break;
        }
        //inputAxis = Vector3.zero;
    }

    void MoveOnXZ(float speed, float accel)
    {
        Vector3 xAxis = Vector3.ProjectOnPlane(transform.forward, groundContactNormal);
        Vector3 zAxis = Vector3.ProjectOnPlane(-transform.right, groundContactNormal);
        xAxis *= inputAxis.x;
        zAxis *= inputAxis.z;

        inputAxis = Vector3.Normalize(inputAxis);
        //Vector3 temp = Vector3.Dot(inputAxis, transform.forward);


        float currentX = Vector3.Dot(RB.velocity, xAxis);
        float currentZ = Vector3.Dot(RB.velocity, zAxis);

        float acceleration = accel;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;

        Vector3 desiredVel = xAxis + zAxis;
        desiredVel.y = 0;
        desiredVel *= speed;


        float newX =
            Mathf.MoveTowards(currentX, desiredVel.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVel.z, maxSpeedChange);

        //RB.velocity += (xAxis * (newX - currentX) + zAxis * (newZ - currentZ)) * TimeSlowDown.instance.timeScale;

        RB.MovePosition(transform.position + desiredVel * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale);


        /*Vector3 manualGrav = groundContactNormal * -9.81f * TimeSlowDown.instance.timeScale;
        if (groundContactNormal == Vector3.zero)
        {
            RB.AddForce(Physics.gravity, ForceMode.Acceleration);
            Debug.DrawRay(transform.position, Physics.gravity, Color.black);
        }
        else
        {
            RB.AddForce(manualGrav, ForceMode.Acceleration);
            Debug.DrawRay(transform.position, manualGrav, Color.gray);
        }*/

        /*if (collidedObj != null)
        {
            Vector3 offset = GetCollidedFrameOffset();
            RB.MovePosition(transform.position + offset);
        }*/
    }
    void MoveOnXY(float speed, float accel)
    {
        Vector3 xAxis = Vector3.ProjectOnPlane(transform.up, climbContactNormal);
        Vector3 zAxis = Vector3.ProjectOnPlane(-transform.right, climbContactNormal);
        xAxis *= inputAxis.x;
        zAxis *= inputAxis.z;


        inputAxis = Vector3.Normalize(inputAxis);
        //Vector3 temp = Vector3.Dot(inputAxis, transform.forward);


        float currentX = Vector3.Dot(RB.velocity, xAxis);
        float currentZ = Vector3.Dot(RB.velocity, zAxis);

        float acceleration = accel;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;

        Vector3 desiredVel = xAxis + zAxis;
        desiredVel.y = 0;
        desiredVel *= speed;


        float newX =
            Mathf.MoveTowards(currentX, desiredVel.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVel.z, maxSpeedChange);

        
        RB.MovePosition(transform.position + (desiredVel * Time.deltaTime));

       

        if (climbContactNormal != Vector3.zero)
        {
            RB.AddForce(-climbContactNormal.normalized * ((climbGripForce * 0.9f) * Time.deltaTime));
            transform.forward = -climbContactNormal;
        }
        else
        {
            RB.AddForce(transform.forward * ((climbGripForce * 0.9f) * Time.deltaTime));
        }
    }
    // check this
    void Jump(Vector3 jumpVec, float jumpHeight)
    {
        if (!enabled)
            return;

        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        Vector3 jumpDirection = (/*groundContactNormal + Vector3.up*/jumpVec).normalized;
        float alignedSpeed = Vector3.Dot(RB.velocity, jumpDirection);

        RB.velocity += jumpDirection * jumpSpeed + (Physics.gravity * Time.deltaTime);
    }

    void SetAnimations()
    {
        if (!enabled)
            return;

        switch (PlayerState)
        {
            case PlayerStates.IDLE:
                break;
            case PlayerStates.MOVING:

                Vector3 flatRBV = RB.velocity;
                flatRBV.y = 0.0f;
                flatRBV = Vector3.ClampMagnitude(flatRBV, setSpeed);
                anims.SetFloat("MovementSpeed", currentSpeed);
                break;
            case PlayerStates.JUMPING:
                break;
            case PlayerStates.FALLING:
                break;
            case PlayerStates.CLIMBING:
                break;
            case PlayerStates.GRAPPLE:
                break;
            default:
                break;
        }

    }

    #region InputMethods

    void Forward(InputState type)
    {
        switch (type)
        {
            case InputState.KEYDOWN:

                inputAxis.x = 1;
                break;
            case InputState.KEYHELD:

                inputAxis.x = 1;
                break;
            case InputState.KEYUP:

                inputAxis.x = 0;
                break;
            default:
                break;
        }
    }
    void Back(InputState type)
    {
        switch (type)
        {
            case InputState.KEYDOWN:

                inputAxis.x = -1;
                break;
            case InputState.KEYHELD:

                inputAxis.x = -1;
                break;
            case InputState.KEYUP:

                inputAxis.x = 0;
                break;
            default:
                break;
        }
    }
    void Left(InputState type)
    {
        switch (type)
        {
            case InputState.KEYDOWN:

                inputAxis.z = 1;
                break;
            case InputState.KEYHELD:

                inputAxis.z = 1;
                break;
            case InputState.KEYUP:

                inputAxis.z = 0;
                break;
            default:
                break;
        }
    }
    void Right(InputState type)
    {
        switch (type)
        {
            case InputState.KEYDOWN:

                inputAxis.z = -1;
                break;
            case InputState.KEYHELD:

                inputAxis.z = -1;
                break;
            case InputState.KEYUP:

                inputAxis.z = 0;
                break;
            default:
                break;
        }
    }
    void Jump(InputState type)
    {
        if (!enabled)
            return;

        if (PlayerState == PlayerStates.MOVING)
        {
            anims.SetTrigger("Jump");
        }

        inputAxis.y = 1;
    }
    void Run(InputState type)
    {
        switch (type)
        {
            case InputState.KEYDOWN:
                setSpeed = runSpeed;
                setAccel = maxRunAcceleration;
                break;
            case InputState.KEYUP:
                setSpeed = walkSpeed;
                setAccel = maxWalkAcceleration;
                break;
            default:
                break;
        }
    }

    #endregion InputMethods

    public bool IsGrounded()
    {
        RaycastHit rh;
        if (Physics.SphereCast(transform.position + GroundCheckStartOffset, CheckRadius,
            -transform.up, out rh,
            GroundCheckDistance, GroundLayers.value))
        {
            float upDot = Vector3.Dot(transform.up, rh.normal);
            if (upDot >= minGroundDotProduct)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsClimbing()
    {
        RaycastHit rh;
        if (Physics.SphereCast(transform.position + ClimbCheckStartOffset, CheckRadius,
            transform.forward, out rh,
            ClimbCheckDistance, ClimbLayers.value))
        {
            float degreeCheck = Vector3.Angle(transform.forward, -rh.normal);
            if (degreeCheck <= maxClimbAngle)
            {
                return true;
            }
        }
        return false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (glider.enabled)
        {
            // temp
            glider.Toggle();
        }

        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;

        EvalCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvalCollision(collision, 1);
    }

    bool onGround;
    Vector3 groundContactNormal;
    Vector3 climbContactNormal;
    /// <summary>
    /// Evaluates when this collider hits some other collider
    /// </summary>
    /// <param name="collision">The Collision struct from the OnCollide Funcs</param>
    /// <param name="collisionEvent">0 = Enter, 1 = Stay, 2 = Exit</param>
    private void EvalCollision(Collision collision, int collisionEvent = 0)
    {
        Vector3 normal = collision.GetContact(0).normal;
        float degreeCheck = Vector3.Angle(transform.forward, -normal);
        if (degreeCheck <= maxClimbAngle)
        {
            climbContactNormal = normal;
        }


        float upDot = Vector3.Dot(transform.up, normal);

        if (upDot >= minGroundDotProduct)
        {
            groundContactNormal = normal;

        }





    }


    #region Utility

    private void OnDrawGizmos()
    {
        Vector3 offsetPos = transform.position + GroundCheckStartOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(offsetPos, 0.1f);

        Gizmos.DrawLine(offsetPos, offsetPos + (Vector3.down * GroundCheckDistance));

        Gizmos.color = Color.red;
        offsetPos = transform.position + ClimbCheckStartOffset;
        Gizmos.DrawSphere(offsetPos, 0.1f);
        Gizmos.DrawLine(offsetPos, offsetPos + (transform.forward * ClimbCheckDistance));

        Gizmos.color = Color.green;
        Vector3 dir = Vector3.ProjectOnPlane(transform.forward, groundContactNormal);
        Vector3 origin = transform.position;
        origin.y -= transform.localScale.y / 2;
        Gizmos.DrawLine(origin, origin + dir);

        Gizmos.color = Color.white;
        Vector3 xAxis = Vector3.ProjectOnPlane(transform.up, climbContactNormal);
        Gizmos.DrawLine(origin, origin + xAxis);
    }

    #endregion



}
