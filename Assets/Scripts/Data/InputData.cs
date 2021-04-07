/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   InputData.cs
  Description :   SO that contains custom inputs. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Input", menuName = "Data/Input", order = 1)]
public class InputData : ScriptableObject
{
    [Header("PLAYER CONTROLS")]
    public List<InputListener> playerInput;
    [Header("WHALE CONTROLS")]
    public List<InputListener> whaleInput;
    [Header("MENU CONTROLS")]
    public List<InputListener> menuInput;

    /// <summary>
    /// Description: Sets listeners for particular input types.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <param name="_type">Input Types</param>
    /// <param name="_inputs">Set of Listeners</param>
    public void SetInputs(InputType _type, List<InputListener> _inputs)
    {
        switch (_type)
        {
            case InputType.PLAYER:
            {
                if (playerInput == null)
                {
                    playerInput = new List<InputListener>();
                }
                playerInput = _inputs;
                break;
            }
            case InputType.WHALE:
            {
                if (whaleInput == null)
                {
                    whaleInput = new List<InputListener>();
                }
                whaleInput = _inputs;
                break;
            }
            case InputType.MENU:
            {
                if (menuInput == null)
                {
                    menuInput = new List<InputListener>();
                }
                menuInput = _inputs;
                break;
            }
        }
    }

    /// <summary>
    /// Description: Copy one set of inputs to another - used for rest to defaults.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <param name="_data">Inputs to copy from</param>
    public void CopyInputs(InputData _data)
    {
        // Done in this fashion to not lose listeners.
        for (int i = 0; i < playerInput.Count; i++)
        {
            playerInput[i].KeyToListen = _data.playerInput[i].KeyToListen;
        }
        for (int i = 0; i < whaleInput.Count; i++)
        {
            whaleInput[i].KeyToListen = _data.whaleInput[i].KeyToListen;
        }
        for (int i = 0; i < menuInput.Count; i++)
        {
            menuInput[i].KeyToListen = _data.menuInput[i].KeyToListen;
        }
    }
}
