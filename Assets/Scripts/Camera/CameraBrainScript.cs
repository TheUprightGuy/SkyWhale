using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBrainScript : MonoBehaviour
{
    public Cinemachine.CinemachineBrain brain;
    private void Awake()
    {
        brain = GetComponent<Cinemachine.CinemachineBrain>();
    }
    // Start is called before the first frame update
    void Start()
    {
        CallbackHandler.instance.cutCam += CutCam;
        CallbackHandler.instance.lerpCam += LerpCam;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.cutCam -= CutCam;
        CallbackHandler.instance.lerpCam -= LerpCam;
    }

    public void CutCam()
    {
        brain.m_DefaultBlend.m_Time = 0;
    }

    public void LerpCam()
    {
        brain.m_DefaultBlend.m_Time = 1.0f;
    }
}
