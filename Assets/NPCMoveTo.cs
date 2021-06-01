using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveTo : MonoBehaviour
{
    Animator animator;
    public Transform target;
    public Vector3 destination;
    Rigidbody RB;
    public Cinemachine.CinemachineVirtualCamera cam;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();

        SetDestination(target.position);
        transform.LookAt(target.position);

        cam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        cam.m_Priority = 2;
    }

    private void Start()
    {
        Invoke("InitialPause", 0.1f);
    }

    void InitialPause()
    {
        CallbackHandler.instance.CinematicPause(true);
        CameraManager.instance.LetterBox(true);
    }


    Vector3 currentVel = Vector3.zero;
    Vector3 groundContactNormal;
    Vector3 climbContactNormal;
    float distanceFromGround;
    public float runSpeed = 6.0f;
    public float maxRunAcceleration = 20f;
    private float currentVelMag;
    public float rotationSpeed = 0.1f;
    PlayerMovement pm;

    Vector3 dir;

    void SetDestination(Vector3 _destination)
    {
        destination = _destination;
    }

    public float dist;

    bool cinematic;

    private void FixedUpdate()
    {
        if (cinematic)
            return;

        dist = GetDistance();

        if (pm && !cinematic)
        {
            animator.SetFloat("MovementSpeed", 0.0f);
            cinematic = true;
            CallbackHandler.instance.CinematicPause(false);
            CameraManager.instance.Standard(false);
            GetComponent<NPCScript>().Interact(InputState.KEYDOWN);
            return;
        }
        GroundMovement(runSpeed, maxRunAcceleration);
        animator.SetFloat("MovementSpeed", 0.5f);
    }

    float GetDistance()
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(destination.x, destination.z));
    }

    /// <summary>
    /// <br>Description: Moves the position of the player using MovePosition based on speed and accel</br>
    /// <br>Author: Jack Belton</br>
    /// <br>Last Updated: 7/04/21</br>
    /// </summary>
    /// <param name="speed">The desired speed to accelerate towards</param>
    /// <param name="accel">The rate to move towards speed</param>
    void GroundMovement(float speed, float accel)
    {
        dir = Vector3.Normalize(destination - transform.position);
        Vector2 dir2 = new Vector2(dir.x, dir.z);

        Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, groundContactNormal);
        float xzMag = dir2.magnitude;//Key is pressed
        projectedForward *= xzMag;

        Vector3 desiredVel = Vector3.zero;
        desiredVel = (projectedForward).normalized; //Input direction
        desiredVel *= speed; //Amount to move in said direction


        //Vector to actually move by
        Vector3 actualVel = currentVel = Vector3.MoveTowards(currentVel, //Current moving velocity
                                                                desiredVel,
                                                                    accel * Time.fixedDeltaTime); //Amount to change by

        //Against a wall in the front dir
        if (climbContactNormal != Vector3.zero)
        {
            float currentForwardVel = Vector3.Dot(currentVel, climbContactNormal);
            Vector3 negateforce = currentForwardVel * climbContactNormal;
            currentVel -= negateforce;
        }

        //animation walk speeds
        currentVelMag = currentVel.magnitude;

        RB.MovePosition(transform.position + currentVel);// * TimeSlowDown.instance.timeScale);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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
        groundContactNormal = normal;
    }
}
