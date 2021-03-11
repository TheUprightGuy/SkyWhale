using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Input", menuName = "Data/Input", order = 1)]
public class InputData : ScriptableObject
{
    public List<InputListener> playerInput;
    public List<InputListener> whaleInput;

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
        }
    }

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
    }
}
