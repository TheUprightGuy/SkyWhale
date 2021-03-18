using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 m_EulerAngleVelocity;
    public Vector3 moveDirection;

    public bool moveTest;
    public bool velTest;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (moveTest)
        {
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
            rb.MovePosition(transform.position + moveDirection * Time.fixedDeltaTime);
        }

        if (velTest)
        {
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * 0.2f);
            rb.angularVelocity = deltaRotation.eulerAngles;
            rb.velocity = moveDirection;
        }
    }
}
