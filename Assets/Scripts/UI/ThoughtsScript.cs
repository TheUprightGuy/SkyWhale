using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Thought
{
    Food = 0,
    Music,
    Weight,
    None
}

public class ThoughtsScript : MonoBehaviour
{
    #region Singleton
    public static ThoughtsScript instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one ThoughtsScript exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion Singleton

    WhaleInfo whaleInfo;
    float thoughtTimer = 0.0f;
    
    private void Start()
    {
        whaleInfo = CallbackHandler.instance.whaleInfo;

        thoughtTimer = UnityEngine.Random.Range(30.0f, 300.0f);
    }

    private void Update()
    {
        // Feed Whale
        /*if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (ResourceDisplayScript.instance.SpendProvisions(1))
            {
                SatisfyThought(Thought.Food);
                whaleInfo.FeedWhale();
            }
        }
        // Play Music
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SatisfyThought(Thought.Music);
        }
        // Remove Weight - NOT IMPLEMENTED
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SatisfyThought(Thought.Weight);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            whaleInfo.weight += 10.0f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            whaleInfo.weight -= 10.0f;
        }

        thoughtTimer -= Time.deltaTime;
        if (thoughtTimer <= 0)
        {
            SetThought(Thought.Music);
        }*/
    }

    public void SetThought(Thought _thought)
    {
        whaleInfo.currentThought = _thought;
        ShowThought(whaleInfo.currentThought, true);
        thoughtTimer = UnityEngine.Random.Range(30.0f, 300.0f);
    }

    public void SatisfyThought(Thought _thought)
    {
        if (whaleInfo.currentThought == _thought)
        {
            whaleInfo.currentThought = Thought.None;
            ShowThought(Thought.None, true);
        }
    }

    public event Action<Thought, bool> showThought;
    public void ShowThought(Thought _id, bool _toggle)
    {
        whaleInfo.currentThought = _id;

        if (showThought != null)
        {
            showThought(_id, _toggle);
        }
    }
}
