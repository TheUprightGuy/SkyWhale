using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaCamera : MonoBehaviour
{
    #region Callbacks
    void Start()
    {
        CameraManager.instance.switchCam += SwitchCam;
    }

    private void OnDestroy()
    {
        CameraManager.instance.switchCam -= SwitchCam;
    }
    #endregion Callbacks

    public void SwitchCam(CameraType _cam)
    {
        if (_cam == CameraType.CinemaCamera)
        {
            GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Priority = 1;
        }
        else
        {
            GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Priority = 0;
        }
    }
}
