using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechPrompt : MonoBehaviour
{
    Transform dialogueTransform;
    GameObject image;
    Animator animator;
    bool inRange;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        image = this.transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        CallbackHandler.instance.speechInRange += InRange;
        CallbackHandler.instance.speechOutOfRange += OutOfRange;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.speechInRange -= InRange;
        CallbackHandler.instance.speechOutOfRange -= OutOfRange;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Show", inRange);

        if (!dialogueTransform)
            return;

        this.transform.position = Camera.main.WorldToScreenPoint(dialogueTransform.position);
        image.SetActive(this.transform.position.z > 0);
    }

    public void InRange(Transform _dialogue)
    {
        dialogueTransform = _dialogue;
        inRange = true;
    }

    public void OutOfRange()
    {
        inRange = false;
    }

    public void Hide()
    {
        dialogueTransform = null;
    }
}
