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

    // Local Components
    Rigidbody RB;
    Animator anims;
    GliderMovement glider;
    GrappleScript grapple;
    // Character State
    public PlayerStates playerState;
    [HideInInspector] public bool haveControl;
    bool gamePaused;

    [Header("Dependencies")]
    public Transform cam;

    [Header("Forces")]
    public float walkSpeed = 2.0f;
    public float maxWalkAcceleration = 10f;
    [Space(20.0f)]
    public float runSpeed = 6.0f;
    public float maxRunAcceleration = 20f;
    public float currentSpeed;
    public float rotationSpeed = 0.1f;
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
    float minGroundDotProduct;
    Vector3 groundContactNormal;
    Vector3 climbContactNormal;
    float distanceFromGround;

    // Input Tracking
    Vector3 inputAxis = Vector3.zero;

    #region Setup
    private void Awake()
    {
        setSpeed = walkSpeed;
        setAccel = maxWalkAcceleration;
        playerState = PlayerStates.IDLE;

        RB = GetComponent<Rigidbody>();
        anims = GetComponentInChildren<Animator>();
        glider = GetComponent<GliderMovement>();
        grapple = GetComponent<GrappleScript>();
    }
    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    #endregion Setup
    #region Inputs & Callbacks
    void Start()
    {
        VirtualInputs.GetInputListener(InputType.PLAYER, "Forward").MethodToCall.AddListener(Forward);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Back").MethodToCall.AddListener(Back);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Left").MethodToCall.AddListener(Left);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Right").MethodToCall.AddListener(Right);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Run").MethodToCall.AddListener(Run);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Jump").MethodToCall.AddListener(Jump);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Glide").MethodToCall.AddListener(ToggleGlider);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Jump").MethodToCall.AddListener(CancelGrappleGlide);

        OnValidate();
        CallbackHandler.instance.pause += Pause;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.pause -= Pause;
    }
    #endregion Inputs & Callbacks

    void Pause(bool _pause)
    {
        gamePaused = _pause;
    }
    private void Update()
    {
        if (gamePaused)
            return;

        SetAnimations();

        GetDistanceToGround();

        if (!haveControl)
            return;

        HandleRotation();
    }
    void FixedUpdate()
    {
        if (gamePaused)
            return;

        SetCurrentPlayerState();

        if (!haveControl)
            return;

        HandleMovement();

        groundContactNormal = climbContactNormal = Vector3.zero;
    }

    #region Animations
    void SetAnimations()
    {
        // Animation State Checks
        anims.SetBool("Grounded", IsGrounded());
        anims.SetBool("Climbing", playerState == PlayerStates.CLIMBING);
        anims.SetBool("Grappling", playerState == PlayerStates.GRAPPLE);
        anims.SetBool("Gliding", playerState == PlayerStates.GLIDING);
        anims.SetFloat("MovementSpeed", currentSpeed / runSpeed);
        anims.speed = 1.0f;
        //anims.SetBool("Jump", !(inputAxis.y <= 0));

        // Individual state Animation checks
        switch (playerState)
        {
            case PlayerStates.IDLE:
                break;
            case PlayerStates.MOVING:
                break;
            case PlayerStates.JUMPING:
                break;
            case PlayerStates.FALLING:
                break;
            case PlayerStates.CLIMBING:
                anims.speed = inputAxis != Vector3.zero ? 1 : 0;
                break;
            case PlayerStates.GRAPPLE:
                break;
            default:
                break;
        }
    }
    #endregion Animations
    #region PlayerStateChecks
    void SetCurrentPlayerState()
    {
        if (CLIMBINGCheck())
        {
            if (playerState == PlayerStates.MOVING || playerState == PlayerStates.IDLE || playerState == PlayerStates.FALLING || playerState == PlayerStates.JUMPING)
            {
                RB.velocity = Vector3.zero;
                inputAxis.y = 0;
            }

            playerState = PlayerStates.CLIMBING;
        }

        if (MOVINGCheck())
        {
            playerState = PlayerStates.MOVING;
        }
        else if (IDLECheck())
        {
            playerState = PlayerStates.IDLE;
        }

        if (FALLINGCheck())
        {
            playerState = PlayerStates.FALLING;
        }

        if (JUMPINGCheck())
        {
            playerState = PlayerStates.JUMPING;
        }
        if (GRAPPLECheck())
        {
            playerState = PlayerStates.GRAPPLE;
        }
        if (GLIDINGCheck())
        {
            playerState = PlayerStates.GLIDING;

        }

        //Don't use gravity if grappling or gliding or climbing
        RB.useGravity = !(playerState == PlayerStates.GRAPPLE
                            || playerState == PlayerStates.GLIDING
                                || playerState == PlayerStates.CLIMBING);

    }    /// <summary>
         /// Description: Sets appropriate animation for current player state.
         /// Author: Wayd Barton-Redgrave
         /// Last Updated: 06/04/2021
         /// </summary>
    bool IDLECheck()
    {
        return (IsGrounded() && inputAxis.magnitude <= 0.0f);
    }
    bool MOVINGCheck()
    {
        return (IsGrounded() && inputAxis.magnitude > 0.0f);
    }
    bool GRAPPLECheck()
    {
        return grapple.IsConnected();
    }
    bool CLIMBINGCheck()
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
    public bool GLIDINGCheck()
    {
        return (glider != null && glider.enabled && !IsGrounded() &&
                playerState != PlayerStates.CLIMBING && playerState != PlayerStates.GRAPPLE);
    }
    #endregion
    #region Movement & Rotation
    void HandleRotation()
    {
        switch (playerState)
        {
            case PlayerStates.IDLE:
            case PlayerStates.MOVING:
                if (inputAxis.magnitude > 0.1f)
                {

                    Vector3 camForwardRelativeToPlayerRot = Vector3.Normalize(Vector3.ProjectOnPlane(cam.forward, transform.up));
                    //cachedRot = Quaternion.FromToRotation(transform.forward, camForwardRelativeToPlayerRot);
                    //if (cachedRot == Quaternion.identity)
                    //{

                    //    prev = Quaternion.identity;
                    //}

                    //if (prev == cachedRot)
                    //{
                    //    break;
                    //}
                    //prev = Quaternion.RotateTowards(prev, cachedRot, 0.1f);
                    
                    float singleStep = rotationSpeed * Time.deltaTime * TimeSlowDown.instance.timeScale;
                    //transform.forward = camForwardRelativeToPlayerRot;
                    transform.forward = Vector3.RotateTowards(transform.forward, camForwardRelativeToPlayerRot, singleStep, 0.0f);
                }
                transform.rotation = (Quaternion.LookRotation(transform.forward, Vector3.up));
                break;
            case PlayerStates.JUMPING:
                break;
            case PlayerStates.FALLING:
                RB.MoveRotation(Quaternion.LookRotation(transform.forward, Vector3.up));
                break;
            case PlayerStates.CLIMBING:
                Vector3 rotateTo = (climbContactNormal != Vector3.zero) ? (-climbContactNormal) : (transform.forward);
                transform.rotation = (Quaternion.LookRotation(rotateTo, transform.up));
                break;
            case PlayerStates.GRAPPLE:
                break;
            case PlayerStates.GLIDING:
                break;
            default:
                break;
        }
    }
    void HandleMovement()
    {
        switch (playerState)
        {
            case PlayerStates.IDLE:
            case PlayerStates.MOVING:
                MoveOnXZ(setSpeed, setAccel);
                if (inputAxis.y > 0)
                {
                    JumpFromGround(groundContactNormal + Vector3.up, groundjumpHeight);
                    /*if (IsClimbing())
                    {
                        //Jump(groundContactNormal + Vector3.up, groundjumpHeight* multi);
                        RB.MovePosition(transform.position + (groundContactNormal.normalized * multi));
                    }
                    else
                    {
                        Jump(groundContactNormal + Vector3.up, groundjumpHeight);
                    }*/
                }
                break;
            case PlayerStates.GRAPPLE:
                {
                }
                break;
            case PlayerStates.CLIMBING:
                MoveOnXY(climbSpeed, maxClimbAcceleration);
                if (inputAxis.y > 0)
                {
                    JumpFromWall();
                    //Jump(climbContactNormal + Vector3.up, wallJumpHeight);
                }
                break;
            case PlayerStates.GLIDING:
                {
                }
                break;
            case PlayerStates.JUMPING:
            case PlayerStates.FALLING:
                MoveOnXZ(inAirSpeed, maxAirAcceleration);

                break;
            default:
                break;
        }
    }
    void MoveOnXZ(float speed, float accel)
    {
        Vector3 xAxis = Vector3.ProjectOnPlane(transform.forward, groundContactNormal);
        Vector3 zAxis = Vector3.ProjectOnPlane(-transform.right, groundContactNormal);
        xAxis *= inputAxis.x;
        zAxis *= inputAxis.z;

        float xzMag = (new Vector2(inputAxis.x, inputAxis.z)).normalized.magnitude;
        currentSpeed = Mathf.MoveTowards(currentSpeed, speed * xzMag, accel * Time.deltaTime);

        inputAxis = Vector3.Normalize(inputAxis);

        Vector3 desiredVel = xAxis + zAxis;
        desiredVel.y = 0;
        desiredVel *= currentSpeed;

        RB.MovePosition(transform.position + desiredVel * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale);

        //Vector3 rotateTo = (groundContactNormal != Vector3.zero) ? (groundContactNormal) : (Vector3.up);
        
    }
    void MoveOnXY(float speed, float accel)
    {
        Vector3 xAxis = Vector3.ProjectOnPlane(transform.up, climbContactNormal);
        Vector3 zAxis = Vector3.ProjectOnPlane(-transform.right, climbContactNormal);
        xAxis *= inputAxis.x;
        zAxis *= inputAxis.z;

        inputAxis = Vector3.Normalize(inputAxis);

        float xzMag = (new Vector2(inputAxis.x, inputAxis.z)).normalized.magnitude;
        currentSpeed = Mathf.MoveTowards(currentSpeed, speed * xzMag, accel * Time.deltaTime);

        Vector3 desiredVel = xAxis + zAxis;
        desiredVel *= speed;

        RB.MovePosition(transform.position + (desiredVel * Time.deltaTime * TimeSlowDown.instance.timeScale));

        //RB.AddForce(-Physics.gravity, ForceMode.Acceleration);
        if (climbContactNormal != Vector3.zero)
        {
            RB.AddForce(-climbContactNormal.normalized * ((climbGripForce * 0.9f) ));
            //transform.forward = -climbContactNormal;
        }
        else
        {
            RB.AddForce(transform.forward * ((climbGripForce * 0.9f)));
        }
    }  
    // Currently bugged with slowtime due to use of impulse
    void JumpFromGround(Vector3 jumpVec, float jumpHeight)
    {
        if (!haveControl || gamePaused)
            return;

        anims.ResetTrigger("Jump");
        anims.SetTrigger("Jump");
        inputAxis.y = 0;

        RB.AddForce(transform.up * 15.0f, ForceMode.Impulse);

        /*float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        Vector3 jumpDirection = jumpVec.normalized;

        RB.velocity += jumpDirection * jumpSpeed + (Physics.gravity * Time.deltaTime);*/
    }

    /// <summary>
    /// Description: Jumps from the wall and rotates player.
    /// Author: Wayd Barton-Redgrave
    /// Last Updated: 06/04/2021
    /// </summary>
    void JumpFromWall()
    {
        anims.ResetTrigger("Jump");
        anims.SetTrigger("Jump");

        RB.AddForce((-transform.forward + transform.up) * 12.0f, ForceMode.Impulse);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 180.0f, transform.rotation.eulerAngles.z);
        inputAxis.y = 0;
    }
    #endregion Movement & Rotation
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
        if (!haveControl)
            return;

        switch (type)
        {
            case InputState.KEYDOWN:
                inputAxis.y = 1;
                break;
            case InputState.KEYUP:
                inputAxis.y = 0;
                break;
            default:
                break;
        }
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
    /// <summary>
    /// <br>Description: Cancels grappling or gliding with press of jump key.</br>
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 06/04/2021</br>
    /// </summary>
    void CancelGrappleGlide(InputState type)
    {
        switch (type)
        {
            case InputState.KEYDOWN:
                if (GRAPPLECheck())
                    grapple.FireHook();
                if (glider.enabled)
                    glider.Toggle();
                break;
            case InputState.KEYHELD:
                break;
            case InputState.KEYUP:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Description: Toggles Glider on and off.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 06/04/2021</br>
    /// </summary>
    void ToggleGlider(InputState type)
    {
        switch (type)
        {
            case InputState.KEYDOWN:
                if ((!(IsGrounded() || GRAPPLECheck() || grapple.hook.enabled) || glider.enabled) && distanceFromGround > 3.0f)
                    glider.Toggle();
                break;
            case InputState.KEYHELD:
                break;
            case InputState.KEYUP:
                break;
            default:
                break;
        }
    }
    #endregion InputMethods
    #region Utility
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
    /// <summary>
    /// <br>Description: Gets distance to ground - used to check if glider can be enabled.</br>
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 06/04/2021</br>
    /// </summary>
    void GetDistanceToGround()
    {
        RaycastHit rh;
        if (Physics.SphereCast(transform.position, CheckRadius, -transform.up, out rh, 1000.0f, GroundLayers.value))
        {
            distanceFromGround = Vector3.Distance(rh.point, transform.position);
            return;
        }

        distanceFromGround = Mathf.Infinity;
    }
    #endregion Utility
    #region Collisions
    /// <summary>
    /// Description: Checks for collisions and acts based on state
    /// Author: Wayd Barton-Redgrave
    /// Last Updated: 06/04/2021
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // Turn off glider upon hitting an object
        if (glider.enabled)      
            glider.Toggle();     

        // Change Collisions Back (so player can now collider with whale) - This isn't perfect, requires the player touches something and doesn't try to reconnect to whale after jumping off.
        if (!collision.gameObject.GetComponent<WhaleMovement>() && this.gameObject.layer == LayerMask.NameToLayer("PlayerFromWhale"))
        {
            this.gameObject.layer = LayerMask.NameToLayer("Player");
        }

        EvalCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        EvalCollision(collision, 1);
    }
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

        float upDot = Vector3.Dot(Vector3.up, normal);

        if (upDot >= minGroundDotProduct)
        {
            groundContactNormal = normal;
        }
    }
    #endregion Collisions


    #region Debug

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 groundStartPos = transform.position + GroundCheckStartOffset;
        Vector3 groundEndPos = groundStartPos + ((-Vector3.up) * GroundCheckDistance);
        Gizmos.DrawLine(groundStartPos, groundEndPos);
        Gizmos.DrawSphere(groundEndPos, CheckRadius);

        Gizmos.color = Color.blue;
        Vector3 climbStartPos = transform.position + ClimbCheckStartOffset;
        Vector3 climbEndPos = climbStartPos + ((transform.forward) * ClimbCheckDistance);
        Gizmos.DrawLine(climbStartPos, climbEndPos);
        Gizmos.DrawSphere(climbEndPos, CheckRadius);
    }
    #endregion
}
