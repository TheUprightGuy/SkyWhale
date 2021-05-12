using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ApplyButton : MenuButton, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public override void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("Selected", true);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                animator.SetTrigger("Pressed");
            }
        }
        else
        {
            animator.SetBool("Selected", false);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
    }
}