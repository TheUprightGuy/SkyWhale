/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   Dialogue.cs
  Description :   Dialogue SO that contains both sides of a conversation. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueComponent
{
    public Sprite image;
    public bool left;
    public string name;
    public List<string> dialogue;

    public int dialogueIndex;

    // Move to next string, return true if in range.
    public bool Advance()
    {
        return (dialogueIndex + 1 < dialogue.Count);
    }
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    public List<DialogueComponent> dialogue;
    public Sprite leftCharacter;
    public Sprite rightCharacter;

    [HideInInspector] public int dialogueIndex = 0;
    //[HideInInspector] 
    public bool inUse;

    [Header("Trigger Callbacks")]
    public string triggerEvent;

    /// <summary>
    /// Description: Resets dialogue to the first stage.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void StartUp()
    {
        inUse = false;
        dialogueIndex = 0;
        foreach(DialogueComponent n in dialogue)
        {
            n.dialogueIndex = 0;
        }
    }

    /// <summary>
    /// Description: Gets dialogue from next index.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <returns></returns>
    public DialogueComponent GetDialogue()
    {
        return (dialogue[dialogueIndex]);
    }

    /// <summary>
    /// Description: Progresses dialogue to next index if possible.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <returns>Next Dialogue Chunk</returns>
    public DialogueComponent Progress()
    {
        // Check if theres more strings in current dialogue
        if (dialogue[dialogueIndex].Advance())
        {
            // if so return same component, next index for string
            dialogue[dialogueIndex].dialogueIndex++;
            return dialogue[dialogueIndex];
        }   
        // Check if theres more components in current dialogue
        if (Advance())
        {
            // if so move to next component at next index
            dialogueIndex++;
            return dialogue[dialogueIndex];
        }
        // Return nullable value (end of dialogue)
        FinishDialogueEvent();
        return null;        
    }

    /// <summary>
    /// Description: Triggers dialogue event if any.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    void FinishDialogueEvent()
    {
        if (triggerEvent != "")
            EventManager.TriggerEvent(triggerEvent);

        CallbackHandler.instance.Pause(false);
        CallbackHandler.instance.ResetCamera();
    }

    /// <summary>
    /// Description: Checks if dialogue can be advanced.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <returns>Can dialogue be advanced</returns>
    public bool Advance()
    {
        return (dialogueIndex + 1 < dialogue.Count);
    }
}
