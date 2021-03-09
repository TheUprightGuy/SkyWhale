using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacter : MonoBehaviour
{
    Animator animator;
    bool steering = false;
    float steeringTimer;

    Rigidbody rb;
    new CapsuleCollider collider;

    public float puntForce;
    public GameObject rod;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        collider.enabled = false;
        rb.isKinematic = true;
    }

    public void UpdateAnimState()
    {
        //animator.SetBool("Moving", moving);
        //animator.SetBool("Standing", standing);
        //animator.SetBool("Sitting", sitting);
        animator.SetBool("Sitting", true);
        animator.SetBool("Steering", steering);
    }

    // Update is called once per frame
    void Update()
    {
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

        UpdateAnimState();
    }

    public void Punt()
    {
        if (rb.isKinematic)
        {
            rb.isKinematic = false;
            rod.SetActive(false);
            rb.AddForce(Vector3.Normalize(transform.forward + transform.up) * puntForce);
        }
    }
}
