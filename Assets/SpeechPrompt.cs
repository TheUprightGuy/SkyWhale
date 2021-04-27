using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechPrompt : MonoBehaviour
{
    TMPro.TextMeshProUGUI text;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        Invoke("SetText", 1.0f);
    }

    private void SetText()
    {
        text.SetText(VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").KeyToListen.ToString());
    }

    public void Show()
    {
        animator.SetBool("Show", true);
    }

    public void Hide()
    {
        animator.SetBool("Show", false);
    }
}
