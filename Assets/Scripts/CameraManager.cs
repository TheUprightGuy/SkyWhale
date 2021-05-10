/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   CameraManager.cs
  Description :   Handles switching between cameras. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CameraType
{
    PlayerCamera,
    WhaleCamera,
    GlideCamera,
    PuzzleCamera,
    WhaleGrappleCamera,
    OtherCamera
}


public class CameraManager : MonoBehaviour
{
    #region Singleton
    public static CameraManager instance;
    /// <summary>
    /// Description: Setup Singleton.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Camera Manager exists!");
            Destroy(this);
        }
        instance = this;
    }
    #endregion Singleton

    public event Action<CameraType> switchCam;
    /// <summary>
    /// Description: Switches Camera to desired type.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    /// <param name="_cam">Desired Camera</param>
    public void SwitchCamera(CameraType _cam)
    {
        if (switchCam != null)
        {
            switchCam(_cam);
        }
    }


    float zoom;
    bool change;

    public void LetterBox()
    {
        zoom = 0.8f;
        change = true;
    }

    public void Standard()
    {
        zoom = 1.0f;
        change = true;
    }

    public float y;

    private void Update()
    {
        // TEMP KEYS
        if (Input.GetKeyDown(KeyCode.U))
        {
            LetterBox();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Standard();
        }

        if (!change)
            return;

        float h = Mathf.MoveTowards(Camera.main.rect.height, zoom, Time.deltaTime / 2.0f);
        y = (1.0f - h) / 2.0f;
        Camera.main.rect = new Rect(0, y, 1, h);


        if (Mathf.Abs(h - zoom) < Mathf.Epsilon)
            change = false;
    }
}
