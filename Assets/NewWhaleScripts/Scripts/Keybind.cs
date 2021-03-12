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

    public void Setup(InputListener _input)
    {
        inputRef = _input;
        UpdateElements();
    }

    private void Start()
    {
        VirtualInputs.instance.resetToDefaults += UpdateElements;
    }

    void UpdateElements()
    {
        actionName.SetText(inputRef.NameForInput);
        inputKey.SetText(inputRef.KeyToListen.ToString());
    }

    public void ListenForNewKey()
    {
        listening = true;
    }

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
