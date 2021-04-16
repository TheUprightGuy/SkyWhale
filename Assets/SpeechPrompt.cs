/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   SpeechPrompt.cs
  Description :   Handles positioning and animation of speech bubble ui element. 
  Date        :   16/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechPrompt : MonoBehaviour
{
    #region Setup
    Transform dialogueTransform;
    GameObject image;
    Animator animator;
    bool inRange;

    /// <summary>
    /// Description: Setup local component references.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
        image = this.transform.GetChild(0).gameObject;
    }
    #endregion Setup

    /// <summary>
    /// Description: Updates animator and position of speechbubble.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    void Update()
    {
        animator.SetBool("Show", inRange);

        if (!dialogueTransform)
            return;

        // Set position - if desired position is behind mc, hide.
        this.transform.position = Camera.main.WorldToScreenPoint(dialogueTransform.position);
        image.SetActive(this.transform.position.z > 0);
    }

    /// <summary>
    /// Description: Gets the desired position to project onto screen plane and sets inrange to true.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    /// <param name="_dialogue"></param>
    public void InRange(Transform _dialogue)
    {
        dialogueTransform = _dialogue;
        inRange = true;
    }

    /// <summary>
    /// Description: Starts fadeout animation as desired position is lost.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    public void OutOfRange()
    {
        inRange = false;
    }

    /// <summary>
    /// Description: Removes desired position reference (so that it keeps correct position until fadeout is complete).
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    public void Hide()
    {
        dialogueTransform = null;
    }
}
