/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   DumbCamera.cs
  Description :   Basic camera to follow glider. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbCamera : MonoBehaviour
{
    #region Callbacks
    /// <summary>
    /// Description: Callback Setup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void Start()
    {
        CameraManager.instance.switchCam += SwitchCam;
    }

    private void OnDestroy()
    {
        CameraManager.instance.switchCam -= SwitchCam;
    }
    #endregion Callbacks

    /// <summary>
    /// Description: Handles camera transition through camera manager.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_cam">Camera Type to Use</param>
    public void SwitchCam(CameraType _cam)
    {
        if (_cam == CameraType.GlideCamera)
        {
            GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Priority = 1;
        }
        else
        {
            GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Priority = 0;
        }
    }
}
