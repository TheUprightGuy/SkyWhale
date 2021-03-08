using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameState",menuName = "GameSpecific/GameState")]
public class GameState : ScriptableObject
{
    public bool inMenu;
    public bool gamePaused;
    public bool inCinematic = false;
    public bool playerOnIsland;
    public List<int> objectivesHighlighted = new List<int>();
}
