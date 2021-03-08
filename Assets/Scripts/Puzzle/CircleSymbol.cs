using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Symbol
{
    White = 0,
    Black,
    Gray,
    NotAssigned
}

public class CircleSymbol : MonoBehaviour
{
    public Symbol symbol = Symbol.NotAssigned;

    public void Setup(Symbol _symbol)
    {
        MeshRenderer temp = GetComponent<MeshRenderer>();
        symbol = _symbol;

        switch(_symbol)
        {
            case Symbol.White:
            {
                temp.material.SetColor("_BaseColor", new Color(1.0f, 1.0f, 0.6f, 1.0f));
                temp.material.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 0.6f, 1.0f));
                break;
            }
            case Symbol.Gray:
            {
                temp.material.SetColor("_BaseColor", new Color(0, 0.5f, 0, 0.0f));
                temp.material.SetColor("_EmissionColor", new Color(0, 0.5f, 0, 0.0f));
                    break;
            }
            case Symbol.Black:
            {
                temp.material.SetColor("_BaseColor", new Color(0, 0, 0.1f, 1.0f));
                temp.material.SetColor("_EmissionColor", new Color(0, 0, 0.1f, 1.0f));
                break;
            }
        }

    }
}
