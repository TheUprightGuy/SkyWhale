using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Data/Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Game Settings")]
    //public Difficulty difficulty;
    // public Language language;
    [Header("Audio Settings")]
    public int masterVolume;
    public int soundVolume;
    public int musicVolume;
    public bool subtitles;
    [Header("Video Settings")]
    public Resolution resolution;
    public bool fullScreen;
    //[Header("Control Settings")]
    // public Keybinds keybinds
}
