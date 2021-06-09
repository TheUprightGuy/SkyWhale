using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

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
    //[HideInInspector] 
    public bool haveControl;
    //[HideInInspector] 
    public bool gamePaused;

    [Header("Dependencies")]
    public Transform cam;

    [Header("Forces")]
    public float walkSpeed = 2.0f;
    public float maxWalkAcceleration = 10f;
    [Space(20.0f)]
    public float runSpeed = 6.0f;
    public float maxRunAcceleration = 20f;
    private float currentVelMag;
    public float rotationSpeed = 0.1f;
    public float climbrotationSpeed = 100.0f;
    public float uprightrotationSpeed = 200.0f;
    [Space(20.0f)]
    private float setSpeed;
    public float setAccel;
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

    public float GroundToClimbAngle = 50.0f;
    public float ForwardToClimbAngle = 50.0f;
    [Header("Check Settings")]

    public LayerMask GroundLayers;
    public float GroundCheckDistance = 1.0f;
    public float GroundCheckRadius = 0.1f;
    public Vector3 GroundCheckStartOffset = Vector3.zero;

    [Space(20.0f)]
    public LayerMask ClimbLayers;
    public float ClimbCheckDistance = 1.0f;
    public float ClimbCheckRadius = 0.1f;
    public Vector3 ClimbCheckStartOffset = Vector3.zero;

    float minGroundDotProduct;
    Vector3 groundContactNormal;
    Vector3 climbContactNormal;
    Vector3 wallContactnormal;
    float distanceFromGround;

    // Input Tracking
    Vector3 inputAxis = Vector3.zero;

    #region Setup
    private void Awake()
    {
        setSpeed = runSpeed;
        setAccel = maxRunAcceleration;
        playerState = PlayerStates.IDLE;

        RB = GetComponent<Rigidbody>();
        anims = GetComponentInChildren<Animator>();
        glider = GetComponent<GliderMovement>();
        grapple = GetComponent<GrappleScript>();
    }
    private void OnValidate()
    {
        //minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnValidate();
        CallbackHandler.instance.pause += Pause;
        CallbackHandler.instance.cinematicPause += CinematicPause;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.pause -= Pause;
        CallbackHandler.instance.cinematicPause -= CinematicPause;
    }
    #endregion Inputs & Callbacks

    void Pause(bool _pause)
    {
        gamePaused = _pause;
        // this is bad but hey it works
        if (cinematicPause)
        {
            CameraManager.instance.LetterBox(true);
        }
    }

    public bool cinematicPause;
    void CinematicPause(bool _pause)
    {
        cinematicPause = _pause;
    }

    private float basePlayerHeight = -27f;
    private float maxHeightDifference = 74f;
    
    private void Update()
    {
        AmbientLayer windLayer = AudioManager.instance.ambientLayers[1].GetComponent<AmbientLayer>();
        windLayer.soundInfo.audioSource.pitch = windLayer.soundInfo.pitchDefault * Mathf.Lerp(1f, 3f, Mathf.Clamp01(Mathf.Abs(basePlayerHeight - transform.position.y)/maxHeightDifference));
        if (gamePaused || cinematicPause)
            return;

        SetAnimations();

        GetDistanceToGround();

        if (!haveControl)
            return;

        //HandleRotation();

        PromptCheck();
    }
    void FixedUpdate()
    {
        if (gamePaused || cinematicPause)
            return;

        SetCurrentPlayerState();

        if (!haveControl)
            return;

        HandleMovement();
        HandleRotation();
        groundContactNormal = climbContactNormal  = wallContactnormal =  calcClimbNormal = Vector3.zero;
    }

    #region Animations

    /// <summary>
    /// Description: Sets appropriate animation for current player state.
    /// <b>Author: Wayd Barton-Redgrave</b>
    /// <b>Last Updated: 06/04/2021</b>
    /// </summary>
    void SetAnimations()
    {
        // Animation State Checks
        anims.SetBool("Grounded", IsGrounded());
        anims.SetBool("Climbing", playerState == PlayerStates.CLIMBING);
        anims.SetBool("Grappling", playerState == PlayerStates.GRAPPLE);
        anims.SetBool("Gliding", playerState == PlayerStates.GLIDING);
        anims.SetFloat("MovementSpeed", currentVelMag / runSpeed);
        //anims.SetBool("Jump", !(inputAxis.y <= 0));

        anims.speed = 1;
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

    /// <summary>
    /// Description: Changes the current player state based on the functions below this
    /// <br>Author: Jack Belton</br>
    /// <br>Last Updated: 6/04/21</br>
    /// </summary>
    void SetCurrentPlayerState()
    {
        if (CLIMBINGCheck())
        {
            if (playerState == PlayerStates.MOVING || playerState == PlayerStates.IDLE || playerState == PlayerStates.FALLING || playerState == PlayerStates.JUMPING)
            {
                RB.velocity = Vector3.zero;
                inputAxis.y = 0;
                AudioManager.instance.PlaySound("WallHit");
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

        Camera.main.GetComponent<ThirdPersonCamera>().climbing = (playerState == PlayerStates.CLIMBING);

    }    
    bool IDLECheck()
    {
        return (/*IsGrounded()*/groundContactNormal.magnitude > 0 && inputAxis.magnitude <= 0.0f);
    }
    bool MOVINGCheck()
    {
        return (/*IsGrounded()*/groundContactNormal.magnitude > 0 && inputAxis.magnitude > 0.0f);
    }
    bool GRAPPLECheck()
    {
        return grapple.IsConnected();
    }
    bool CLIMBINGCheck()
    {
        //return (!IsGrounded() && IsClimbing());
        return (groundContactNormal == Vector3.zero && climbContactNormal.magnitude > 0.0f);
    }
    bool JUMPINGCheck()
    {
        float currenty = Vector3.Dot(RB.velocity, transform.up);
        return currenty > 0.0f && groundContactNormal == Vector3.zero && !CLIMBINGCheck();
    }
    bool FALLINGCheck()
    {
        float currenty = Vector3.Dot(RB.velocity, transform.up);
        return currenty < 0.0f && groundContactNormal == Vector3.zero && !CLIMBINGCheck();
    }
    public bool GLIDINGCheck()
    {
        return (glider != null && glider.enabled && !IsGrounded() &&
                playerState != PlayerStates.CLIMBING && playerState != PlayerStates.GRAPPLE);
    }
    #endregion

    #region Movement & Rotation

    /// <summary>
    /// Description: Handles rotation of the player based on state
    /// <br>Author: Jack Belton</br>
    /// <br>Last Updated: 7/04/21</br>
    /// </summary>
    void HandleRotation()
    {
        switch (playerState)
        {
            case PlayerStates.IDLE:
            case PlayerStates.MOVING:

                if (inputAxis.magnitude > 0.1f)
                {
                    RotateTowardInput();
                }

                float deg = uprightrotationSpeed * Time.deltaTime;
                //Setting the up position without changing the forward
                Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
                Quaternion to = Quaternion.LookRotation(projectedForward, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, to, deg);
                //transform.rotation = (to);
                break;
            case PlayerStates.JUMPING:
            case PlayerStates.FALLING:
                if (inputAxis.magnitude > 0.1f)
                {
                    RotateTowardInput();
                }
                Vector3 projectedOtherForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
                RB.MoveRotation(Quaternion.LookRotation(projectedOtherForward, Vector3.up));
                break;
            case PlayerStates.CLIMBING:
                Vector3 rotateTo = (climbContactNormal != Vector3.zero) ? (-climbContactNormal) : (transform.forward);
                float climbdeg = climbrotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rotateTo, Vector3.up), climbdeg);
                //transform.rotation = (Quaternion.LookRotation(rotateTo, Vector3.up));
                break;
            case PlayerStates.GRAPPLE:
                break;
            case PlayerStates.GLIDING:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Description: Rotates player towards direction of input
    /// <br>Author: Jack Belton</br>
    /// <br>Last Updated: 7/04/21</br>
    /// </summary>
    void RotateTowardInput()
    {
        float singleStep = rotationSpeed * Time.deltaTime;// * TimeSlowDown.instance.timeScale;

        //Forward Vector relative to the camera direction
        Vector3 camForwardRelativeToPlayerRot = Vector3.Normalize(Vector3.ProjectOnPlane(cam.forward, transform.up));
        //Vector perpendicular to the above
        Vector3 left = Vector3.Cross(camForwardRelativeToPlayerRot, Vector3.up).normalized;

        //Direction currently represented by the keys pressed relative to the camera forward direction
        Vector3 heading = ((camForwardRelativeToPlayerRot * inputAxis.z) + (left * inputAxis.x)).normalized;
        //Debug.DrawLine(transform.position, transform.position + heading, Color.blue);

        //Rotate from the current forward towards the given heading over singlestep time
        transform.forward = Vector3.RotateTowards(transform.forward, heading, singleStep, 0.0f);
    }

    /// <summary>
    /// Description: Handles movement of the player based on state
    /// <br>Author: Jack Belton</br>
    /// <br>Last Updated: 7/04/21</br>
    /// </summary>
    void HandleMovement()
    {
        switch (playerState)
        {
            case PlayerStates.IDLE:
            case PlayerStates.MOVING:
                if (inputAxis.y > 0)
                {
                    JumpFromGround(groundContactNormal + Vector3.up, groundjumpHeight);
                }

                GroundMovement(setSpeed, setAccel);
                break;
            case PlayerStates.GRAPPLE:
                {
                }
                break;
            case PlayerStates.CLIMBING:
                ClimbMovement(climbSpeed, maxClimbAcceleration);
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
                {
                    GroundMovement(inAirSpeed, maxAirAcceleration);
                }
                break;
            default:
                break;
        }

        Physics.gravity = RB.velocity.y >= -0.8f ? Vector3.up * -20.0f : Vector3.up * -20.0f * Mathf.Clamp((TimeSlowDown.instance.drainTimer / TimeSlowDown.instance.slowDuration), 0.5f, 1.0f);
        debugGrav = Physics.gravity.y;
    }
    public float debugGrav;

    Vector3 currentVel = Vector3.zero;

    /// <summary>
    /// <br>Description: Moves the position of the player using MovePosition based on speed and accel</br>
    /// <br>Author: Jack Belton</br>
    /// <br>Last Updated: 7/04/21</br>
    /// </summary>
    /// <param name="speed">The desired speed to accelerate towards</param>
    /// <param name="accel">The rate to move towards speed</param>
    void GroundMovement(float speed, float accel)
    {
        //Forward Vector relative to the camera direction
        Vector3 camForwardRelativeToPlayerRot = Vector3.Normalize(Vector3.ProjectOnPlane(cam.forward, transform.up));
        //Vector perpendicular to the above
        Vector3 left = Vector3.Cross(camForwardRelativeToPlayerRot, Vector3.up).normalized;

        //Direction currently represented by the keys pressed relative to the camera forward direction
        Vector3 heading = ((camForwardRelativeToPlayerRot * inputAxis.z) + (left * inputAxis.x)).normalized;


        Vector3 projectedForward = Vector3.ProjectOnPlane(heading, groundContactNormal);
        float xzMag = new Vector2(inputAxis.x, inputAxis.z).magnitude;//Key is pressed
        projectedForward *= xzMag; 

        Vector3 desiredVel = Vector3.zero;
        desiredVel = (projectedForward).normalized; //Input direction
        desiredVel *= speed; //Amount to move in said direction

        
        //Vector to actually move by
        Vector3 actualVel = currentVel =  Vector3.MoveTowards(currentVel, //Current moving velocity
                                                                desiredVel,
                                                                    accel * Time.fixedDeltaTime ); //Amount to change by

        //Against a wall in the front dir
        if (wallContactnormal != Vector3.zero)
        {
            float currentForwardVel = Vector3.Dot(currentVel, wallContactnormal);
            Vector3 negateforce = currentForwardVel * wallContactnormal;
            currentVel -= negateforce;
        }

        //animation walk speeds
        currentVelMag = currentVel.magnitude;

        RB.MovePosition(transform.position + currentVel);// * TimeSlowDown.instance.timeScale);

    }

    /// <summary>
    /// <br>Description: Moves the position of the player based on inputaxis, projected on the right and up axis</br>
    /// <br>Author: Jack Belton</br>
    /// <br>Last Updated: 6/04/20</br>
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="accel"></param>
    void ClimbMovement(float speed, float accel)
    {
        Vector3 xAxis = Vector3.ProjectOnPlane(-transform.right, climbContactNormal);
        Vector3 zAxis = Vector3.ProjectOnPlane(transform.up, climbContactNormal);
        xAxis *= inputAxis.x;
        zAxis *= inputAxis.z;

        Vector3 desiredVel = Vector3.zero;
        desiredVel = (xAxis + zAxis).normalized; //Input direction
        desiredVel *= speed; //Amount to move in said direction

        Vector3 actualVel = currentVel = Vector3.MoveTowards(currentVel, //Current moving velocity
                                                               desiredVel,
                                                                   accel * Time.fixedDeltaTime);// * TimeSlowDown.instance.timeScale); //Amount to change by

        RB.MovePosition(transform.position + actualVel);
        currentVelMag = currentVel.magnitude;

        if (climbContactNormal != Vector3.zero)
        {
            RB.AddForce(-climbContactNormal.normalized * ((climbGripForce * 0.9f) ));
        }
        else
        {
            RB.AddForce(transform.forward * ((climbGripForce * 0.9f)));
        }
    }

    // Currently bugged with slowtime due to use of impulse
    /// <summary>
    /// <br>Description: Apply an impulse force upwards</br>
    /// <br>Author: Jack Belton, Wayd Barton-Redgrave</br>
    /// <br>Last Updated: </br>
    /// </summary>
    /// <param name="jumpVec"></param>
    /// <param name="jumpHeight"></param>
    void JumpFromGround(Vector3 jumpVec, float jumpHeight)
    {
        if (!haveControl || gamePaused)
            return;

        anims.ResetTrigger("Jump");
        anims.SetTrigger("Jump");
        inputAxis.y = 0;

        //RB.AddForce(transform.up * 15.0f, ForceMode.Impulse);
        AudioManager.instance.PlaySound("Jump");
        
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        Vector3 jumpDirection = jumpVec.normalized;

        RB.velocity += (jumpDirection * jumpSpeed + (Physics.gravity * Time.fixedDeltaTime));
    }

    /// <summary>
    /// <br>Description: Jumps from the wall and rotates player.</br>
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 06/04/2021</br>
    /// </summary>
    void JumpFromWall()
    {
        anims.ResetTrigger("Jump");
        anims.SetTrigger("Jump");

        //RB.AddForce((-transform.forward + transform.up) * 12.0f, ForceMode.Impulse);
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 180.0f, transform.rotation.eulerAngles.z);

        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * wallJumpHeight);
        Vector3 jumpDirection = (-transform.forward + transform.up).normalized;

        RB.velocity += jumpDirection * jumpSpeed + (Physics.gravity * Time.deltaTime);

        inputAxis.y = 0;
        AudioManager.instance.PlaySound("Jump");
    }
    #endregion Movement & Rotation

    /// <summary>
    /// <br>Description: All of the functions within this region are used by the Virtual Inputs class to handle player inputs</br>
    ///<br>Author: Jack Belton</br>
    ///<br>Last Updated: void Jump(InputState type): 7/04/20 </br>    
    /// </summary>
    #region InputMethods
    void Forward(InputState type)
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
    void Back(InputState type)
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
    void Left(InputState type)
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
    void Right(InputState type)
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
                setSpeed = walkSpeed;
                setAccel = maxWalkAcceleration;
                break;
            case InputState.KEYUP:
                setSpeed = runSpeed;
                setAccel = maxRunAcceleration;
                break;
            default:
                break;
        }
        //setSpeed =  (setSpeed != runSpeed) ? (runSpeed) : (walkSpeed);
        //setAccel = (setAccel != maxRunAcceleration) ? (maxRunAcceleration) : (maxWalkAcceleration);
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
                {
                    grapple.FireHook();
                    grapple.YeetPlayer();
                }
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
    //public bool IsGrounded()
    //{
    //    RaycastHit rh;
    //    if (Physics.SphereCast(transform.position + GroundCheckStartOffset, GroundCheckRadius,
    //        -transform.up, out rh,
    //        GroundCheckDistance, GroundLayers.value))
    //    {
    //        float upDot = Vector3.Dot(transform.up, rh.normal);
    //        if (upDot >= minGroundDotProduct)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    /// <summary>
    /// Description: Checks for a ground below the player, uses a spherecast and double checks with groundcontactnormal
    /// <br>Author: Jack Belton</br>
    /// <br>Last Updated: 7/04/21</br>
    /// </summary>
    public bool IsGrounded()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + GroundCheckStartOffset, GroundCheckRadius,
            -transform.up,
            GroundCheckDistance, GroundLayers.value);


        //If this returns no hits and collisions returns no hits
        if (hits.Length < 0 || groundContactNormal == Vector3.zero)
        { 
            return false;
        }

        
        Vector3 totaledNormal = Vector3.zero;
        foreach (RaycastHit item in hits)
        {
            totaledNormal += item.normal;
        }
        totaledNormal = totaledNormal.normalized;

        //Debug.DrawLine(transform.position, transform.position + totaledNormal);
        //Debug.DrawLine(transform.position, transform.position + groundContactNormal, Color.red);
        float upDot = Vector3.Dot(transform.up, totaledNormal);
        return upDot >= minGroundDotProduct;
    }
    public bool IsClimbing()
    {
        RaycastHit rh;
        if (Physics.SphereCast(transform.position + ClimbCheckStartOffset, ClimbCheckRadius,
            transform.forward, out rh,
            ClimbCheckDistance, ClimbLayers.value))
        {
            float degreeCheck = Vector3.Angle(transform.forward, -rh.normal);
            if (climbContactNormal !=Vector3.zero)
            {
                return true;
            }
        }
        return false;
    }

    //This doesn't work ¯\_(ツ)_/¯
    //public bool IsClimbing()
    //{
    //    RaycastHit[] hits = Physics.SphereCastAll(transform.position + ClimbCheckStartOffset, ClimbCheckRadius,
    //            transform.forward,
    //            ClimbCheckDistance, ClimbLayers.value);

    //    if (hits.Length < 0) { return false; }

    //    Vector3 totaledNormal = Vector3.zero;
    //    foreach (RaycastHit item in hits)
    //    {
    //        totaledNormal += item.normal;
    //    }
    //    totaledNormal = totaledNormal.normalized;

    //    float degreeCheck = Vector3.Angle(transform.forward, totaledNormal);
    //    return degreeCheck <= maxClimbAngle;
    //}

    /// <summary>
    /// <br>Description: Gets distance to ground - used to check if glider can be enabled.</br>
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 06/04/2021</br>
    /// </summary>
    void GetDistanceToGround()
    {
        RaycastHit rh;
        if (Physics.SphereCast(transform.position, GroundCheckRadius, -transform.up, out rh, 1000.0f, GroundLayers.value))
        {
            distanceFromGround = Vector3.Distance(rh.point, transform.position);
            return;
        }

        distanceFromGround = Mathf.Infinity;
    }

    void PromptCheck()
    {
        // temp testing
        if (distanceFromGround > 3.0f && GetComponent<GliderMovement>().unlocked && playerState == PlayerStates.FALLING)
        {
            //CallbackHandler.instance.DisplayHotkey(InputType.PLAYER, "Glide", "");
            //CallbackHandler.instance.ShowGlide();
            CallbackHandler.instance.DisplayPrompt(PromptType.Glide);
        }
        else
        {
            //CallbackHandler.instance.HideHotkey("Glide");
            //CallbackHandler.instance.HideGlide();
            CallbackHandler.instance.HidePrompt(PromptType.Glide);
        }

        if (IsClimbing() && IsGrounded())
        {
            //CallbackHandler.instance.DisplayHotkey(InputType.PLAYER, "Jump", "Climb");
            //CallbackHandler.instance.ShowJump();
            CallbackHandler.instance.DisplayPrompt(PromptType.Climb);
        }
        else
        {
            //CallbackHandler.instance.HideHotkey("Jump");
            //CallbackHandler.instance.HideJump();
            CallbackHandler.instance.HidePrompt(PromptType.Climb);
        }
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
        {
            glider.Toggle();
            AudioManager.instance.PlaySound("Crash");
        }

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

    private void OnCollisionExit(Collision collision)
    {

    }


    Vector3 calcClimbNormal = Vector3.zero;
    Vector3 calcGroundNormal = Vector3.zero;

    /// <summary>
    /// Evaluates when this collider hits some other collider
    /// </summary>
    /// <param name="collision">The Collision struct from the OnCollide Funcs</param>
    /// <param name="collisionEvent">0 = Enter, 1 = Stay, 2 = Exit</param>
    /// 
    Dictionary<GameObject, Vector3> NormalMap = new Dictionary<GameObject, Vector3>();
    private void EvalCollision(Collision collision, int collisionEvent = 0)
    {

        Vector3 normal = collision.GetContact(0).normal;
        //Vector3 projectedNormal = Vector3.ProjectOnPlane(normal, transform.right);
        //projectedNormal = new Vector3(Mathf.Abs(projectedNormal.x), Mathf.Abs(projectedNormal.y), Mathf.Abs(projectedNormal.z));
        Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        float degreeDownCheck = Vector3.Angle(Vector3.down, -normal);
        float degreeForwardCheck = Vector3.Angle(projectedForward, -normal);

        bool GtC = degreeDownCheck > GroundToClimbAngle;
        bool FtC = degreeForwardCheck < ForwardToClimbAngle;
        if (GtC) //Climbing
        {
            Debug.DrawLine(transform.position, transform.position + (1 * -normal), Color.blue);

            

            if (FtC) //In front of player
            {
                wallContactnormal += normal; //theres a wall here
                wallContactnormal = wallContactnormal.normalized;

                Vector3 checkpos = anims.transform.position;
                checkpos.y += transform.localScale.y * 1.75f;
             


                if (ClimbLayers == (ClimbLayers | (1 << collision.gameObject.layer))//This is a climbing surface dawg
                    && (playerState == PlayerStates.CLIMBING || Physics.Raycast(checkpos, transform.forward, ClimbCheckDistance))) //Not a step
                {
                    calcClimbNormal += normal;
                    calcClimbNormal = calcClimbNormal.normalized;
                    climbContactNormal = calcClimbNormal;
                }
                
                //climbContactNormal = normal;
            }


        }
        else if (GroundLayers == (GroundLayers | (1 << collision.gameObject.layer))) //Ground
        {
            groundContactNormal = normal;
            Debug.DrawLine(transform.position, transform.position + (1 * -normal), Color.green);
        }
        //if (degreeCheck <= maxClimbAngle)
        //{
        //    climbContactNormal = normal;
            
        //}
        
        //float upDot = Vector3.Dot(Vector3.up, normal);

        //if (upDot >= minGroundDotProduct)
        //{
        //    groundContactNormal = normal;
        //}
    }
    #endregion Collisions


    #region Debug
    
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Vector3 groundStartPos = transform.position + GroundCheckStartOffset;
        //Vector3 groundEndPos = groundStartPos + ((-Vector3.up) * GroundCheckDistance);
        //Gizmos.DrawLine(groundStartPos, groundEndPos);
        //Gizmos.DrawSphere(groundEndPos, GroundCheckRadius);

        //Gizmos.color = Color.blue;
        //Vector3 climbStartPos = transform.position + ClimbCheckStartOffset;
        //Vector3 climbEndPos = climbStartPos + ((transform.forward) * ClimbCheckDistance);
        //Gizmos.DrawLine(climbStartPos, climbEndPos);
        //Gizmos.DrawSphere(climbEndPos, ClimbCheckRadius);

        //Vector3 representAngle = Vector3.RotateTowards(transform.up, -transform.up, GroundToClimbAngle*Mathf.Deg2Rad, 0.0f);
        //Gizmos.DrawLine(transform.position, transform.position - (representAngle * 1.0f));

        //Vector3 representForwardAngle = Vector3.RotateTowards(transform.forward, transform.right, ForwardToClimbAngle * Mathf.Deg2Rad, 0.0f);
        //Gizmos.DrawLine(transform.position, transform.position - (representForwardAngle * 1.0f));

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + (-calcClimbNormal.normalized * 1));
        //Gizmos.color = Color.cyan;
        //Vector3 checkpos = anims.transform.position;
        //checkpos.y += transform.localScale.y * 1.75f;
        //Gizmos.DrawSphere(checkpos, 0.05f);
        //Gizmos.DrawLine(checkpos, checkpos + (transform.forward * 0.5f));
    }
    #endregion
}
