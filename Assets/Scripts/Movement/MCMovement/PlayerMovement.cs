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


    VirtualInputs vInputs;
    Rigidbody RB;
    Animator anims;
    GrappleHook gHook;
    Glider glider;
    [Tooltip("This can't be set, and sets to IDLE on Start call, so no touchy")]
    public PlayerStates PlayerState;

    [Header("Forces")]
    public float walkSpeed = 2.0f;
    public float maxWalkAcceleration = 10f;
    [Space(20.0f)]
    
    public float runSpeed = 6.0f;
    public float maxRunAcceleration = 20f;

    [Space(20.0f)]
    private float setSpeed;
    private float setAccel;

    [Space(20.0f)]
    public float inAirSpeed = 1.0f;
    public float maxAirAcceleration = 10.0f;
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

    void Start()
    {
        setSpeed = walkSpeed;
        setAccel = maxWalkAcceleration;

        PlayerState = PlayerStates.IDLE;

        RB = GetComponent<Rigidbody>();
        anims = GetComponentInChildren<Animator>();
        gHook = GetComponent<GrappleHook>();
        glider = GetComponent<Glider>();

        vInputs = GetComponent<VirtualInputs>();
        vInputs.GetInputListener("Forward").MethodToCall.AddListener(Forward);
        vInputs.GetInputListener("Back").MethodToCall.AddListener(Back);
        vInputs.GetInputListener("Left").MethodToCall.AddListener(Left);
        vInputs.GetInputListener("Right").MethodToCall.AddListener(Right);
        vInputs.GetInputListener("Run").MethodToCall.AddListener(Run);
        vInputs.GetInputListener("Jump").MethodToCall.AddListener(Jump);

        RB.useGravity = false;
        OnValidate();
    }

    private void Update()
    {
        
        SetAnimations();
    }


    Vector3 inputAxis = Vector3.zero;
    void FixedUpdate()
    {
        SetCurrentPlayerState();
        HandleMovement();

        groundcollidedObject = null;
        climbcollidedObject = null;
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
        if (IDLECheck())
        {
            PlayerState = PlayerStates.IDLE;
        }

        if (MOVINGCheck())
        {
            PlayerState = PlayerStates.MOVING;
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
        return RB.velocity.magnitude <= 0.1f;
    }

    bool MOVINGCheck()
    {
        return RB.velocity.magnitude > 0;
    }
     
    bool GRAPPLECheck()
    {
        //Only check if available, currently hooked, and on the ground to allow grapple movement
        return (gHook != null && gHook.enabled && gHook.GrappleActive && !IsGrounded());
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
        return (glider != null && glider.enabled && glider.GliderActive && !IsGrounded() &&
                PlayerState !=PlayerStates.CLIMBING && PlayerState != PlayerStates.GRAPPLE);
    }
    #endregion


    void HandleMovement()
    {
        switch (PlayerState)
        {
            case PlayerStates.IDLE:
            case PlayerStates.MOVING:
                MoveOnXZ(setSpeed, setAccel);
                if (inputAxis.y > 0)
                {
                    Jump(groundContactNormal + Vector3.up, groundjumpHeight);
                }
                break;
            case PlayerStates.GRAPPLE:
                {
                    gHook.ApplyForces(inputAxis);
                }
                break;
            case PlayerStates.CLIMBING:
                MoveOnXY(inAirSpeed, maxAirAcceleration);
                if (inputAxis.y > 0)
                {
                    Jump(climbContactNormal +Vector3.up, wallJumpHeight);
                }
                break;
            case PlayerStates.GLIDING:
                {
                    glider.ApplyForces(inputAxis);
                }
                break;
            case PlayerStates.JUMPING:
            case PlayerStates.FALLING:
                MoveOnXZ(inAirSpeed, maxAirAcceleration);
                break;
            default:
                break;
        }
        inputAxis = Vector3.zero;
    }

    void MoveOnXZ(float speed, float accel)
    {
        Vector3 xAxis = Vector3.ProjectOnPlane(transform.forward, groundContactNormal);
        Vector3 zAxis = Vector3.ProjectOnPlane(-transform.right, groundContactNormal);

        float currentX = Vector3.Dot(RB.velocity, xAxis);
        float currentZ = Vector3.Dot(RB.velocity, zAxis);

        float acceleration = accel;
        float maxSpeedChange = acceleration * Time.deltaTime;

        Vector3 desiredVel = inputAxis;
        desiredVel.y = 0;
        desiredVel *= speed;

        
        float newX =
            Mathf.MoveTowards(currentX, desiredVel.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVel.z, maxSpeedChange);

        Vector3 change = (xAxis * (newX - currentX) + zAxis * (newZ - currentZ)
            + (Physics.gravity * Time.deltaTime));
        RB.velocity += change;
    }

    void MoveOnXY(float speed, float accel)
    {
        
        
    }
    void Jump(Vector3 jumpVec, float jumpHeight)
    {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        Vector3 jumpDirection = (/*groundContactNormal + Vector3.up*/jumpVec).normalized;
        float alignedSpeed = Vector3.Dot(RB.velocity, jumpDirection);

        RB.velocity += jumpDirection * jumpSpeed + (Physics.gravity * Time.deltaTime);
    }

    void SetAnimations()
    {
        switch (PlayerState)
        {
            case PlayerStates.IDLE:
                break;
            case PlayerStates.MOVING:

                Vector3 flatRBV = RB.velocity;
                flatRBV.y = 0.0f;
                flatRBV = Vector3.ClampMagnitude(flatRBV, setSpeed);
                anims.SetFloat("MovementSpeed", flatRBV.magnitude / runSpeed);
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
        inputAxis.x = 1;
    }
    void Back(InputState type)
    {
        inputAxis.x = -1;

    }
    void Left(InputState type)
    {
        inputAxis.z = 1;

    }
    void Right(InputState type)
    {
        inputAxis.z = -1;
    }
    void Jump(InputState type)
    {
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
            Vector3.down, out rh,
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
    }

    #endregion



}
