/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   MenuButtonController.cs
  Description :   Button UI Element to toggle Menus. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    [Header("Required Fields")]
    public MenuOptions menuOption;

    // Local Variables
    bool keyDown;
    List<MenuButton> buttons = new List<MenuButton>();
    //[HideInInspector]
    public int index;

    #region Setup
    private void Awake()
    {
        foreach (MenuButton n in transform.GetComponentsInChildren<MenuButton>())
        {
            buttons.Add(n);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].thisIndex = i;
            buttons[i].menuButtonController = this;
        }

        index = -1;
    }
    #endregion Setup
    #region Callbacks
    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Start()
    {
        PauseMenuCanvasController.instance.toggleMenuOption += ToggleMenuOption;
    }

    private void OnDestroy()
    {
        PauseMenuCanvasController.instance.toggleMenuOption -= ToggleMenuOption;
    }
    #endregion Callbacks

    /// <summary>
    /// Description: Check if this is desired menu option to show
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_menuOption">Menu To Display</param>
    public void ToggleMenuOption(MenuOptions _menuOption)
    {
        if (menuOption == _menuOption)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
