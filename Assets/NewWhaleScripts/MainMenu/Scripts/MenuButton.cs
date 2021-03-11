using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Local Variables
    [HideInInspector] public MenuButtonController menuButtonController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AnimatorFunctions animatorFunctions;
    [HideInInspector] public int thisIndex;

    public MenuOptions menuOptions;

    #region Setup
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animatorFunctions = GetComponent<AnimatorFunctions>();
    }
    #endregion Setup

    public virtual void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("Selected", true);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                animator.SetTrigger("Pressed");
                PauseMenuCanvasController.instance.ToggleMenuOption(menuOptions);
            }
        }
        else
        {
            animator.SetBool("Selected", false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuButtonController.index = thisIndex;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (menuButtonController.index == thisIndex)
        {
            menuButtonController.index = -1;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        animator.SetTrigger("Pressed");
        Invoke("ChangeMenu", 0.15f);
    }

    public void ChangeMenu()
    {
        PauseMenuCanvasController.instance.ToggleMenuOption(menuOptions);

    }
}
