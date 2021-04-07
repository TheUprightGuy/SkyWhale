/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   DisplayValue.cs
  Description :   Displays related value next to slider and updates game settings SO. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SliderType
{
    Master,
    Sound,
    Music
}

public class DisplayValue : MonoBehaviour
{
    public SliderType sliderType;

    #region Setup
    TMPro.TextMeshProUGUI text;
    Slider slider;
    GameSettings settings;

    /// <summary>
    /// Description: Gets local Components.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        slider = GetComponentInChildren<Slider>();
    }

    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Start()
    {
        settings = PauseMenuCanvasController.instance.gameSettings;
        SetSettings();
        SetText();

        PauseMenuCanvasController.instance.setSettings += SetSettings;
    }

    private void OnDestroy()
    {
        PauseMenuCanvasController.instance.setSettings -= SetSettings;
    }
    #endregion Setup

    /// <summary>
    /// Description: Adjust AudioManager values by slider, and updates game settings SO.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void SetSettings()
    {
        switch (sliderType)
        {
            case SliderType.Master:
            {
                slider.value = settings.masterVolume;
                Audio.AudioManager.instance.OnVolumeAdjusted(0, slider.value);
                break;
            }
            case SliderType.Music:
            {
                slider.value = settings.musicVolume;
                Audio.AudioManager.instance.OnVolumeAdjusted(1, slider.value);
                break;
            }
            case SliderType.Sound:
            {
                slider.value = settings.soundVolume;
                Audio.AudioManager.instance.OnVolumeAdjusted(2, slider.value);
                break;
            }
        }
    }

    /// <summary>
    /// Description: Sets text element to slider value.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void SetText()
    {
        if (text && slider)
        {
            switch (sliderType)
            {
                case SliderType.Master:
                {
                    settings.masterVolume = (int)slider.value;
                    break;
                }
                case SliderType.Sound:
                {
                    settings.soundVolume = (int)slider.value;
                    break;
                }
                case SliderType.Music:
                {
                    settings.musicVolume = (int)slider.value;
                    break;
                }
            }

            SetSettings();
            text.SetText(slider.value.ToString());
        }
    }
}
