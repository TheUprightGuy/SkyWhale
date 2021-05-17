using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    #region Setup

    PuzzlePrompt puzzlePrompt;
    SpeechUIElement speechUIElement;
    Fader fader;
    public ButtonPrompt[] prompts;

    private void Awake()
    {
        speechUIElement = GetComponentInChildren<SpeechUIElement>();
        puzzlePrompt = GetComponentInChildren<PuzzlePrompt>();

        prompts = GetComponentsInChildren<ButtonPrompt>();
        fader = GetComponentInChildren<Fader>();
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

        CallbackHandler.instance.fadeIn += FadeIn;
        CallbackHandler.instance.fadeOut += FadeOut;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.displayPrompt -= DisplayButtonPrompt;
        CallbackHandler.instance.hidePrompt -= HideButtonPrompt;

        CallbackHandler.instance.speechInRange -= DisplaySpeechUIElement;
        CallbackHandler.instance.speechOutOfRange -= HideSpeechUIElement;

        CallbackHandler.instance.puzzleInRange -= DisplayPuzzlePrompt;
        CallbackHandler.instance.puzzleOutOfRange -= HidePuzzlePrompt;

        CallbackHandler.instance.fadeIn -= FadeIn;
        CallbackHandler.instance.fadeOut -= FadeOut;
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

    public void FadeIn()
    {
        fader.FadeIn();
    }

    public void FadeOut()
    {
        fader.FadeOut();
    }
}
