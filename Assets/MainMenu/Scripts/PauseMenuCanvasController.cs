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

    public void Pause(InputState type)
    {
        toggle.SetActive(!toggle.activeSelf);
    }

    public Action<MenuOptions> toggleMenuOption;
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

    public Action setSettings;
    public void SetSettings()
    {
        if (setSettings != null)
        {
            setSettings();
        }
    }
}
