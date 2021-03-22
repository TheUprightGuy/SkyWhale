using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbCamera : MonoBehaviour
{
    Vector3 offset;
    public Transform target;
    public float distance = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
        CameraManager.instance.switchCam += SwitchCam;
    }

    private void OnDestroy()
    {
        CameraManager.instance.switchCam -= SwitchCam;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position + target.rotation * offset * distance;
        transform.LookAt(target);
        //transform.rotation = target.rotation;
    }

    public void SetDistance(float _speed)
    {
        distance = 1.0f + _speed;
    }

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
