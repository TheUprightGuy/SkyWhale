using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbCamera : MonoBehaviour
{
    #region Callbacks
    // Start is called before the first frame update
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
