using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    private Toggle toggle;
    private GameSettings settings;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    // Start is called before the first frame update
    void Start()
    {
        settings = PauseMenuCanvasController.instance.gameSettings;
        SetSettings();

        PauseMenuCanvasController.instance.setSettings += SetSettings;
    }

    private void OnDestroy()
    {
        PauseMenuCanvasController.instance.setSettings -= SetSettings;
    }

    public void SetSettings()
    {
        toggle.isOn = settings.fullScreen;
    }

    public void ToggleFullscreen()
    {
        if (settings)
        {
            settings.fullScreen = toggle.isOn;
        }
    }
}
