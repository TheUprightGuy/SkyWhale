using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("Movement Positions")]
    public Transform targetPos;
    public Transform campFirePos;
    public Transform saddlePos;
    public Transform newSaddlePos;
    public float moveSpeed;

    [Header("Rod Positions")]
    public Transform tuskPos;
    public Transform handPos;
    public GameObject rod;

    [Header("Local Variables")]
    public bool moving;
    public bool standing;
    public bool sitting;
    public bool steering;
    public float sitTransTimer = 30.0f;
    public float steeringTimer;
    #region Setup
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    #endregion Setup
    #region Callbacks
    private void Start()
    {
        WhaleHandler.instance.moveToSaddle += MoveToSaddle;
        WhaleHandler.instance.moveToFire += MoveToFire;
        CallbackHandler.instance.unlockSaddle += NewSaddlePos;
        SitDown();
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.unlockSaddle -= NewSaddlePos;
        WhaleHandler.instance.moveToSaddle -= MoveToSaddle;
        WhaleHandler.instance.moveToFire -= MoveToFire;
    }
    #endregion Callbacks

    public void NewSaddlePos()
    {
        saddlePos = newSaddlePos;
    }

    private void Update()
    {
        if (moving)
        {
            Quaternion desiredRot = Quaternion.LookRotation(targetPos.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * 5.0f);
            transform.Translate((Vector3.forward) * Time.deltaTime * moveSpeed);
            if (Vector3.Distance(transform.position, targetPos.position) < 0.2f)
            {
                ReachedDestination();
            }
        }
        if (sitting)
        {
            sitTransTimer -= Time.deltaTime;
            if (sitTransTimer <= 0)
            {
                sitTransTimer = 30.0f;
                SwitchPose();
            }
            steeringTimer -= Time.deltaTime;
            if (steeringTimer <= 0 && steering)
            {
                steering = false;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                steering = true;
                steeringTimer = 3.0f;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FeedWhale();
            }
        }

        UpdateAnimState();
    }

    public void MoveToSaddle()
    {
        WalkTo(saddlePos);
    }
    public void MoveToFire()
    {
        WalkTo(campFirePos);
    }

    public void SitDown()
    {
        if (!sitting)
        {
            animator.SetTrigger("SitDown");
            animator.ResetTrigger("StandUp");
        }
        sitting = true;
        standing = false;
        UpdateAnimState();
    }

    public void StandUp()
    {
        if (!standing)
        {
            animator.ResetTrigger("SitDown");
            animator.SetTrigger("StandUp");
        }
        standing = true;
        sitting = false;
        UpdateAnimState();
    }

    public void WalkTo(Transform _target)
    {
        StandUp();

        if (_target == campFirePos)
        {
            rod.transform.parent = tuskPos;
            rod.transform.localPosition = Vector3.zero;
        }

        targetPos = _target;
        moving = true;
    }

    public void ReachedDestination()
    {
        if (targetPos == saddlePos)
        {
            rod.transform.parent = handPos;
            rod.transform.localPosition = Vector3.zero;
        }

        SitDown();
        moving = false;
        targetPos = null;
    }

    public void SwitchPose()
    {
        animator.SetTrigger("SitTransition");
    }

    public void FeedWhale()
    {
        animator.SetTrigger("FeedWhale");
    }

    public void UpdateAnimState()
    {
        animator.SetBool("Moving", moving);
        animator.SetBool("Standing", standing);
        animator.SetBool("Sitting", sitting);
        animator.SetBool("Steering", steering);
    }
}
