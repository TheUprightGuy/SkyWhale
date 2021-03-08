using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorRotationScript : MonoBehaviour
{
    public GameObject door;
    public SlidingDoorScript parent;

    public bool rotatingLocks;
    public bool slidingDoors;

    public GameObject buttonLock;
    public GameObject middleLock;
    public GameObject outsideLock;

    // Update is called once per frame
    void Update()
    {
        if (rotatingLocks)
        {
            if (buttonLock.transform.eulerAngles.z >= -180 && buttonLock.transform.eulerAngles.z <= 180)
            {
                outsideLock.transform.eulerAngles += new Vector3(0, 0, 50) * Time.deltaTime * 3;

                middleLock.transform.eulerAngles += new Vector3(0, 0, 50) * Time.deltaTime * 2;

                buttonLock.transform.eulerAngles += new Vector3(0, 0, 50) * Time.deltaTime;
            }
            else
            {
                rotatingLocks = false;
                slidingDoors = true;
                parent.doorPFX.StopPFX();
                parent.doorPFX.PlayDoorPFX();
            }
        }
        if (slidingDoors)
        {
            if (door.transform.localPosition.x > -1.0f)
            {
                buttonLock.transform.localPosition += new Vector3(0, 0, 0.1f) * Time.deltaTime;
                door.transform.localPosition += new Vector3(-1, 0, 0) * Time.deltaTime;
            }
            else
            {
                parent.doorPFX.StopDoorPFX();
                this.enabled = false;
            }
        }

    }
}
