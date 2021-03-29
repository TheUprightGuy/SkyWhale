using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeSlowDown : MonoBehaviour
{
    [Header("Debug")]
    public float timeScale = 1.0f;
    // Local Variables
    float slowMo = 0.01f;
    bool slowDown;
    Vector3 grav;

    public static TimeSlowDown instance;
    UnityEngine.Rendering.Universal.ColorAdjustments adjustments;

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

    private void Start()
    {
        Camera.main.GetComponent<UnityEngine.Rendering.Volume>().sharedProfile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out adjustments);
    }

    public void SlowDown()
    {
        slowDown = true;
    }

    public void SpeedUp()
    {
        slowDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (slowDown)
        {
            timeScale = Mathf.Lerp(timeScale, slowMo, Time.deltaTime * 15.0f);
            Physics.gravity = grav * timeScale;
        }
        else
        {
            timeScale = Mathf.Lerp(timeScale, 1.0f, Time.deltaTime * 5.0f);
            Physics.gravity = grav * timeScale;
        }

        Camera.main.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView = ((timeScale / 2) * 100.0f) + 50.0f;
        adjustments.saturation.value = timeScale * 100.0f - 100.0f;
    }
}
