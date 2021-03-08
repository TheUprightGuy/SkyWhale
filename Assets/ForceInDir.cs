using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceInDir : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        SJ = GetComponent<SpringJoint>();
    }

    public float RotSpeed = 1.0f;
    public float PullSpeed = 0.25f;
    public float forceMultiplier = 1.0f;
    public float gravityClamp = 1.0f;
    private float forceLR;
    private float forceUD;

    Rigidbody RB;
    SpringJoint SJ;
    // Update is called once per frame
    void FixedUpdate()
    {

        forceLR = Input.GetAxis("Horizontal");
        forceUD = Input.GetAxis("Vertical");

        
        Vector3 frontalForce = transform.forward * forceUD * forceMultiplier;
        Vector3 sideForce = transform.right * forceLR * forceMultiplier;

        Vector3 limited = Vector3.ClampMagnitude(frontalForce + sideForce, Physics.gravity.magnitude * gravityClamp);
        RB.AddForce(limited);


        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, -RotSpeed, 0));
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, RotSpeed, 0));
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            SJ.maxDistance += PullSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            SJ.maxDistance -= PullSpeed * Time.deltaTime;
        }
    }
}
