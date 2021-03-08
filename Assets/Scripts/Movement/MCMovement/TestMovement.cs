using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    #region Setup
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    #endregion Setup

    public float moveSpeed;
    bool jumping;
    float distToGround;
    public bool freezeMe;

    public Animator animator;

    [Header("Required Fields")]
    public float jumpVelocity;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;

    Vector2 moveVector = Vector2.zero;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float runSpeed = 6.0f;
    public float walkSpeed = 2.0f;

    public Camera cam;
    public GameObject lamp;

    bool tutMsg;
    float safetyTimer;
   

    // Start is called before the first frame update
    void Start()
    {
        distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
        lamp.SetActive(false);
        CallbackHandler.instance.toggleLamp += ToggleLamp;
        CallbackHandler.instance.interact += Interact;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.toggleLamp -= ToggleLamp;
        CallbackHandler.instance.interact -= Interact;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventHandler.instance.gameState.inMenu) return;
        if (freezeMe)
        {
            safetyTimer -= Time.deltaTime;
            if (safetyTimer <= 0)
            {
                WhaleHandler.instance.MoveWhale();
            }
        }

        // Falling
        if (transform.position.y <= -55.0f)
        {
            Fader.instance.FadeOut();
            WhaleHandler.instance.ZeroOut();
        }

        if (freezeMe)
        {
            rb.velocity = Vector3.zero;
            currentSpeed = 0;
            return;
        }

        // Grounded - Allow Jump
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector3.up * jumpVelocity;
            jumping = true;
            animator.SetTrigger("Jump");
            //AudioManager.instance.PlaySound("jump");
        }
        // Falling - Apply Gravity Multiplier
        if (rb.velocity.y < 0)
        {
            jumping = false;
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Bigger Jumps - Lower Gravity Multiplier
        else if (jumping && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        moveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveVector = moveVector.normalized;

        if (moveVector != Vector2.zero)
        {
            float targetRot = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, turnSmoothTime);
        }
        animator.SetFloat("MovementSpeed", currentSpeed / runSpeed);

        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = ((running) ? runSpeed : walkSpeed) * moveVector.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayFlute();
            safetyTimer = 5.0f;
        }
    }

    private void FixedUpdate()
    {
        if (EventHandler.instance.gameState.inMenu) return;
        // Move Character
        rb.MovePosition(rb.position + transform.forward * currentSpeed * Time.fixedDeltaTime);
    }

    // Check if Player is Grounded
    public bool IsGrounded()
    {
        // Prevents Grounded at Start of Jump
        if (jumping)
            return false;

        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    public void ToggleLamp(bool _toggle)
    {
        lamp.SetActive(_toggle);
    }

    public void PlayFlute()
    {
        animator.SetBool("Flute", true);
        WhaleHandler.instance.StartHoming(transform);
        freezeMe = true;

        if (!tutMsg)
        {
            Invoke("LanternTutorial", 2.0f);
            // Change Objective Here
            tutMsg = true;
        }
    }

    public void LanternTutorial()
    {
        TutorialMessage lanternTutorial = new TutorialMessage();
        lanternTutorial.message = "Those clouds look pretty heavy, maybe something at the shop could help?";
        lanternTutorial.timeout = 5.0f;
        lanternTutorial.key = KeyCode.E;

        CallbackHandler.instance.AddMessage(lanternTutorial);
        CallbackHandler.instance.NextMessage();
    }

    public void Interact()
    {
        animator.ResetTrigger("Interact");
        animator.SetTrigger("Interact");
    }
}