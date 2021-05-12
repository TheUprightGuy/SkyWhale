using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleTutorial : MonoBehaviour
{
    public List<GameObject> tutorialCanvases;
    private GrappleScript _grappleScript;

    private float _timer;
    private const float MAXTime = 0.75f;
    private int tutorialsCompleted;
    private bool showingTutorial;

    private void Awake()
    {
        _grappleScript = gameObject.GetComponent<GrappleScript>();
        _timer = 0f;
        tutorialsCompleted = 0;
        showingTutorial = false;
        VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").MethodToCall.AddListener(EndGrappleReleaseTutorial);
        CallbackHandler.instance.onGrappleJump += FailGrappleReleaseTutorial;
    }

    private void EndGrappleReleaseTutorial(InputState arg0)
    {
        _timer = 0f;
        if (!showingTutorial || tutorialsCompleted != 0) return;
        EndTutorial();
        CallbackHandler.instance.onGrappleJump -= FailGrappleReleaseTutorial;
        CallbackHandler.instance.onGrappleJump += EndGrappleJumpTutorial;
        VirtualInputs.GetInputListener(InputType.PLAYER, "Grapple").MethodToCall.AddListener(FailGrappleJumpTutorial);
    }

    private void EndGrappleJumpTutorial()
    {
        _timer = 0f;
        if (!showingTutorial || tutorialsCompleted != 1) return;
        EndTutorial();
        CallbackHandler.instance.onGrappleJump -= EndGrappleJumpTutorial;
        GetComponent<PlayerMovement>().playerState = PlayerMovement.PlayerStates.FALLING;
        GetComponent<Rigidbody>().velocity = transform.forward * 5f + Vector3.up * 6f;
        Destroy(this);
    }

    private void EndTutorial()
    {
        //Disable freeze time
        TimeSlowDown.instance.timeScale = 1f;
        TimeSlowDown.instance.stop = false;

        //Disable letterbox goes here
        
        //Hide canvas UI
        Destroy(tutorialCanvases[tutorialsCompleted]);
        tutorialsCompleted++;
        showingTutorial = false;
        GetComponent<PlayerMovement>().playerState = PlayerMovement.PlayerStates.FALLING;
    }

    private void FailTutorial()
    {
        //Disable tutorial without ending it
        //Unfreeze time
        TimeSlowDown.instance.timeScale = 1f;
        TimeSlowDown.instance.stop = false;
        
        
        //Disable Letterbox effect

        
        GetComponent<PlayerMovement>().playerState = PlayerMovement.PlayerStates.FALLING;
        
        showingTutorial = false;
    }

    private void FailGrappleReleaseTutorial()
    {
        if(!showingTutorial || tutorialsCompleted != 0) return;
        //Hide tutorial UI
        tutorialCanvases[0].SetActive(false);
        FailTutorial();
    }
    
    private void FailGrappleJumpTutorial(InputState arg0)
    {
        if(!showingTutorial || tutorialsCompleted != 1) return;
        tutorialCanvases[1].SetActive(false);
        FailTutorial();
    }

    // Update is called once per frame
    void Update()
    {
        if (showingTutorial) return;
            if (tutorialsCompleted < tutorialCanvases.Count && _grappleScript.hook.connected)
        {
            _timer += Time.deltaTime;
            if (_timer >= MAXTime)
            {
                //Trigger first tutorial
                TriggerTutorial(tutorialCanvases[tutorialsCompleted]);
            }
        }
    }

    private void TriggerTutorial(GameObject tutorialCanvas)
    {
        showingTutorial = true;
        //Freeze time
        TimeSlowDown.instance.timeScale = 0f;
        TimeSlowDown.instance.stop = true;
        
        //Letterbox effect goes here

        //Show tutorial UI
        tutorialCanvas.SetActive(true);
    }
}
