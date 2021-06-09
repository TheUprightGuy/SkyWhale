using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CameraManager.instance.letterbox += Letterbox;
        CameraManager.instance.standard += Standard;
    }
    private void OnDestroy()
    {
        CameraManager.instance.letterbox -= Letterbox;
        CameraManager.instance.standard -= Standard;
    }

    public void Letterbox()
    {
        animator.SetBool("Fade", true);
    }

    public void Standard()
    {
        animator.SetBool("Fade", false);
    }
}
