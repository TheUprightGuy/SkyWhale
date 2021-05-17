using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    #region Setup

    PuzzlePrompt puzzlePrompt;
    SpeechUIElement speechUIElement;
    public ButtonPrompt[] prompts;
    public List<GameObject> whaleTutorials;
    private int whaleTutorialIndex = 0;
    private float timer = 2.5f;
    private float maxTimer = 2.5f;

    private void Awake()
    {
        speechUIElement = GetComponentInChildren<SpeechUIElement>();
        puzzlePrompt = GetComponentInChildren<PuzzlePrompt>();

        prompts = GetComponentsInChildren<ButtonPrompt>();
    }
    #endregion Setup

    #region Callbacks
    /// <summary>
    /// Description: Setup callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    private void Start()
    {
        CallbackHandler.instance.displayPrompt += DisplayButtonPrompt;
        CallbackHandler.instance.hidePrompt += HideButtonPrompt;


        CallbackHandler.instance.speechInRange += DisplaySpeechUIElement;
        CallbackHandler.instance.speechOutOfRange += HideSpeechUIElement;

        CallbackHandler.instance.puzzleInRange += DisplayPuzzlePrompt;
        CallbackHandler.instance.puzzleOutOfRange += HidePuzzlePrompt;
        
        VirtualInputs.GetInputListener(InputType.WHALE, "Thrust").MethodToCall.AddListener(ProgressTutorial);
        
        EventManager.StartListening("WhaleTutorial", StartWhaleTutorial);
    }

    private void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
    }

    private void ProgressTutorial(InputState arg0)
    {
        if(timer > 0f) return;
        if (whaleTutorialIndex == 0) return;
        whaleTutorials[whaleTutorialIndex - 1].SetActive(false);
        if (whaleTutorialIndex == whaleTutorials.Count)
        {
            VirtualInputs.GetInputListener(InputType.WHALE, "Thrust").MethodToCall.RemoveListener(ProgressTutorial);
            return;
        }
        whaleTutorials[whaleTutorialIndex].SetActive(true);
        whaleTutorialIndex++;
        timer = maxTimer;
    }

    private void StartWhaleTutorial()
    {
        whaleTutorials[whaleTutorialIndex].SetActive(true);
        whaleTutorialIndex++;
        EventManager.StopListening("WhaleTutorial", StartWhaleTutorial);
        timer = maxTimer;
    }

    private void OnDestroy()
    {

        CallbackHandler.instance.displayPrompt -= DisplayButtonPrompt;
        CallbackHandler.instance.hidePrompt -= HideButtonPrompt;

        CallbackHandler.instance.speechInRange -= DisplaySpeechUIElement;
        CallbackHandler.instance.speechOutOfRange -= HideSpeechUIElement;

        CallbackHandler.instance.puzzleInRange -= DisplayPuzzlePrompt;
        CallbackHandler.instance.puzzleOutOfRange -= HidePuzzlePrompt;
    }
    #endregion Callbacks


    public void DisplayButtonPrompt(PromptType _type)
    {
        foreach (ButtonPrompt n in prompts)
        {
            n.Display(_type);
        }
    }

    public void HideButtonPrompt(PromptType _type)
    {
        foreach(ButtonPrompt n in prompts)
        {
            n.Hide(_type);
        }
    }

    public void DisplaySpeechUIElement(Transform _position)
    {
        speechUIElement.InRange(_position);
        DisplayButtonPrompt(PromptType.Speech);
    }

    public void HideSpeechUIElement()
    {
        speechUIElement.OutOfRange();
        HideButtonPrompt(PromptType.Speech);
    }

    public void DisplayPuzzlePrompt(Transform _position)
    {
        puzzlePrompt.InRange(_position);
        DisplayButtonPrompt(PromptType.Interact);
    }

    public void HidePuzzlePrompt()
    {
        puzzlePrompt.OutOfRange();
        HideButtonPrompt(PromptType.Interact);
    }
}
