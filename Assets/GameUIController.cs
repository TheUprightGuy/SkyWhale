using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    #region Setup
    ButtonPrompt buttonPrompt;
    PuzzlePrompt puzzlePrompt;
    GlidePrompt glidePrompt;
    GrapplePrompt grapplePrompt;
    SpeechPrompt speechPrompt;

    SpeechUIElement speechUIElement;

    private void Awake()
    {
        buttonPrompt = GetComponentInChildren<ButtonPrompt>();
        puzzlePrompt = GetComponentInChildren<PuzzlePrompt>();
        glidePrompt = GetComponentInChildren<GlidePrompt>();
        grapplePrompt = GetComponentInChildren<GrapplePrompt>();
        speechPrompt = GetComponentInChildren<SpeechPrompt>();

        speechUIElement = GetComponentInChildren<SpeechUIElement>();
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
        CallbackHandler.instance.speechInRange += DisplaySpeechUIElement;
        CallbackHandler.instance.speechOutOfRange += HideSpeechUIElement;

        CallbackHandler.instance.puzzleInRange += DisplayPuzzlePrompt;
        CallbackHandler.instance.puzzleOutOfRange += HidePuzzlePrompt;

        CallbackHandler.instance.displayHotkey += DisplayButtonPrompt;
        CallbackHandler.instance.hideHotkey += HideButtonPrompt;

        CallbackHandler.instance.showGlide += DisplayGlidePrompt;
        CallbackHandler.instance.hideGlide += HideGlidePrompt;

        CallbackHandler.instance.showGrapple += DisplayGrapplePrompt;
        CallbackHandler.instance.hideGrapple += HideGrapplePrompt;

        CallbackHandler.instance.showSpeech += DisplaySpeechPrompt;
        CallbackHandler.instance.hideSpeech += HideSpeechPrompt;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.speechInRange -= DisplaySpeechUIElement;
        CallbackHandler.instance.speechOutOfRange -= HideSpeechUIElement;

        CallbackHandler.instance.puzzleInRange -= DisplayPuzzlePrompt;
        CallbackHandler.instance.puzzleOutOfRange -= HidePuzzlePrompt;

        CallbackHandler.instance.displayHotkey -= DisplayButtonPrompt;
        CallbackHandler.instance.hideHotkey -= HideButtonPrompt;

        CallbackHandler.instance.showGlide -= DisplayGlidePrompt;
        CallbackHandler.instance.hideGlide -= HideGlidePrompt;

        CallbackHandler.instance.showSpeech -= DisplaySpeechPrompt;
        CallbackHandler.instance.hideSpeech -= HideSpeechPrompt;
    }
    #endregion Callbacks

    public void DisplayButtonPrompt(InputType _type, string _action, string _subName)
    {
        if (VirtualInputs.GetInputListener(_type, _action) != null)
            buttonPrompt.Show(true, VirtualInputs.GetInputListener(_type, _action).KeyToListen, _action, _subName);
    }

    public void HideButtonPrompt(string _action)
    {
        buttonPrompt.Hide(_action);
    }

    public void DisplaySpeechUIElement(Transform _position)
    {
        speechUIElement.InRange(_position);
        //buttonPrompt.Show(true, VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").KeyToListen, "Interact", "");
    }

    public void HideSpeechUIElement()
    {
        speechUIElement.OutOfRange();
        buttonPrompt.Hide("Interact");
    }

    public void DisplayPuzzlePrompt(Transform _position)
    {
        puzzlePrompt.InRange(_position);
        //buttonPrompt.Show(true, VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").KeyToListen, "Interact", "");
    }

    public void HidePuzzlePrompt()
    {
        puzzlePrompt.OutOfRange();
        buttonPrompt.Hide("Interact");
    }

    public void DisplayGlidePrompt()
    {
        glidePrompt.Show();
    }

    public void HideGlidePrompt()
    {
        glidePrompt.Hide();
    }

    public void DisplayGrapplePrompt()
    {
        grapplePrompt.Show();
    }

    public void HideGrapplePrompt()
    {
        grapplePrompt.Hide();
    }

    public void DisplaySpeechPrompt()
    {
        speechPrompt.Show();
    }

    public void HideSpeechPrompt()
    {
        speechPrompt.Hide();
    }
}
