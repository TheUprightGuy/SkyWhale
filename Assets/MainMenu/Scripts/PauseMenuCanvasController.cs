﻿/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   PauseMenuCanvasController.cs
  Description :   Handles PauseUI elements. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MenuOptions
{
    Main,
    NewGame,
    Load,
    Options,
    Game,
    Audio,
    Video,
    Controls,
    Back,
    Quit,
    QuitToMenu,
    Continue,
    Save
}

public class PauseMenuCanvasController : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Description: Singleton Setup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public static PauseMenuCanvasController instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one CanvasController exists!");
            Destroy(this.gameObject);
        }
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    #endregion Singleton
    #region Setup
    /// <summary>
    /// Description: Setup Inputs.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Start()
    {
        prevMenu = menu;
        Invoke("StartUpFunc", 0.01f);
        VirtualInputs.GetInputListener(InputType.MENU, "Pause").MethodToCall.AddListener(Pause);
    }

    private void StartUpFunc()
    {
        ToggleMenuOption(menu);
        if (toggle)
            toggle.SetActive(false);
    }
    #endregion Setup

    [HideInInspector] public MenuOptions menu = MenuOptions.Main;
    [HideInInspector] public MenuOptions prevMenu;
    [HideInInspector] public AudioSource audioSource;

    public GameObject toggle;
    [Header("Game Settings")]
    public GameSettings gameSettings;
    public GameSettings defaultSettings;

    /// <summary>
    /// Description: Toggles pause on or off.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="type">InputState (Down/Held/Up)</param>
    public void Pause(InputState type)
    {
        toggle.SetActive(!toggle.activeSelf);
        CallbackHandler.instance.Pause(toggle.activeSelf);

        if (toggle.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            TimeSlowDown.instance.SlowDown();
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        TimeSlowDown.instance.SpeedUp();
    }

    public Action<MenuOptions> toggleMenuOption;
    /// <summary>
    /// Description: Toggles which menu to show.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_option">Menu to Display</param>
    public void ToggleMenuOption(MenuOptions _option)
    {
        switch (_option)
        {
            case MenuOptions.NewGame:
            {
                SaveManager.instance.saveToUse = 3;
                SaveManager.instance.LoadScene(3);
                break;
            }
            case MenuOptions.Continue:
            {
                toggle.SetActive(false); 
                Cursor.lockState = CursorLockMode.Locked;
                TimeSlowDown.instance.SpeedUp();
                break;
            }
            case MenuOptions.QuitToMenu:
            {
                SaveManager.instance.ReturnToMain();
                break;
            }
            case MenuOptions.Quit:
            {
                Application.Quit();
                break;
            }
            case MenuOptions.Back:
            {
                if (toggleMenuOption != null)
                {
                    menu = prevMenu;
                    prevMenu = MenuOptions.Main;
                    toggleMenuOption(menu);
                }
                break;
            }
            default:
            {
                if (toggleMenuOption != null)
                {
                    prevMenu = menu;
                    menu = _option;
                    toggleMenuOption(menu);
                }
                break;
            }
        }
    }

    /// <summary>
    /// Description: Sets game settings to saved settings.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public Action setSettings;
    public void SetSettings()
    {
        if (setSettings != null)
        {
            setSettings();
        }
    }
}
