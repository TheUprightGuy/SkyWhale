/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   EventManager.cs
  Description :   Handles event triggers and function callbacks - mostly used for objectives. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> eventDictionary;

    private static EventManager tutorialListener;

    /// <summary>
    /// Description: Singleton Setup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public static EventManager instance
    {
        get
        {
            if (!tutorialListener)
            {
                tutorialListener = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!tutorialListener)
                {
                    Debug.LogError("Evenet Manager is missing!");
                }
                else
                {
                    tutorialListener.Init();
                }
            }
            return tutorialListener;
        }
    }

    /// <summary>
    /// Description: Initialises Dictionary - Event/Trigger.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    /// <summary>
    /// Description: Utilised to start listening for an event, and the action to call once triggered.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    /// <param name="eventName">String for Event</param>
    /// <param name="listener">Function to Call on Trigger</param>
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            //thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Description: Stop listening for an event.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br> 
    /// </summary>
    /// <param name="eventName">String for Event</param>
    /// <param name="listener">Function to remove listener for</param>
    public static void StopListening(string eventName, UnityAction listener)
    {
        if (tutorialListener == null)
            return;

        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Description: Triggers the event - calling the function paired with the eventname in the dictionary.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="eventName">Event to Trigger</param>
    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
