using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveTrigger : MonoBehaviour
{
    bool tutMsg;

    private void OnTriggerEnter(Collider other)
    {
        TestMovement player = other.GetComponent<TestMovement>();

        if (player && !tutMsg)
        { 
            TutorialMessage caveTutorial = new TutorialMessage();
            caveTutorial.message = "I think I can hear someone in there?";
            caveTutorial.timeout = 5.0f;
            caveTutorial.key = KeyCode.E;
            // CHANGE OBJECTIVE HERE

            // not working?
            CallbackHandler.instance.AddMessage(caveTutorial);
            CallbackHandler.instance.NextMessage();
            tutMsg = true;
        }
    }
}
