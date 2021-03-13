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
        GRAPPLE
    }

    Rigidbody RB;
    Animator anims;
    GrappleHook gHook;

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

    public float jumpHeight = 5.0f;
    [Space(20.0f)]
    public float stickForce = 1.0f;

    float maxGroundAngle = 40f;

    [Header("GroundChecks")]
    public LayerMask GroundLayers;
    public float GroundCheckDistance = 1.0f;
    public float GroundCheckRadius = 0.1f;
    public Vector3 GroundCheckStartOffset = Vector3.zero;

    [Header("Misc")]
    public bool LocalToGround = true;



    void Start()
    {
        setSpeed = walkSpeed;
        setAccel = maxWalkAcceleration;

        PlayerState = PlayerStates.IDLE;

        RB = GetComponent<Rigidbody>();
        anims = GetComponentInChildren<Animator>();
        gHook = GetComponent<GrappleHook>();
        VirtualInputs.GetInputListener(InputType.PLAYER, "Forward").MethodToCall.AddListener(Forward);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Back").MethodToCall.AddListener(Back);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Left").MethodToCall.AddListener(Left);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Right").MethodToCall.AddListener(Right);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Run").MethodToCall.AddListener(Run);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Jump").MethodToCall.AddListener(Jump);

        OnValidate();
    }

    private void Update()
    {
        SetAnimations();
    }

    public Transform cam;

    public Vector3 inputAxis = Vector3.zero;
    void FixedUpdate()
    {
        Vector3 desired = new Vector3(transform.eulerAngles.x, cam.eulerAngles.y, transform.eulerAngles.z);

        RB.MoveRotation(transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(desired), 20 * Time.fixedDeltaTime));

        SetCurrentPlayerState();
        HandleMovement();

        if (collidedObj != null) //If attached to something
        {
            collidedprevPos = collidedObj.position;
        }

        if (inputAxis != Vector3.zero)
        {
            inputAxis = Vector3.zero;
        }
    }

    float minGroundDotProduct;
    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
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
        if (CLIMBINGCheck()) //Currently disabled
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
        return RB.velocity.magnitude > 0.1f;
    }
     
    bool GRAPPLECheck()
    {
        //Only check if available, currently hooked, and on the ground to allow grapple movement
        return (gHook != null && gHook.enabled && gHook.GrappleActive && !IsGrounded());
    }
    bool CLIMBINGCheck()//ToDo Later when implementing climbing
    {
        return false;
    }
    bool JUMPINGCheck()
    {
        float currenty = Vector3.Dot(RB.velocity, transform.up);
        return currenty > 0.0f && !IsGrounded();
    }
    bool FALLINGCheck()
    {
        float currenty = Vector3.Dot(RB.velocity, transform.up);
        return currenty < 0.0f && !IsGrounded();
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
                    Jump();
                }
                break;
            case PlayerStates.GRAPPLE:
                {
                    gHook.ApplyForces(inputAxis);
                }
                break;
            case PlayerStates.CLIMBING:
            case PlayerStates.JUMPING:
                break;
            case PlayerStates.FALLING:
                if (LocalToGround)
                {
                    collidedObj = null;
                    
                }
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

        float currentX = Vector3.Dot(RB.velocity, xAxis);
        float currentZ = Vector3.Dot(RB.velocity, zAxis);

        float acceleration = accel;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;

        Vector3 desiredVel = inputAxis;
        desiredVel.y = 0;
        desiredVel *= speed;

        

        float newX =
            Mathf.MoveTowards(currentX, desiredVel.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVel.z, maxSpeedChange);


        RB.velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        /*if (collidedObj != null)
        {
            Vector3 offset = GetCollidedFrameOffset();
            RB.MovePosition(transform.position + offset);
        }*/
    }

    void Jump()
    {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        Vector3 jumpDirection = (groundContactNormal + Vector3.up).normalized;
        float alignedSpeed = Vector3.Dot(RB.velocity, jumpDirection);

        RB.velocity += jumpDirection * jumpSpeed;
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
        if (Physics.SphereCast(transform.position + GroundCheckStartOffset, GroundCheckRadius,
            Vector3.down, out rh,
            GroundCheckDistance, GroundLayers.value))
        {
            if (rh.normal.y >= minGroundDotProduct)
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

    /// <summary>
    /// Evaluates when this collider hits some other collider
    /// </summary>
    /// <param name="collision">The Collision struct from the OnCollide Funcs</param>
    /// <param name="collisionEvent">0 = Enter, 1 = Stay, 2 = Exit</param>
    private void EvalCollision(Collision collision, int collisionEvent = 0)
    {
        Vector3 normal = collision.GetContact(0).normal;
        if (normal.y >= minGroundDotProduct)
        {
            groundContactNormal = normal;
            if (LocalToGround)
            {
                collidedObj = collision.transform;
                if (collisionEvent == 0) //OnEnter
                {
                    collidedprevPos = collision.transform.position;
                }
            }
            
        }
    }

    Vector3 collidedprevPos = Vector3.zero;
    Transform collidedObj;
    Vector3 GetCollidedFrameOffset() //The movement vector since the last frame;
    {
        return (collidedObj == null) ? (Vector3.zero) :
            (collidedObj.position - collidedprevPos);
    }

    #region Utility

    private void OnDrawGizmos()
    {
        Vector3 offsetPos = transform.position + GroundCheckStartOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(offsetPos, 0.1f);

        Gizmos.DrawLine(offsetPos, offsetPos + (Vector3.down * GroundCheckDistance));

        Vector3 dir = Vector3.ProjectOnPlane(transform.forward, groundContactNormal);
        Vector3 origin = transform.position;
        origin.y -= transform.localScale.y / 2;
        Gizmos.DrawLine(origin, origin + dir);
    }

    #endregion



}
