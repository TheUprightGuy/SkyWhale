using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Local Variables
    [HideInInspector] public Animator animator;
    [HideInInspector] public AnimatorFunctions animatorFunctions;
    public bool mousedOver;

    #region Setup
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animatorFunctions = GetComponent<AnimatorFunctions>();
    }
    #endregion Setup

    private void Update()
    {
        if (mousedOver)
        {
            animator.SetBool("Selected", true);
        }
        else
        {
            animator.SetBool("Selected", false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mousedOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mousedOver = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        animator.SetTrigger("Pressed");
    }
}
