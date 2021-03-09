using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct QuestObjective
{
    public string eventName;
    public int quant;
    public string dialogueText;
}

[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest", order = 1)]
public class Quest : ScriptableObject
{
    public List<QuestObjective> objectives;
    UnityAction questListener;
    int index;
    int tracking;

    public void Setup()
    {
        //CallbackHandler.instance.CreateQuestTracker(this);
        questListener = new UnityAction(ProgressQuest);
        index = 0;
        tracking = 0;
        EventManager.StartListening(objectives[index].eventName, questListener);
        //CallbackHandler.instance.SetQuestText(this, objectives[index].eventName + " " + tracking + "/" + objectives[index].quant);
        //CallbackHandler.instance.SetDialogueText(objectives[index].dialogueText, 3.0f);
    }

    private void OnDisable()
    {
        EventManager.StopListening(objectives[index].eventName, questListener);
    }

    public void ProgressQuest()
    {
        Debug.Log("QuestProgress!");

        // Update Count
        tracking++;
        // Check if ready to progress to next objective
        if (tracking >= objectives[index].quant)
        {
            // stop listening for prev objective
            EventManager.StopListening(objectives[index].eventName, questListener);

            // Progress to next
            tracking = 0;
            index++;

            // Check if at end of objectives
            if (index >= objectives.Count)
            {
                // temp
                //CallbackHandler.instance.SetQuestText(this, "");
            }
            // If not progress to next objective
            else
            {
                EventManager.StartListening(objectives[index].eventName, questListener);
                //CallbackHandler.instance.SetQuestText(this, objectives[index].eventName + " " + tracking + "/" + objectives[index].quant);
                //CallbackHandler.instance.SetDialogueText(objectives[index].dialogueText, 3.0f);
            }
        }
        // Else just update quest text
        else
        {
            //CallbackHandler.instance.SetQuestText(this, objectives[index].eventName + " " + tracking + "/" + objectives[index].quant);
        }
    }
}
