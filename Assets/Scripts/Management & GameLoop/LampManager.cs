using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CallbackHandler.instance.toggleLamp += ToggleLamp;
    }

    public void ToggleLamp(bool _toggle)
    {
        transform.GetChild(0).gameObject.SetActive(_toggle);
        transform.GetComponentInChildren<LineSetup>().enabled = _toggle;
    }
}
