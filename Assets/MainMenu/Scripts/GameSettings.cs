using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Data/Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Game Settings")]
    //public Difficulty difficulty;
    // public Language language;
    public int mouseSensitivity;
    [Header("Audio Settings")]
    public int masterVolume;
    public int soundVolume;
    public int musicVolume;
    [Header("Video Settings")]
    public Resolution resolution;
    public bool fullScreen;
    public int fieldOfView;
    //[Header("Control Settings")]
    // public Keybinds keybinds
}
