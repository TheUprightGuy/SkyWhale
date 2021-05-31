using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public PuzzleGenerator puzzleGenerator;

    private void OnTriggerEnter(Collider other)
    {
        //puzzleGenerator.FoundCollectable();
        Destroy(gameObject);
    }
}
