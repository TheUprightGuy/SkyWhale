/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   Keybind.cs
  Description :   UI Element for Keybind. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Keybind : MonoBehaviour
{
    public InputListener inputRef;
    bool listening;

    public TMPro.TextMeshProUGUI actionName;
    public TMPro.TextMeshProUGUI inputKey;

    /// <summary>
    /// Description: Setup reference that this key will refer to.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <param name="_input">Input Reference</param>
    public void Setup(InputListener _input)
    {
        inputRef = _input;
        UpdateElements();
    }

    #region Callbacks
    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void Start()
    {
        VirtualInputs.instance.resetToDefaults += UpdateElements;
    }
    #endregion Callbacks

    /// <summary>
    /// Description: Updates text elements in UI.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    void UpdateElements()
    {
        actionName.SetText(inputRef.NameForInput);
        inputKey.SetText(inputRef.KeyToListen.ToString());
    }

    /// <summary>
    /// Description: Listen for new key to bind to listener.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void ListenForNewKey()
    {
        listening = true;
    }

    /// <summary>
    /// Description: Rebind key.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void Update()
    {
        if (listening)
        {
            foreach (KeyCode n in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(n))
                {
                    inputRef.SetKey(n);
                    inputKey.SetText(inputRef.KeyToListen.ToString());
                    listening = false;
                }
            }
        }
    }
}
