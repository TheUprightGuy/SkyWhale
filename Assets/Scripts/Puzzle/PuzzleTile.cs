﻿using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
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
        AudioManager.instance.PlaySound(gameObject.GetInstanceID().ToString());
        //Check if correct type
        if (puzzleGenerator.CheckIfCorrectType(puzzleTileSo.type))
        {
            puzzleTileSo.triggered = true;
        }
        else
        {
            puzzleGenerator.ResetPuzzle(other.gameObject);
        }
    }
}
