using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithScript : NPCScript
{
    public Dialogue followUpDialogue;

    public override void Start()
    {
        dialogue.StartUp();
        followUpDialogue.StartUp();

        currentDialogue = dialogue;

        EventManager.StartListening("SwitchPuzzleCompletion", SwitchDialogue);

        CallbackHandler.instance.pause += Pause;
    }
    
    public void SwitchDialogue()
    {
        currentDialogue = followUpDialogue;
    }
}
