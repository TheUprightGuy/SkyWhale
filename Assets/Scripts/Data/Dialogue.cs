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

    public int dialogueIndex = 0;

    public bool inUse;

    public void StartUp()
    {
        inUse = false;
        dialogueIndex = 0;
        foreach(DialogueComponent n in dialogue)
        {
            n.dialogueIndex = 0;
        }
    }


    // Get next dialogue component
    public DialogueComponent GetDialogue()
    {
        return (dialogue[dialogueIndex]);
    }

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
        return null;        
    }

    public bool Advance()
    {
        return (dialogueIndex + 1 < dialogue.Count);
    }
}
