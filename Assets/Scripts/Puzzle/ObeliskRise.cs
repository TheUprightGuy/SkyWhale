using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskRise : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartAnim()
    {
        animator.SetTrigger("Start");
    }
}
