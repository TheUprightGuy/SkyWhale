/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   QuestManager.cs
  Description :   Tracks quests/objectives in partnership with the Event Manager. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    #region Singleton
    public static QuestManager instance;
    /// <summary>
    /// Description: Singleton Setup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one QuestManager exists!");
            Destroy(this.gameObject);
        }
        instance = this;
    }
    #endregion Singleton

    public List<Quest> activeQuests;

    private void Start()
    {
        SaveManager.instance.load += Setup;

        Invoke("Setup", 0.1f);
    }

    private void OnDestroy()
    {
        SaveManager.instance.load -= Setup;
    }

    /// <summary>
    /// Description: Resets Quests - temporary.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void Setup()
    {
        foreach (Quest n in activeQuests)
        {
            n.Setup();
            CallbackHandler.instance.StartTrackingQuest(n);
        }
    }



    /// <summary>
    /// Description: Tracks new quest.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="n">Quest to start tracking</param>
    public void AddQuest(Quest n)
    {
        activeQuests.Add(n);
    }
}
