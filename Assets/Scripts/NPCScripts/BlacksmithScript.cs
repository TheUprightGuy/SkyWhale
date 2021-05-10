/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   BlacksmithScript.cs
  Description :   Derives from NPC Script - Listens for Puzzle Completion. 
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
    public PlayerMovement player;

    /// <summary>
    /// Description: Adds a listener as override.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    public override void Start()
    {
        dialogue.StartUp();
        followUpDialogue.StartUp();

        currentDialogue = dialogue;

        EventManager.StartListening("SwitchPuzzleCompletion", SwitchDialogue);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").MethodToCall.AddListener(Interact);
        CallbackHandler.instance.pause += Pause;
        CallbackHandler.instance.resetCamera += ResetCamera;
    }

    /// <summary>
    /// Description:  Switches dialogue after puzzle completion.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    public void SwitchDialogue()
    {
        currentDialogue = followUpDialogue;
        pm = player;
        Interact(InputState.KEYDOWN);
    }

    /// <summary>
    /// Description: Passes dialogue to dialogue manager.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public override void Interact(InputState type)
    {
        if (currentDialogue.inUse || !pm)
            return;

        cam.m_Priority = 2;

        CallbackHandler.instance.SetDialogue(currentDialogue);
        CallbackHandler.instance.Pause(true);

        CallbackHandler.instance.HidePrompt(PromptType.Speech);

        EventManager.TriggerEvent((currentDialogue != followUpDialogue) ? "TalkBlacksmith" : "ReturnBlacksmith");
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();

            CallbackHandler.instance.SpeechInRange(dialogueTransform);

            EventManager.TriggerEvent("FindBlacksmith");
        }
    }
}
