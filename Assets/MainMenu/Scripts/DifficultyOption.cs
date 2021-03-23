using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    EASY = 0,
    NORMAL,
    HARD
}

public class DifficultyOption : MonoBehaviour
{
    public Difficulty difficulty;
    public TMPro.TextMeshProUGUI text;
    private GameSettings settings;

    private void Start()
    {
        settings = PauseMenuCanvasController.instance.gameSettings;
        SetSettings();

        PauseMenuCanvasController.instance.setSettings += SetSettings;
    }

    private void OnDestroy()
    {
        PauseMenuCanvasController.instance.setSettings -= SetSettings;
    }

    public void SetSettings()
    {
        //difficulty = settings.difficulty;
        UpdateText();
    }

    public void IncreaseDifficulty()
    {
        if ((int)difficulty < 2)
        {
            difficulty++; 
        }
        UpdateText();
        //settings.difficulty = difficulty;
    }

    public void LowerDifficulty()
    {
        if ((int)difficulty > 0)
        {
            difficulty--;
        }
        UpdateText();
        //settings.difficulty = difficulty;
    }

    public void UpdateText()
    {
        text.SetText(difficulty.ToString());
    }
}
