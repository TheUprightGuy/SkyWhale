using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceInDir : MonoBehaviour
{
    [Header("Type")]
    public MoveTypes moveType;

    [Header("Movement")]
    public Vector3 dir;
    public float force;

    [Header("Rotation")]
    public Vector3 rot;
    public float torque;
    public enum MoveTypes
    {
        VELOCITY,
        MOVEPOS,
        ADDFORCE
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {

    }

    private void Update()
    {
        rot = rot.normalized;
        dir = dir.normalized;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        Quaternion deltaRotation = Quaternion.Euler((rot * torque) * TimeSlowDown.instance.timeScale);

        switch (moveType)
        {
            case MoveTypes.VELOCITY:
                GetComponent<Rigidbody>().velocity += dir * force * TimeSlowDown.instance.timeScale;
                GetComponent<Rigidbody>().angularVelocity += rot * torque * TimeSlowDown.instance.timeScale;
                break;
            case MoveTypes.MOVEPOS:
                GetComponent<Rigidbody>().MovePosition(transform.position + dir * force * TimeSlowDown.instance.timeScale);
                GetComponent<Rigidbody>().MoveRotation(transform.rotation * deltaRotation);

                break;
            case MoveTypes.ADDFORCE:
                GetComponent<Rigidbody>().AddForce(dir * force * TimeSlowDown.instance.timeScale);
                GetComponent<Rigidbody>().AddTorque(rot * torque * TimeSlowDown.instance.timeScale);
                break;
            default:
                break;
        }

    }
}
