using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void FadeIn()
    {
        anim.SetBool("Fade", false);
    }

    public void FadeOut()
    {
        anim.SetBool("Fade", true);
    }
}
