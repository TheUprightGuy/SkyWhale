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
        
        CallbackHandler.instance.checkPointInRange += DisplayCheckPointPrompt;
        CallbackHandler.instance.checkPointOutOfRange += HideCheckPointPrompt;

        CallbackHandler.instance.fadeIn += FadeIn;
        CallbackHandler.instance.fadeOut += FadeOut;

        VirtualInputs.GetInputListener(InputType.WHALE, "Thrust").MethodToCall.AddListener(ProgressTutorial);

        EventManager.StartListening("WhaleTutorial", StartWhaleTutorial);
        EventManager.StartListening("GliderTutorial", ShowGliderTutorial);

        SetupTutorialHotkeys();
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.displayPrompt -= DisplayButtonPrompt;
        CallbackHandler.instance.hidePrompt -= HideButtonPrompt;

        CallbackHandler.instance.speechInRange -= DisplaySpeechUIElement;
        CallbackHandler.instance.speechOutOfRange -= HideSpeechUIElement;

        CallbackHandler.instance.puzzleInRange -= DisplayPuzzlePrompt;
        CallbackHandler.instance.puzzleOutOfRange -= HidePuzzlePrompt;

        CallbackHandler.instance.checkPointInRange -= DisplayCheckPointPrompt;
        CallbackHandler.instance.checkPointOutOfRange -= HideCheckPointPrompt;
        
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
    
    public void DisplayCheckPointPrompt(Transform _position)
    {
        //.InRange(_position);
        DisplayButtonPrompt(PromptType.CheckPoint);
    }
    
    public void HideCheckPointPrompt()
    {
        //checkPointPrompt.OutOfRange();
        HideButtonPrompt(PromptType.CheckPoint);
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

    /// TUTORIAL TEXT
    [Header("Tutorials")]
    // GRAPPLE TUTORIAL
    public List<TMPro.TextMeshProUGUI> grappleTuts;
    // WHALE TUTORIAL
    public List<TMPro.TextMeshProUGUI> whaleTuts;
    // GLIDER TUTORIAL
    public List<TMPro.TextMeshProUGUI> glideTuts;

    public void SetupTutorialHotkeys()
    {
        // Grapple
        grappleTuts[1].SetText(CheckKey(VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").KeyToListen) + " TO MANUALLY RELEASE GRAPPLE.");
        grappleTuts[0].SetText("PRESS " + CheckKey(VirtualInputs.GetInputListener(InputType.PLAYER, "Jump").KeyToListen) + " TO JUMP FROM GRAPPLE.");

        // Whale
        whaleTuts[0].SetText(CheckKey(VirtualInputs.GetInputListener(InputType.WHALE, "PitchDown").KeyToListen) + CheckKey(VirtualInputs.GetInputListener(InputType.WHALE, "YawLeft").KeyToListen) 
            + CheckKey(VirtualInputs.GetInputListener(InputType.WHALE, "PitchUp").KeyToListen) + CheckKey(VirtualInputs.GetInputListener(InputType.WHALE, "YawRight").KeyToListen) + " TO STEER WHALE.\n"
            + "PRESS " + CheckKey(VirtualInputs.GetInputListener(InputType.WHALE, "Thrust").KeyToListen) + " TO MOVE FORWARDS.\n"
            + "FLY THROUGH RINGS TO GAIN A SPEED BOOST.");
        whaleTuts[1].SetText("PRESS " + CheckKey(VirtualInputs.GetInputListener(InputType.WHALE, "Dismount").KeyToListen) + " TO DISMOUNT FROM THE WHALE.\n"
            + "THE WHALE WILL SAVE YOU IF YOU FALL.");

        // Glider
        glideTuts[0].SetText("PRESS " + CheckKey(VirtualInputs.GetInputListener(InputType.PLAYER, "Glide").KeyToListen) + " WHILE FALLING TO OPEN THE GLIDER.\n"
            + "GAIN SPEED BY PITCHING DOWNWARDS.");
    }

    string CheckKey(KeyCode _key)
    {
        string temp;

        if (_key == KeyCode.Mouse0)
        {
            temp = "LMB";
        }
        else if (_key == KeyCode.Mouse1)
        {
            temp = "RMB";
        }
        else
        {
            temp = _key.ToString();
        }

        return temp;
    }

}
