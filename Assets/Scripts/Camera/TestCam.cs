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
        cam.m_XAxis.m_MaxSpeed = 0;
        cam.m_YAxis.m_MaxSpeed = 0;
        cam.m_RecenterToTargetHeading.m_enabled = true;

        if (Input.GetMouseButton(0))
        {
            cam.m_XAxis.m_MaxSpeed = 400;
            cam.m_YAxis.m_MaxSpeed = 5;
            cam.m_RecenterToTargetHeading.m_enabled = false;
        }
    }
}
