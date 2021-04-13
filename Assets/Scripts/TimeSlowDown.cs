/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   TimeSlowDown.cs
  Description :   Handles timescale without messing with unity time scale. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeSlowDown : MonoBehaviour
{
    [Header("Debug")]
    public float timeScale = 1.0f;
    // Local Variables
    float defaultGrayScale = 0.0f;
    float slowMo = 0.01f;
    bool slowDown;
    Vector3 grav;

    #region Singleton
    public static TimeSlowDown instance;
    UnityEngine.Rendering.Universal.ColorAdjustments adjustments;
    /// <summary>
    /// Description: Singleton Setup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Time Slow Down exists!");
            Destroy(this);
        }
        instance = this;
        grav = Physics.gravity;
    }
    #endregion Singleton
    #region Callbacks
    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    private void Start()
    {
        SaveManager.instance.load += GetCamReference;
        SaveManager.instance.load += SpeedUp;

        GetCamReference();
        
    }
    private void OnDestroy()
    {
        SaveManager.instance.load -= GetCamReference;
        SaveManager.instance.load -= SpeedUp;
    }
    #endregion Callbacks

    /// <summary>
    /// Description: Gets Camera Reference for Time Slowdown effect.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    public void GetCamReference()
    {
        if (!Camera.main.GetComponent<UnityEngine.Rendering.Volume>())
            return;

        Camera.main.GetComponent<UnityEngine.Rendering.Volume>().sharedProfile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out adjustments);
    }

    /// <summary>
    /// Description: Speeds Up/Slows Down Time.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    public void SlowDown()
    {
        slowDown = true;
    }
    public void SpeedUp()
    {
        slowDown = false;
    }

    /// <summary>
    /// Description: Lerps time change, changes FOV and saturation on slowdown/speed up.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    void Update()
    {
        if (slowDown)
        {
            timeScale = Mathf.Lerp(timeScale, slowMo, Time.deltaTime * 15.0f);
            //Physics.gravity = grav * timeScale;
        }
        else
        {
            timeScale = Mathf.Lerp(timeScale, 1.0f, Time.deltaTime * 5.0f);
            //Physics.gravity = grav * timeScale;
        }

        if (!Camera.main.GetComponent<Cinemachine.CinemachineVirtualCamera>())
            return;

        Camera.main.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView = ((timeScale / 2) * 100.0f) + 50.0f;
        adjustments.saturation.value = timeScale * 100.0f - 100.0f;
    }

    // Safety
    void OnApplicationQuit()
    {
         adjustments.saturation.value = defaultGrayScale;
    }
}
