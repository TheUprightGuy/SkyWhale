﻿/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   BlacksmithScript.cs
  Description :   Inherits from NPC Script - Starts listening for puzzle completion. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithScript : NPCScript
{
    public Dialogue followUpDialogue;

    /// <summary>
    /// Description: Adds listener as ovverride from NPC script.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public override void Start()
    {
        dialogue.StartUp();
        followUpDialogue.StartUp();

        currentDialogue = dialogue;

        EventManager.StartListening("SwitchPuzzleCompletion", SwitchDialogue);

        CallbackHandler.instance.pause += Pause;
    }

    /// <summary>
    /// Description: Provides follow up dialogue upon puzzle completion.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void SwitchDialogue()
    {
        currentDialogue = followUpDialogue;
    }
}
