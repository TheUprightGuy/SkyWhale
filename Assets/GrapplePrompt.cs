using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePrompt : MonoBehaviour
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
        string temp;
        KeyCode key = VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleAim").KeyToListen;
        if (key == KeyCode.Mouse0 || key == KeyCode.Mouse1)
        {
            temp = key == KeyCode.Mouse0 ? "LMB" : "RMB";
        }
        else
        {
            temp = VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleAim").KeyToListen.ToString();
        }

        text.SetText(temp);
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
