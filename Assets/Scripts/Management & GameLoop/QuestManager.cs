using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    #region Singleton
    public static QuestManager instance;
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
        Invoke("Setup", 0.1f);
    }

    void Setup()
    {
        foreach (Quest n in activeQuests)
        {
            n.Setup();
        }
    }

    public void AddQuest(Quest n)
    {
        activeQuests.Add(n);
    }
}
