using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementDirection
{
    SlightRotation,
    Forward,
    Circle
}


public class BackgroundWhaleMovement : MonoBehaviour
{
    public float animSpeed = 1.0f;
    public float speed = 1.0f;
    private Animator animator;
    public MovementDirection md;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator.SetBool("Moving", true);
        animator.speed = animSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        switch (md)
        {
            case MovementDirection.SlightRotation:
            {
                transform.Rotate(Vector3.up * Time.deltaTime * Time.deltaTime);
                break;
            }
            case MovementDirection.Forward:
            {
                break;
            }
            case MovementDirection.Circle:
            {
                transform.Rotate(Vector3.up * Time.deltaTime);
                break;
            }
        }
       
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
