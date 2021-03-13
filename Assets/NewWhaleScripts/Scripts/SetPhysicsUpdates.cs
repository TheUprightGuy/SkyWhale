using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPhysicsUpdates : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        SetUpdates();
    }

    public void SetUpdates()
    {
        text.SetText(slider.value.ToString());
        Time.fixedDeltaTime = 1.0f / (float)slider.value;
    }
}
