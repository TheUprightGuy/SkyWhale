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
    public List<GameObject> whaleTutorials;
    public GameObject gliderTutorial;
    private int whaleTutorialIndex = 0;
    private float timer = 1.0f;
    private float maxTimer = 1.0f;

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

        VirtualInputs.GetInputListener(InputType.WHALE, "Thrust").MethodToCall.AddListener(ProgressTutorial);

        EventManager.StartListening("WhaleTutorial", StartWhaleTutorial);
        EventManager.StartListening("GliderTutorial", ShowGliderTutorial);
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

    private void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
    }

    private void ProgressTutorial(InputState arg0)
    {
        if (timer > 0f) return;
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
        //Also enable speed boost rings
        EntityManager.instance.SpeedBoostRingContainer.SetActive(true);
        whaleTutorials[whaleTutorialIndex].SetActive(true);
        whaleTutorialIndex++;
        EventManager.StopListening("WhaleTutorial", StartWhaleTutorial);
        timer = maxTimer;
    }

    public void ShowGliderTutorial()
    {
        gliderTutorial.SetActive(true);
        EventManager.StopListening("GliderTutorial", ShowGliderTutorial);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Jump").MethodToCall.AddListener(HideGliderTutorial);
    }

    private void HideGliderTutorial(InputState arg0)
    {
        gliderTutorial.SetActive(false);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Jump").MethodToCall.RemoveListener(HideGliderTutorial);
        CallbackHandler.instance.MovePilotNPCs();
    }
}
