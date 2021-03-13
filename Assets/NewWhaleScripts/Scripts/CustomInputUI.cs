using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomInputUI : MonoBehaviour
{
    public GameObject inputUIPrefab;
    public void Start()
    {
        InputUIPrefab playerControls = Instantiate(inputUIPrefab, transform).GetComponent<InputUIPrefab>();
        playerControls.CreateUIElements("PLAYER", VirtualInputs.instance.currentInput.playerInput);
        playerControls.CreateUIElements("WHALE", VirtualInputs.instance.currentInput.whaleInput);
    }
}
