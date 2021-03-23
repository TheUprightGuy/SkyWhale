using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSlowDown : MonoBehaviour
{
    [Header("Debug")]
    public float timeScale = 1.0f;
    // Local Variables
    float slowMo = 0.01f;
    bool slowDown;
    Vector3 grav;

    public static TimeSlowDown instance;

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
            //timeScale = Mathf.Lerp(timeScale, slowMo, Time.deltaTime * Time.deltaTime * 1000.0f);
            //Physics.gravity = grav * timeScale;
        }
        else
        {
            //timeScale = Mathf.Lerp(timeScale, 1.0f, Time.deltaTime * 15.0f);
            //Physics.gravity = grav * timeScale;
        }
    }
}
