using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle;
using UnityEngine;

public class PuzzleTile : MonoBehaviour
{
    public PuzzleTileSO puzzleTileSo;
    public PuzzleGenerator puzzleGenerator;

    private void OnTriggerEnter(Collider other)
    {
        if (puzzleTileSo.triggered || puzzleGenerator.disabled)
            return;
        //Check if correct type
        if (puzzleGenerator.CheckIfCorrectType(puzzleTileSo.type))
        {
            Debug.Log(gameObject.name + " triggered");
            puzzleTileSo.triggered = true;
        }
        else
        {
            puzzleGenerator.ResetPuzzle(other.gameObject);
        }
    }
}
