/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   CustomInputUI.cs
  Description :   Creates customizable control buttons. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomInputUI : MonoBehaviour
{
    public GameObject inputUIPrefab;
    /// <summary>
    /// Description: Sets up customizable control buttons, which take user input and bind to input manager.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void Start()
    {
        InputUIPrefab playerControls = Instantiate(inputUIPrefab, transform).GetComponent<InputUIPrefab>();
        playerControls.CreateUIElements("PLAYER", VirtualInputs.instance.currentInput.playerInput);
        playerControls.CreateUIElements("WHALE", VirtualInputs.instance.currentInput.whaleInput);
    }
}
