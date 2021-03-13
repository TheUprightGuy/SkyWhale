using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewDialogueManager : MonoBehaviour
{
    #region Setup
    TMPro.TextMeshProUGUI speaker;
    TMPro.TextMeshProUGUI dialogue;
    Animator animator;
    Image leftCharacter;
    Image rightCharacter;
    private void Awake()
    {
        leftCharacter = transform.GetChild(0).GetComponent<Image>();
        rightCharacter = transform.GetChild(1).GetComponent<Image>();
        speaker = transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>();
        dialogue = transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>();
        animator = GetComponent<Animator>();
    }
    #endregion Setup

    public Dialogue testDialogue;

    public Dialogue currentDialogue;
    public float dialogueTime;
    float timer;
    new bool enabled;

    private void Start()
    {
        timer = dialogueTime;
        if (currentDialogue)
        {
            currentDialogue.StartUp();
        }
        ShowDialogue();
    }

    private void Update()
    {
        if (enabled)
        {
            timer -= Time.deltaTime;
            if (timer <= 0 || Input.GetKeyDown(KeyCode.F))
            {
                ProgressDialogue();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SetDialogue(testDialogue);
        }
    }

    public void SetDialogue(Dialogue _dialogue)
    {
        enabled = true;
        _dialogue.StartUp();

        currentDialogue = _dialogue;
        leftCharacter.sprite = _dialogue.leftCharacter;
        rightCharacter.sprite = _dialogue.rightCharacter;
        
        ShowDialogue();
    }

    public DialogueComponent GetDialogue()
    {
        return currentDialogue.GetDialogue();
    }

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

        StartCoroutine(WriteDialogue(temp.dialogue[temp.dialogueIndex]));        
    }

    public void ProgressDialogue()
    {
        timer = dialogueTime;

        if (currentDialogue.Progress() != null)
        {
            ShowDialogue();
        }
        else
        {
            HideText();
        }
    }

    public void HideText()
    {
        enabled = false;
        animator.ResetTrigger("PopIn");
        animator.SetTrigger("PopOut");
        dialogue.SetText(" ");
        dialogue.text = "";
    }

    IEnumerator WriteDialogue(string _text)
    {
        animator.ResetTrigger("PopOut");
        animator.SetTrigger("PopIn");
        dialogue.SetText(" ");
        dialogue.text = "";

        foreach (char n in _text)
        {
            dialogue.text += n;
            yield return new WaitForSeconds(0.03f);
        }
    }
}
