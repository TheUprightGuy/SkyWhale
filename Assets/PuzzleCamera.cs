using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCamera : MonoBehaviour
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
    #region Setup
    MouseOverHighlight mh;
    private void Awake()
    {
        mh = GetComponent<MouseOverHighlight>();
    }
    #endregion Setup

    public void SwitchCam(CameraType _cam)
    {
        if (_cam == CameraType.PuzzleCamera)
        {
            GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Priority = 1;
            mh.enabled = true;
        }
        else
        {
            GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Priority = 0;
            mh.enabled = false;
        }
    }
}
