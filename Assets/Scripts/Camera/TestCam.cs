using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TestCam : MonoBehaviour
{
    CinemachineFreeLook cam;
    private void Awake()
    {
        cam = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("`"))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        cam.m_XAxis.m_MaxSpeed = 400;
        cam.m_YAxis.m_MaxSpeed = 5;
        cam.m_RecenterToTargetHeading.m_enabled = false;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
