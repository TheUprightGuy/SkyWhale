using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resolution
{
    HD = 0,
    FULLHD,
    MASTERRACE
}
public class ResolutionOption : MonoBehaviour
{
    public Resolution resolution;
    public TMPro.TextMeshProUGUI text;
    private GameSettings settings;

    private void Start()
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
        resolution = settings.resolution;
        UpdateText();
    }

    public void IncreaseResolution()
    {
        if ((int)resolution < 2)
        {
            resolution++; 
        }
        UpdateText();
        settings.resolution = resolution;
    }

    public void LowerResolution()
    {
        if ((int)resolution > 0)
        {
            resolution--;
        }
        UpdateText();
        settings.resolution = resolution;
    }

    public void UpdateText()
    {
        switch(resolution)
        {
            case Resolution.HD:
            {
                text.SetText("1280x720");
                break;
            }
            case Resolution.FULLHD:
            {
                text.SetText("1920x1080");
                break;
            }
            case Resolution.MASTERRACE:
            {
                text.SetText("2560x1440");
                break;
            }
        }
    }
}
