using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : NPCScript
{
    public Dialogue followUpDialogue;
    public Dialogue completedChallenges;
    private int _glidingChallengesCompleted;

    /// <summary>
    /// Description: Adds a listener as override.
    /// <br>Author: Wayd Barton-Redgrave Edited for pilot by Jacob Gallagher</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    public override void Start()
    {
        dialogue.StartUp();
        followUpDialogue.StartUp();
        completedChallenges.StartUp();

        currentDialogue = dialogue;
        _glidingChallengesCompleted = 0;

        EventManager.StartListening("PilotVillagerDialogueUpdate", SwitchDialogue);
        EventManager.StartListening("GrappleIslandGlidingChallengeComplete", SwitchDialogueFinal);
        VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").MethodToCall.AddListener(Interact);
        CallbackHandler.instance.pause += Pause;
        CallbackHandler.instance.resetCamera += ResetCamera;
    }

    /// <summary>
    /// Description:  Switches dialogue after enabling glider.
    /// <br>Author: Jacob Gallagher </br>
    /// <br>Last Updated: 05/10/2021</br> 
    /// </summary>
    public void SwitchDialogue()
    {
        currentDialogue = followUpDialogue;
    }
    
    /// <summary>
    /// Description:  Switches dialogue after completing gliding challenge.
    /// <br>Author: Jacob Gallagher </br>
    /// <br>Last Updated: 05/10/2021</br> 
    /// </summary>
    public void SwitchDialogueFinal()
    {
        _glidingChallengesCompleted++;
        if(_glidingChallengesCompleted < 3) return;
        currentDialogue = completedChallenges;
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
