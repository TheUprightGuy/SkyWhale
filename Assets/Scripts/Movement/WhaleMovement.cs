/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   WhaleMovement.cs
  Description :   Handles movement and rotation for the Whale. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
  
  Also slightly worked on by Jacob Gallagher when first working on dismount by glide and grapple.
*/

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

    #region Local Variables
    [HideInInspector] public bool tooClose;
    [HideInInspector] public bool control = true;
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
    PickUp pickUp;
    OrbitScript orbit;
    NewCharacter cc;
    GameObject cachedHeightRef;
    GrappleScript gs;
    bool pause;
    #endregion Local Variables

    /// <summary>
    /// Description: Gets Component References.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        orbit = GetComponent<OrbitScript>();
        pickUp = GetComponent<PickUp>();
        cc = GetComponentInChildren<NewCharacter>();
        gs = GetComponentInChildren<GrappleScript>();
        if (GameObject.Find("DismountPos"))
            dismountPosition = GameObject.Find("DismountPos").transform;
    }

    /// <summary>    
    /// Description: Sets callbacks and input methods.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
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

        EntityManager.instance.toggleControl += ToggleControl;
        CallbackHandler.instance.pause += Pause;
    }
    private void OnDestroy()
    {
        EntityManager.instance.toggleControl -= ToggleControl;
        CallbackHandler.instance.pause -= Pause;
    }

    /// <summary>
    /// Description: Freezes Movement.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_pause">Pause from main</param>
    void Pause(bool _pause)
    {
        pause = _pause;
    }

    /// <summary>
    /// Description: Sets callbacks and input methods.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_toggle">Control</param>
    public void ToggleControl(bool _toggle)
    {
        control = !_toggle;
    }

    /// <summary>
    /// Description: Spawns a Trailing Island.
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void AddIsland()
    {
        CallbackHandler.instance.SpawnCollectableIsland();
    }

    /// <summary>
    /// Description: Dismounts player from whale.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="arg0">Input state (Down/Held/Up)</param>
    private void Dismount(InputState arg0)
    {
        if (!control) 
            return;

        control = false;
        EntityManager.instance.OnDismountPlayer(dismountPosition);
        //CallbackHandler.instance.DismountPlayer(dismountPosition);
    }

    /// <summary>
    /// Description: Yaw/Pitch the whale to desired rotation - rolling body on yaw.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
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

    /// <summary>
    /// Description: Controls the whales movement speed.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
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

    /// <summary>
    /// Description: Levels out and lerps movement back to base speed if no changes made.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
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

    /// <summary>
    /// Description: Handles corrections to movement and rotation, as well as animation parameter updates.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void Update()
    {
        if (pause)
            return;

        float movement = currentSpeed / 2;
        float f = body.transform.rotation.eulerAngles.z;
        f = (f > 180) ? f - 360 : f;
        animator.SetFloat("Turning", f / 10.0f);
        animator.SetFloat("Movement", movement);
        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * accelSpeed);// * TimeSlowDown.instance.timeScale);

        if (control)
        {
            MovementCorrections();
        }
        GetDistance();

        Movement();
    }

    /// <summary>
    /// Description: Checks if whale is too close to an object.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
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

    /// <summary>
    /// Description: Starts the back off and orbit sequence, caching the island and height reference for orbit. Also triggers the catapult.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="hit">Object hit</param>
    void Crash(GameObject hit)
    {
        tooClose = true;
        control = false;
        buckTimer = 2.0f;

        NewIslandScript temp = (hit.GetComponent<NewIslandScript>()) ? hit.GetComponent<NewIslandScript>() : hit.GetComponentInParent<NewIslandScript>();
        cachedHeightRef = temp.heightRef;
    }

    /// <summary>
    /// Description: Handles the actual movement of the whale.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void FixedUpdate()
    {
        if (pause)
            return;

        // Reached minimum distance threshold to island
        if (tooClose)
        {
            rb.MovePosition(transform.position - transform.forward * maxSpeed * Time.deltaTime);// * TimeSlowDown.instance.timeScale);
            buckTimer -= Time.deltaTime;// * TimeSlowDown.instance.timeScale;
            CatapultPlayer();

            // Finished backing off to a safe distance
            if (buckTimer <= 0)
            {
                tooClose = false;
                moveSpeed = maxSpeed / 2.0f;
                orbit.SetOrbit(cachedHeightRef);
            }
        }
        else
        {
            rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.deltaTime);// * TimeSlowDown.instance.timeScale);
        }

        if (!control)
            return;

        // Slerps current rotation to make movement seem more organic
        Quaternion temp = Quaternion.Slerp(transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed);// * TimeSlowDown.instance.timeScale);
        rb.MoveRotation(temp);
    }

    /// <summary>
    /// Description: Defines desired movement vector and rotates body.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void Movement()
    {
        if (!control)
            return;

        desiredRoll = new Vector3(body.transform.eulerAngles.x, body.transform.eulerAngles.y, myRoll);
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.Euler(desiredRoll), Time.deltaTime * rotationSpeed);// * TimeSlowDown.instance.timeScale);
        // Rot
        desiredVec = new Vector3(myPitch, transform.eulerAngles.y + myTurn, 0);
    }

    /// <summary>
    /// Description: Catapult player on collision - requires rework given changes to player/whale transitions.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void CatapultPlayer()
    {
        if (cc)
        {
            cc.transform.parent = null;
            cc.Punt();
        }
    }

    /// <summary>
    /// Description: Stops the whale.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void ComeToHalt()
    {
        moveSpeed = 0.0f;
        currentSpeed = 0.0f;
    }

    /// <summary>
    /// Description: Stops the whale on player enter and transitions.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="other">Colliding Object</param>
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement pc = other.GetComponent<PlayerMovement>();
        if (pc)
        {
            // Change Layer so when we transition back we don't collide with whale instantly
            pc.gameObject.layer = LayerMask.NameToLayer("PlayerFromWhale");
            //pc.gameObject.SetActive(false);
            EntityManager.instance.TogglePlayer(false);

            OnPlayerMountWhale();
        }
    }

    public void OnPlayerMountWhale()
    {
        // Stop the whale and give control
        ComeToHalt();
        //control = true;
        gs.active = true;
    }

    /// <summary>
    /// Description: Swaps control.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void GiveControl()
    {
        orbit.enabled = false;
        pickUp.enabled = false;
        control = true;
        tooClose = false;
    }
}
