using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TutorialMessage
{
    public string message;
    public KeyCode key;
    public float timeout;
}

public class TutorialScript : MonoBehaviour
{
    public List<TutorialMessage> tutorialMessages;
    int index = -1;
    float delay = 5.0f;

    private void Start()
    {
        CallbackHandler.instance.nextMessage += NextMessage;
        CallbackHandler.instance.addMessage += AddMessage;
        CallbackHandler.instance.startTutorial += NextMessage;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.nextMessage -= NextMessage;
        CallbackHandler.instance.addMessage -= AddMessage;
        CallbackHandler.instance.startTutorial -= NextMessage;
    }

    public void NextMessage()
    {
        if (index < tutorialMessages.Count - 1)
        {
            index++;

            Invoke("SetMessage", delay);
        }
        delay = 5.0f;
    }

    public void SetMessage()
    {
        CallbackHandler.instance.SetTutorialMessage(tutorialMessages[index]);
    }

    public void AddMessage(TutorialMessage _msg)
    {
        tutorialMessages.Add(_msg);
        delay = 0.0f;
    }
}
