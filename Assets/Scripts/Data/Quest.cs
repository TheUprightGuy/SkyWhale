/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   Quest.cs
  Description :   Quest SO with objectives to complete. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

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

    /// <summary>
    /// Description: Notifies the eventmanager to start listening for objectives within the quest.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
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

    /// <summary>
    /// Description: Notifies the eventmanager to stop listening .
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void OnDisable()
    {
        EventManager.StopListening(objectives[index].eventName, questListener);
    }

    /// <summary>
    /// Description: Progresses the quest to next stage, notifying the eventmanager to start listening for next objective.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
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
