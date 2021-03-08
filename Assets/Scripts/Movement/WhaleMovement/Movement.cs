using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Singleton
    public static Movement instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Whale Exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        rb = GetComponent<Rigidbody>();
        orbit = GetComponent<OrbitScript>();
    }
    #endregion Singleton 

    [Header("Setup Fields")]
    public GameObject body;
    private Rigidbody rb;
    public Animator animator;
    public TestMovement player;
    public CharacterControllerScript rider;
    public Cinemachine.CinemachineFreeLook followCam;
    public FollowCamera followCamera;
    public GameObject front;
    [Header("Movement")]
    public float currentSpeed = 0.0f;
    public float moveSpeed = 1;
    public float accelSpeed = 1;
    public float maxSpeed = 5.0f;
    [Header("Upgrade Objects")]
    public GameObject saddle;
    public GameObject npc;
    bool endScreen;

    #region Local Variables
    WhaleInfo whaleInfo;
    static bool tutMessage = false;
    [HideInInspector] public OrbitScript orbit;
    [HideInInspector] public bool inRange;
    [HideInInspector] public bool orbiting;
    float islandMod = 0.0f;
    float distance;
    float rotationSpeed = 0.2f;
    Vector3 desiredVec;
    Vector3 desiredRoll;
    float myRoll = 0.0f;
    float myTurn = 0.0f;
    float myPitch = 0.0f;
    float turnSpeed = 40;
    float liftSpeed = 20;
    float rollSpeed = 20;
    float dotProduct;
    [HideInInspector] public bool homing;
    [HideInInspector] public bool exit;
    #endregion Local Variables

    #region Setup
    private void Start()
    {
        desiredVec = body.transform.eulerAngles;
        temp.SetActive(false);
        whaleInfo = CallbackHandler.instance.whaleInfo;
        saddle.SetActive(false);

        WhaleHandler.instance.pickUpMC += PickUpMC;
        WhaleHandler.instance.zeroOut += ZeroOut;
        CallbackHandler.instance.unlockSaddle += UnlockSaddle;
        CallbackHandler.instance.addCollectableMan += UnlockNPC;
    }
    private void OnDestroy()
    {
        WhaleHandler.instance.pickUpMC -= PickUpMC;
        WhaleHandler.instance.zeroOut -= ZeroOut;
        CallbackHandler.instance.unlockSaddle -= UnlockSaddle;
        CallbackHandler.instance.addCollectableMan -= UnlockNPC;
    }
    #endregion Setup
    #region Upgrades
    public void UnlockSaddle()
    {
        saddle.SetActive(true);
        maxSpeed = 6.5f;
        rider.transform.localPosition = rider.newSaddlePos.localPosition;
    }
    public void UnlockNPC()
    {
        npc.SetActive(true);
    }
    #endregion Upgrades
    #region PickUp&DropOff
    public void MoveCharacter()
    {
        followCam.gameObject.SetActive(true);
        rider.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
        player.transform.parent = null;
        player.transform.position = cachedPosition;
        followCamera.enabled = false;
        player.freezeMe = false;
        EventHandler.instance.gameState.playerOnIsland = true;
        CompassRotation.instance.whale = player.transform;
    }
    public void PickUpMC()
    {
        followCam.gameObject.SetActive(false);
        rider.gameObject.SetActive(true);
        player.animator.SetBool("Flute", false);
        player.freezeMe = false;
        player.gameObject.SetActive(false);
        orbiting = false;
        homing = false;
        whaleInfo.ToggleLeashed(false);
        exit = true;
        orbit.leashObject.GetComponent<MeshCollider>().convex = true;
        followCamera.enabled = true;
        WhaleHandler.instance.MoveToSaddle();
        EventHandler.instance.gameState.playerOnIsland = false;
        CompassRotation.instance.whale = this.transform;

        if (npc.active && !endScreen)
        {
            CallbackHandler.instance.ShowEndScreen();
            endScreen = true;
        }
    }
    #endregion PickUp&DropOff
    // Update is called once per frame
    void Update()
    {
        if (EventHandler.instance.gameState.inMenu) return;
        if (orbiting)
        {
            animator.SetFloat("Movement", currentSpeed * islandMod / 2);
            currentSpeed = Mathf.Lerp(currentSpeed, 1.0f, Time.deltaTime);
            return;
        }
        if (homing)
        {
            animator.SetFloat("Movement", currentSpeed * islandMod / 2);
            currentSpeed = Mathf.Lerp(currentSpeed, 3.0f, Time.deltaTime);
            return;
        }

        float movement = currentSpeed * islandMod / 2;
        float f = body.transform.rotation.eulerAngles.z;
        f = (f > 180) ? f - 360 : f;
        animator.SetFloat("Turning", f / 10.0f);
        animator.SetFloat("Movement", movement);
        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * accelSpeed);   
        float slowSpeedTurnBonus = (maxSpeed / currentSpeed);

        // Yaw
        if (Input.GetKey(KeyCode.D))
        {

            if (myTurn + Time.deltaTime * turnSpeed * slowSpeedTurnBonus < 40)
            {
                myTurn += Time.deltaTime * turnSpeed * slowSpeedTurnBonus;
            }
           
            if (myRoll - Time.deltaTime * rollSpeed > -10)
            {
                myRoll -= Time.deltaTime * rollSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (myTurn - Time.deltaTime * turnSpeed * slowSpeedTurnBonus > -40)
            {
                myTurn -= Time.deltaTime * turnSpeed * slowSpeedTurnBonus;
            }
            
            if (myRoll + Time.deltaTime * rollSpeed < 10)
            {
                myRoll += Time.deltaTime * rollSpeed;
            }
        }
        else
        {
            myTurn = Mathf.Lerp(myTurn, 0, Time.deltaTime * turnSpeed);
            myRoll = Mathf.Lerp(myRoll, 0, Time.deltaTime * rollSpeed * 5);
        }
        // Pitch
        if (Input.GetKey(KeyCode.W))
        {
            if (myPitch + Time.deltaTime * liftSpeed < 30)
            {
                myPitch += Time.deltaTime * liftSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (myPitch - Time.deltaTime * liftSpeed > -30)
            {
                myPitch -= Time.deltaTime * liftSpeed;
            }
        }
        else
        {
            myPitch = Mathf.Lerp(myPitch, 0.0f, Time.deltaTime);
        }
        // Move
        if (Input.GetKey(InputHandler.instance.move))
        {
            if (moveSpeed < maxSpeed)
            {
                moveSpeed += accelSpeed * Time.deltaTime;
            }
        }
        else
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

        if (!EventHandler.instance.gameState.gamePaused)
        {
            // Landing Tooltip
            WhaleHandler.instance.LandingTooltip(orbit.leashObject && CheckBelow() != Vector3.zero);
            //DropOff
            if (Input.GetKeyDown(InputHandler.instance.orbit))
            {
                if (orbit.leashObject && CheckBelow() != Vector3.zero)
                {
                    orbit.leashObject.GetComponent<MeshCollider>().convex = false;
                    Fader.instance.FadeOut(this);
                    orbiting = true;
                    WhaleHandler.instance.LandingTooltip(false);
                    if (!tutMessage)
                    {
                        Invoke("Tutorials", 2.0f);
                        tutMessage = true;
                    }
                }
            }
        }
    }
    public void ZeroOut()
    {
        currentSpeed = 0.0f;
        moveSpeed = 0.0f;
        myTurn = 0.0f;
        myRoll = 0.0f;
        myPitch = 0.0f; 
        rb.velocity = Vector3.zero;
        desiredRoll = Vector3.zero;
        desiredVec = new Vector3(myPitch, transform.eulerAngles.y + myTurn, transform.eulerAngles.z);
    }

    public void Tutorials()
    {
        TutorialMessage movementTutorial = new TutorialMessage();
        movementTutorial.message = "Use the WASD keys to move around. \nPress Shift to run.";
        movementTutorial.timeout = 5.0f;
        movementTutorial.key = KeyCode.LeftShift;

        TutorialMessage resourceTutorial = new TutorialMessage();
        resourceTutorial.message = "Make sure to collect resources while you're here!";
        resourceTutorial.timeout = 5.0f;
        resourceTutorial.key = KeyCode.E;

        TutorialMessage leaveTutorial = new TutorialMessage();
        leaveTutorial.message = "When you're ready, press F to leave the island.";
        leaveTutorial.timeout = 5.0f;
        leaveTutorial.key = KeyCode.F;

        CallbackHandler.instance.AddMessage(movementTutorial);
        CallbackHandler.instance.AddMessage(resourceTutorial);
        CallbackHandler.instance.AddMessage(leaveTutorial);
        CallbackHandler.instance.NextMessage();
    }


    private void FixedUpdate()
    {
        if (EventHandler.instance.gameState.inMenu) return;
        desiredRoll = new Vector3(body.transform.eulerAngles.x, body.transform.eulerAngles.y, myRoll);
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.Euler(desiredRoll), Time.deltaTime * rotationSpeed);
        // Rot
        desiredVec = new Vector3(myPitch, transform.eulerAngles.y + myTurn, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0)), Time.deltaTime * 10.0f);

        if (orbiting || homing)
        {
            rb.MovePosition(transform.position + transform.forward * islandMod * currentSpeed * Time.deltaTime);
            return;
        }

        if (exit)
        {
            rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.deltaTime);
            return;
        }

        if (inRange)
        {
            distance = Mathf.Infinity;

            if (orbit.leashObject)
            {
                Vector3 closestPoint = orbit.leashObject.GetComponent<MeshCollider>().ClosestPoint(front.transform.position);
                distance = Vector3.Distance(closestPoint, front.transform.position);

                Debug.DrawLine(front.transform.position, closestPoint, Color.red);

                // New Attempt
                if (distance < 30.0f)
                {
                    float perc = Mathf.Clamp01(distance / 30.0f);

                    Vector3 pointNorm = Vector3.Normalize(closestPoint - front.transform.position);
                    dotProduct = 1 - Vector3.Dot(front.transform.forward, pointNorm);
                    islandMod = Mathf.Clamp01(perc / dotProduct);
                }
            }
        }
        else
        {
            islandMod = 1.0f;
        }

        rb.MovePosition(transform.position + transform.forward * islandMod * currentSpeed * Time.deltaTime);
    }

    public GameObject temp;
    RaycastHit hit;
    public Vector3 cachedPosition;
    public LayerMask layerMask;

    public Vector3 CheckBelow()
    {
        Vector3 closestPoint = orbit.leashObject.GetComponent<MeshCollider>().ClosestPoint(front.transform.position);
        float checkDistance = Vector3.Distance(front.transform.position, closestPoint);

        Vector3 dir = closestPoint - front.transform.position;

        if (Physics.Raycast(front.transform.position, Vector3.down, out hit, 100.0f, ~layerMask))
        {
            if (!hit.collider.isTrigger)
            {

                /*
                 * Get the location of the hit.
                 * This data can be modified and used to move your object.
                 */
                //temp.SetActive(true);
                //temp.transform.position = hit.point;
                //Instantiate(temp, hit.point, Quaternion.identity);
                Debug.Log("Hit");
                cachedPosition = hit.point;
                return hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }
        /*else if (checkDistance < 12.0f && dir.y < 0)
        {
            //temp.SetActive(true);
            //temp.transform.position = hit.point;
            // Need to add a slight push inwards to the island
            Debug.Log("Side Hit");
            return closestPoint;
        }*/
        else
        {
            Debug.Log("No Hit");
            return Vector3.zero;
        }
    }

    public void Orbit(bool _toggle)
    {
        if (inRange)
        {
            whaleInfo.ToggleLeashed(_toggle);
            if (whaleInfo.leashed)
            {
                orbit.SetOrbitDirection();
            }
        }
        else
        {
            whaleInfo.ToggleLeashed(false);
        }
    }
}
