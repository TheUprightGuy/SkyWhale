using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueScript : MonoBehaviour
{
    #region Setup
    TMPro.TextMeshProUGUI speaker;
    TMPro.TextMeshProUGUI dialogue;
    float timer;
    bool active;
    KeyCode skipKey;
    Animator animator;
    private void Awake()
    {
        speaker = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        dialogue = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        CallbackHandler.instance.setDialogue += SetDialogue;
        CallbackHandler.instance.setTutorialMessage += SetTutorialMessage;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.setDialogue -= SetDialogue;
        CallbackHandler.instance.setTutorialMessage -= SetTutorialMessage;
    }
    #endregion Setup
    string text;
    string[] dialogueList;


    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && active)
        {
            NextLine();
        }

        if (Input.GetKeyDown(skipKey) || Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    public void SetDialogue(string _speaker, string _text)
    {
        text = _text;
        speaker.SetText(_speaker);
        dialogue.SetText("");
        timer = Mathf.Infinity;
        skipKey = KeyCode.E;

        // Weird that this is a thing
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(WriteDialogue());
            active = true;
        }
    }

    public void SetTutorialMessage(TutorialMessage _msg)
    {
        _msg.message = _msg.message.Replace("\\n", "\n");
        text = _msg.message;
        speaker.SetText("Tutorial");
        dialogue.SetText(" ");
        dialogue.text = "";
        timer = _msg.timeout;
        skipKey = _msg.key;

        // Weird that this is a thing
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(WriteDialogue());
            active = true;
        }
    }

    IEnumerator WriteDialogue()
    {
        animator.SetTrigger("PopIn");
        dialogue.SetText(" ");
        dialogue.text = "";

        foreach (char n in text)
        {
            dialogue.text += n;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void NextLine()
    {
        animator.SetTrigger("PopOut");
        active = false;
        Invoke("Next", 0.35f);
        // move to next line in dialogueList
        // if not close 
    }
    
    public void Next()
    {
        CallbackHandler.instance.ToggleText();
        CallbackHandler.instance.NextMessage();
    }


    private void OnMouseDown()
    {
        NextLine();
    }
}
