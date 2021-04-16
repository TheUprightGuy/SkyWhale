/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   DialogueManager.cs
  Description :   Handles dialogue SOs and UI. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Setup
    TMPro.TextMeshProUGUI speaker;
    TMPro.TextMeshProUGUI dialogue;
    Animator animator;
    Image leftCharacter;
    Image rightCharacter;

    /// <summary>
    /// Description: Setup local Components.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        leftCharacter = transform.GetChild(0).GetComponent<Image>();
        rightCharacter = transform.GetChild(1).GetComponent<Image>();
        speaker = transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>();
        dialogue = transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>();
        animator = GetComponent<Animator>();
    }
    #endregion Setup
    #region Callbacks
    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Start()
    {
        timer = dialogueTime;
        if (currentDialogue)
        {
            currentDialogue.StartUp();
        }
        ShowDialogue();

        CallbackHandler.instance.setDialogue += SetDialogue;
        CallbackHandler.instance.stopDialogue += StopDialogue;
        VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").MethodToCall.AddListener(ProgressDialogue);
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.setDialogue -= SetDialogue;
        CallbackHandler.instance.stopDialogue -= StopDialogue;
    }
    #endregion Callbacks
    
    public Dialogue currentDialogue;
    public float dialogueTime;
    float timer;
    new bool enabled;
    // Prevents Double Input of Interact Key
    float startTimer;

    private Coroutine myCoroutine;

    /// <summary>
    /// Description: Progresses dialogue over time or on E press.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Update()
    {
        if (enabled)
        {
            timer -= Time.deltaTime;
            startTimer -= Time.deltaTime;
            if (timer <= 0)            
                ProgressDialogue(InputState.KEYDOWN);          
        }
    }

    /// <summary>
    /// Description: Sets dialogue SO to use and begins dialogue.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_dialogue">Dialogue SO</param>
    public void SetDialogue(Dialogue _dialogue)
    {
        enabled = true;
        _dialogue.StartUp();

        currentDialogue = _dialogue;
        leftCharacter.sprite = _dialogue.leftCharacter;
        rightCharacter.sprite = _dialogue.rightCharacter;

        startTimer = 0.1f;
        
        ShowDialogue();
    }

    /// <summary>
    /// Description: Gets reference to current dialogue.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <returns>Reference to current dialogue</returns>
    public DialogueComponent GetDialogue()
    {
        return currentDialogue.GetDialogue();
    }

    /// <summary>
    /// Description: Displays next line of dialogue and updates image.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void ShowDialogue()
    {
        // No More Dialogue
        if (!currentDialogue)
        {
            HideText();
            return;
        }

        DialogueComponent temp = currentDialogue.GetDialogue();

        if (temp.left)
        {
            leftCharacter.sprite = temp.image;
            leftCharacter.color = Color.white;
            rightCharacter.color = Color.grey;
        }
        else
        {
            rightCharacter.sprite = temp.image;
            rightCharacter.color = Color.white;
            leftCharacter.color = Color.grey;
        }

        speaker.SetText(temp.name);

        myCoroutine = StartCoroutine(WriteDialogue(temp.dialogue[temp.dialogueIndex]));
    }

    /// <summary>
    /// Description: Checks if dialogue can be progressed.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void ProgressDialogue(InputState type)
    {
        if (type != InputState.KEYDOWN || !currentDialogue || startTimer > 0)
            return;

        timer = dialogueTime;

        if (typing)
        {
            StopWriting();
        }
        
        if (currentDialogue.Progress() != null)
        {
            ShowDialogue();
        }
        else
        {
            StopDialogue();
            StopCoroutine(myCoroutine);
            currentDialogue = null;
        }
    }

    void StopWriting()
    {
        dialogue.SetText("");
        dialogue.text = "";
        StopCoroutine(myCoroutine);
    }

    /// <summary>
    /// Description: Hides dialogue.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void HideText()
    {
        enabled = false;
        animator.ResetTrigger("PopIn");
        animator.SetTrigger("PopOut");
        dialogue.SetText(" ");
        dialogue.text = "";
    }

    /// <summary>
    /// Description: Reset dialogue upon completion.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void ResetDialogue()
    {
        if (currentDialogue)
        {
            cachedDialogue = currentDialogue;
            Invoke("RestartDialogue", 1.0f);
        }
            //currentDialogue.Restart();
    }

    Dialogue cachedDialogue;
    void RestartDialogue()
    {
        cachedDialogue.StartUp();
        cachedDialogue = null;
    }

    /// <summary>
    /// Description: Sets dialogue as inUse for checks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void SetInUse()
    {
        if (currentDialogue)
            currentDialogue.inUse = true;
    }

    /// <summary>
    /// Description: Ends current dialogue.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void StopDialogue()
    {
        ResetDialogue();
        HideText();
    }

    bool typing;

    /// Description: Coroutine to type out text.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    IEnumerator WriteDialogue(string _text)
    {
        typing = true;
        animator.ResetTrigger("PopOut");
        animator.SetTrigger("PopIn");
        dialogue.SetText(" ");
        dialogue.text = "";

        foreach (char n in _text)
        {
            dialogue.text += n;
            yield return new WaitForSeconds(0.03f);
        }
        typing = false;
    }
}
