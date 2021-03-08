using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Show()
    {
        animator.SetTrigger("Show");
    }
    public void Hide()
    {
        Invoke("HideSlide", 5.0f);
    }
    public void HideSlide()
    {
        animator.SetTrigger("Hide");
    }
}
