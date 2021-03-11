using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultsButton : MenuButton, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private GameSettings gameSettings;
    private GameSettings defaults;

    private void Start()
    {
        gameSettings = PauseMenuCanvasController.instance.gameSettings;
        defaults = PauseMenuCanvasController.instance.defaultSettings;
    }

    public override void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("Selected", true);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                animator.SetTrigger("Pressed");
                ResetDefaults();
            }
        }
        else
        {
            animator.SetBool("Selected", false);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        animator.SetTrigger("Pressed");
        ResetDefaults();
    }

    public void ResetDefaults()
    {
        switch(menuOptions)
        {
            case MenuOptions.Game:
            {
                //gameSettings.difficulty = defaults.difficulty;
                // Language
                break;
            }
            case MenuOptions.Audio:
            {
                gameSettings.masterVolume = defaults.masterVolume;
                gameSettings.soundVolume = defaults.soundVolume;
                gameSettings.musicVolume = defaults.musicVolume;
                gameSettings.subtitles = defaults.subtitles;
                break;
            }
            case MenuOptions.Video:
            {
                gameSettings.resolution = defaults.resolution;
                gameSettings.fullScreen = defaults.fullScreen;
                break;
            }
            case MenuOptions.Controls:
            {
                // Keybindings
                break;
            }
            default:
                break;
        }

        Debug.Log("Reset " + menuOptions.ToString() + " options to defaults.");
        PauseMenuCanvasController.instance.SetSettings();
    }
}