using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InputState
{
    KEYDOWN,
    KEYHELD,
    KEYUP
}

public enum InputType
{
    PLAYER,
    WHALE,
    MENU
}

[System.Serializable]
public class InputListener
{
    public string NameForInput;
    public KeyCode KeyToListen;
    public bool CallOnKeyDown = true;
    public bool CallOnKeyHeld = true;
    public bool CallOnKeyUp = true;
    /// <summary>
    /// MethodToCall must have parameter of type InputState
    /// </summary>
    [HideInInspector]
    public InputEvent MethodToCall;

    public void SetKey(KeyCode _key)
    {
        KeyToListen = _key;
    }
}

[System.Serializable]
public class InputEvent : UnityEvent<InputState> {}

public class VirtualInputs : MonoBehaviour
{

    private static VirtualInputs virtualInputs;
    public static VirtualInputs instance
    {
        get
        {
            if (!virtualInputs)
            {
                virtualInputs = FindObjectOfType(typeof(VirtualInputs)) as VirtualInputs;
                if (!virtualInputs)
                {
                    //Debug.LogError("Input Manager is missing!");
                }
            }
            return virtualInputs;
        }
    }

    [Header("Saved Inputs")]
    public InputData currentInput;
    public InputData defaultInput;

    // Update is called once per frame
    void Update()
    {
        foreach (InputListener IJ in instance.currentInput.playerInput)
        {
            if (IJ.NameForInput == "") //Leave name blank to stop listener
            {
                continue;
            }

            if (Input.GetKeyDown(IJ.KeyToListen) && IJ.CallOnKeyDown)
            {
                IJ.MethodToCall.Invoke(InputState.KEYDOWN);
            }
            else if (Input.GetKey(IJ.KeyToListen) && IJ.CallOnKeyHeld)
            {
                IJ.MethodToCall.Invoke(InputState.KEYHELD);
            }
            else if (Input.GetKeyUp(IJ.KeyToListen) && IJ.CallOnKeyUp)
            {
                IJ.MethodToCall.Invoke(InputState.KEYUP);
            }
        }

        foreach (InputListener IJ in instance.currentInput.whaleInput)
        {
            if (IJ.NameForInput == "") //Leave name blank to stop listener
            {
                continue;
            }

            if (Input.GetKeyDown(IJ.KeyToListen) && IJ.CallOnKeyDown)
            {
                IJ.MethodToCall.Invoke(InputState.KEYDOWN);
            }
            else if (Input.GetKey(IJ.KeyToListen) && IJ.CallOnKeyHeld)
            {
                IJ.MethodToCall.Invoke(InputState.KEYHELD);
            }
            else if (Input.GetKeyUp(IJ.KeyToListen) && IJ.CallOnKeyUp)
            {
                IJ.MethodToCall.Invoke(InputState.KEYUP);
            }
        }

        foreach (InputListener IJ in instance.currentInput.menuInput)
        {
            if (IJ.NameForInput == "") //Leave name blank to stop listener
            {
                continue;
            }

            if (Input.GetKeyDown(IJ.KeyToListen) && IJ.CallOnKeyDown)
            {
                IJ.MethodToCall.Invoke(InputState.KEYDOWN);
            }
            else if (Input.GetKey(IJ.KeyToListen) && IJ.CallOnKeyHeld)
            {
                IJ.MethodToCall.Invoke(InputState.KEYHELD);
            }
            else if (Input.GetKeyUp(IJ.KeyToListen) && IJ.CallOnKeyUp)
            {
                IJ.MethodToCall.Invoke(InputState.KEYUP);
            }
        }
    }

    public static InputListener GetInputListener(InputType _type, string _ILName)
    {
        switch(_type)
        {
            case InputType.PLAYER:
            {
                for (int i = 0; i < instance.currentInput.playerInput.Count; i++)
                {
                    if (instance.currentInput.playerInput[i].NameForInput == _ILName)
                    {
                        return instance.currentInput.playerInput[i];
                    }
                }
                break;
            }
            case InputType.WHALE:
            {
                for (int i = 0; i < instance.currentInput.whaleInput.Count; i++)
                {
                    if (instance.currentInput.whaleInput[i].NameForInput == _ILName)
                    {
                        return instance.currentInput.whaleInput[i];
                    }
                }
                break;
            }
            case InputType.MENU:
            {
                for (int i = 0; i < instance.currentInput.menuInput.Count; i++)
                {
                    if (instance.currentInput.menuInput[i].NameForInput == _ILName)
                    {
                        return instance.currentInput.menuInput[i];
                    }
                }
                break;
            }
        }

        Debug.LogWarning(_ILName + " does not exist.");
        return null;
    }

    public static void SetKey(InputType _type, string _ILName, KeyCode _key)
    {
        switch (_type)
        {
            case InputType.PLAYER:
            {
                for (int i = 0; i < instance.currentInput.playerInput.Count; i++)
                {
                    if (instance.currentInput.playerInput[i].NameForInput == _ILName)
                    {
                        instance.currentInput.playerInput[i].KeyToListen = _key;
                    }
                }
                break;
            }
            case InputType.WHALE:
            {
                for (int i = 0; i < instance.currentInput.whaleInput.Count; i++)
                {
                    if (instance.currentInput.whaleInput[i].NameForInput == _ILName)
                    {
                        instance.currentInput.whaleInput[i].KeyToListen = _key;
                    }
                }
                break;
            }
            case InputType.MENU:
            {
                for (int i = 0; i < instance.currentInput.menuInput.Count; i++)
                {
                    if (instance.currentInput.menuInput[i].NameForInput == _ILName)
                    {
                        instance.currentInput.menuInput[i].KeyToListen = _key;
                    }
                }
                break;
            }
        }
    }

    public event Action resetToDefaults;
    public static void ResetToDefaults()
    {
        instance.currentInput.CopyInputs(instance.defaultInput);
        if (instance.resetToDefaults != null)
        {
            instance.resetToDefaults();
        }
    }
}
