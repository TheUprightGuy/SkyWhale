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
    private void Awake()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        slider = GetComponentInChildren<Slider>();
    }
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
