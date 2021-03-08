using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTileSO : ScriptableObject
{
    public void Initialise(int _type, int _col, int _row)
    {
        type = _type;
        col = _col;
        row = _row;
        triggered = false;
    }

    public int type;
    public int col;
    public int row;
    public bool triggered;
}
