using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MenuOptions
{
    Main,
    NewGame,
    Options,
    Game,
    Audio,
    Video,
    Controls,
    Back,
    Quit,
    QuitToMenu,
    Continue
}

public class PauseMenuCanvasController : MonoBehaviour
{
    #region Singleton
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
    private void Start()
    {
        prevMenu = menu;
        Invoke("StartUpFunc", 0.01f);
    }

    private void StartUpFunc()
    {
        ToggleMenuOption(menu);
    }
    #endregion Setup

    [HideInInspector] public MenuOptions menu = MenuOptions.Main;
    [HideInInspector] public MenuOptions prevMenu;
    [HideInInspector] public AudioSource audioSource;

    [Header("Game Settings")]
    public GameSettings gameSettings;
    public GameSettings defaultSettings;

    public Action<MenuOptions> toggleMenuOption;
    public void ToggleMenuOption(MenuOptions _option)
    {
        switch (_option)
        {
            case MenuOptions.Continue:
            {
                //CallbackHandler.instance.TogglePause();
                break;
            }
            case MenuOptions.QuitToMenu:
            {
                //CallbackHandler.instance.QuitToMenu();
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

    public Action setSettings;
    public void SetSettings()
    {
        if (setSettings != null)
        {
            setSettings();
        }
    }
}
