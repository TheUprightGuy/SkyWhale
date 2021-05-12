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
using System;

[System.Serializable]
public struct QuestObjective
{
    public string eventName;
    public string objDesc;
    public Vector3 location;
}

[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest", order = 1)]
public class Quest : ScriptableObject
{
    public string questName;
    public List<QuestObjective> objectives;
    UnityAction questListener;
    public int index;

    List<UnityAction> listeners = new List<UnityAction>();

    /// <summary>
    /// Description: Notifies the eventmanager to start listening for objectives within the quest.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void Setup()
    {
        //CallbackHandler.instance.CreateQuestTracker(this);
        //questListener = new UnityAction(ProgressQuest);
        index = 0;
        //EventManager.StartListening(objectives[index].eventName, questListener);

        for (int i = 0; i < objectives.Count; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        listeners.Add(ProgressQuest0);
                        break;
                    }
                case 1:
                    {
                        listeners.Add(ProgressQuest1);
                        break;
                    }
                case 2:
                    {
                        listeners.Add(ProgressQuest2);
                        break;
                    }
                case 3:
                    {
                        listeners.Add(ProgressQuest3);
                        break;
                    }
                case 4:
                    {
                        listeners.Add(ProgressQuest4);
                        break;
                    }
                case 5:
                    {
                        listeners.Add(ProgressQuest5);
                        break;
                    }
                default:
                    Debug.LogError("QUEST TOO LONG!");
                    break;
            }

            EventManager.StartListening(objectives[i].eventName, listeners[i]);
        }


        //CallbackHandler.instance.SetQuestText(this, objectives[index].eventName + " " + tracking + "/" + objectives[index].quant);
        //CallbackHandler.instance.SetDialogueText(objectives[index].dialogueText, 3.0f);
    }

    public bool FinishedQuest()
    {
        return index >= objectives.Count;
    }

    /// <summary>
    /// Description: Notifies the eventmanager to stop listening .
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void OnDisable()
    {
        // This needs to be uncommented later - just giving errors in editor
       // EventManager.StopListening(objectives[index].eventName, questListener);
    }

    void ProgressQuest0()
    {
        ProgressQuest(0);
    }

    void ProgressQuest1()
    {
        ProgressQuest(1);
    }

    void ProgressQuest2()
    {
        ProgressQuest(2);
    }

    void ProgressQuest3()
    {
        ProgressQuest(3);
    }

    void ProgressQuest4()
    {
        ProgressQuest(4);
    }

    void ProgressQuest5()
    {
        ProgressQuest(5);
    }


    /// <summary>
    /// Description: Progresses the quest to next stage, notifying the eventmanager to start listening for next objective.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void ProgressQuest(int _i)
    {
        Debug.Log("QuestProgress!");
        index = _i;

        // Check if ready to progress to next objective
        // stop listening for prev objective
        for (int i = _i; i >= 0; i--)
        {
            EventManager.StopListening(objectives[i].eventName, listeners[i]);
        }

        index++;

        // Check if at end of objectives
        if (index >= objectives.Count)
        {
            // temp
            //CallbackHandler.instance.SetQuestText(this, "");
            Debug.Log("End of QuestChain");
        }
        // If not progress to next objective
        else
        {
            //EventManager.StartListening(objectives[index].eventName, questListener);
            //CallbackHandler.instance.SetQuestText(this, objectives[index].eventName + " " + tracking + "/" + objectives[index].quant);
            //CallbackHandler.instance.SetDialogueText(objectives[index].dialogueText, 3.0f);
        }

        CallbackHandler.instance.UpdateObjectives();
    }
    
}
