using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum PromptType
{
    Interact,
    Speech,
    Climb,
    GrappleAim,
    GrappleFire,
    Glide,
}

public class ButtonPrompt : MonoBehaviour
{
    Animator animator;
    TMPro.TextMeshProUGUI text;

    public PromptType type;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        Invoke("SetText", 1.0f);
    }

    public void Display(PromptType _type)
    {
        if (type == _type)
        {
            SetText();
            Show();
            return;
        }
        
        Hide();
    }

    public void Hide(PromptType _type)
    {
        if (type == _type)
            Hide();
    }

    private void SetText()
    {
        switch(type)
        {
            case PromptType.Interact:
            {
                text.SetText(VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").KeyToListen.ToString());
                break;
            }
            case PromptType.Speech:
            {
                text.SetText(VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").KeyToListen.ToString());
                break;
            }
            case PromptType.Climb:
            {
                text.SetText(VirtualInputs.GetInputListener(InputType.PLAYER, "Jump").KeyToListen.ToString());
                break;
            }
            case PromptType.GrappleAim:
            {
                string temp;

                if (VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleAim").KeyToListen == KeyCode.Mouse0)
                {
                    temp = "LMB";
                }
                else if (VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleAim").KeyToListen == KeyCode.Mouse1)
                {
                    temp = "RMB";
                }
                else
                {
                    temp = VirtualInputs.GetInputListener(InputType.PLAYER, "GrappleAim").KeyToListen.ToString();
                }

                text.SetText(temp);
                break;
            }
            case PromptType.GrappleFire:
            {
                string temp;

                if (VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").KeyToListen == KeyCode.Mouse0)
                {
                    temp = "LMB";
                }
                else if (VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").KeyToListen == KeyCode.Mouse1)
                {
                    temp = "RMB";
                }
                else
                {
                    temp = VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").KeyToListen.ToString();
                }

                text.SetText(temp);
                break;
            }
            case PromptType.Glide:
            {
                text.SetText(VirtualInputs.GetInputListener(InputType.PLAYER, "Glide").KeyToListen.ToString());
                break;
            }
        }

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
