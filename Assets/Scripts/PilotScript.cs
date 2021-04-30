/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   BlacksmithScript.cs
  Description :   Derives from NPC Script - Listens for Special collectable collection.  Was modified from blacksmith script
  Date        :   07/04/2021
  Author      :   Jacob Gallagher
  Mail        :   Jacob.Gallagher1@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotScript : NPCScript
{
    public Dialogue followUpDialogue;
    public Dialogue gatheredMaterialFirstDialogue;
    public Quest pilotQuest;

    private bool _talkedTo = false;

    /// <summary>
    /// Description: Adds a listener as override.
    /// <br>Author: Wayd Barton-Redgrave Edited for pilot by Jacob Gallagher</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    public override void Start()
    {
        dialogue.StartUp();
        followUpDialogue.StartUp();

        currentDialogue = dialogue;

        EventManager.StartListening("SpecialMetalCollected", SwitchDialogue);
        EventManager.StartListening("StartPilotQuest", StartPilotQuest);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").MethodToCall.AddListener(Interact);
        CallbackHandler.instance.pause += Pause;
        CallbackHandler.instance.resetCamera += ResetCamera;
    }

    /// <summary>
    /// Description:  Switches dialogue after collecting material.
    /// Dialogue to switch to depends if the player has already talked to the pilot 
    /// <br>Author: Jacob Gallagher </br>
    /// <br>Last Updated: 04/30/2021</br> 
    /// </summary>
    public void SwitchDialogue()
    {
        currentDialogue = _talkedTo ? followUpDialogue : gatheredMaterialFirstDialogue;
    }

    /// <summary>
    /// Description: Passes dialogue to dialogue manager.
    /// <br>Author: Wayd Barton-Redgrave Edited for pilot by Jacob Gallagher</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public override void Interact(InputState type)
    {
        if (currentDialogue.inUse || !pm)
            return;

        cam.m_Priority = 2;

        CallbackHandler.instance.SetDialogue(currentDialogue);
        CallbackHandler.instance.Pause(true);

        CallbackHandler.instance.HideSpeech();
        _talkedTo = true;

        if (currentDialogue == followUpDialogue)
        {
            EventManager.TriggerEvent("ReturnPilot");
            return;
        }
    }

    public void StartPilotQuest()
    {
        QuestManager.instance.AddQuest(pilotQuest);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();

            CallbackHandler.instance.SpeechInRange(dialogueTransform);
            CallbackHandler.instance.ShowSpeech();
        }
    }
}
