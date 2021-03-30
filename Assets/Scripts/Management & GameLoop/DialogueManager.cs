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
    private void Awake()
    {
        leftCharacter = transform.GetChild(0).GetComponent<Image>();
        rightCharacter = transform.GetChild(1).GetComponent<Image>();
        speaker = transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>();
        dialogue = transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>();
        animator = GetComponent<Animator>();
    }
    #endregion Setup
    
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


        CallbackHandler.instance.setDialogue += SetDialogue;
        CallbackHandler.instance.stopDialogue += StopDialogue;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.setDialogue -= SetDialogue;
        CallbackHandler.instance.stopDialogue -= StopDialogue;
    }

    private void Update()
    {
        if (enabled)
        {
            timer -= Time.deltaTime;
            if (timer <= 0 || Input.GetKeyDown(KeyCode.E))
            {
                ProgressDialogue();
            }
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

        if (typing)
            return;
        
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

    public void ResetDialogue()
    {
        if (currentDialogue)
            currentDialogue.StartUp();
    }
    public void SetInUse()
    {
        if (currentDialogue)
            currentDialogue.inUse = true;
    }

    public void StopDialogue()
    {
        ResetDialogue();
        HideText();
    }

    bool typing;

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
