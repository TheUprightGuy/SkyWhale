using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CameraType
{
    PlayerCamera,
    WhaleCamera,
    GlideCamera,
    PuzzleCamera
}


public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Camera Manager exists!");
            Destroy(this);
        }
        instance = this;
    }

    public event Action<CameraType> switchCam;
    public void SwitchCamera(CameraType _cam)
    {
        if (switchCam != null)
        {
            switchCam(_cam);
        }
    }
}
