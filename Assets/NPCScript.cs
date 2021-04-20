/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   NPCScript.cs
  Description :   Handles character interaction. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    PlayerMovement pm;
    bool pause;
    Transform dialogueTransform;

    private void Awake()
    {
        dialogueTransform = this.transform.GetChild(0);
    }

    #region Callbacks
    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public virtual void Start()
    {
        dialogue.StartUp();
        currentDialogue = dialogue;
        CallbackHandler.instance.pause += Pause;
        VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").MethodToCall.AddListener(Interact);
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.pause -= Pause;
    }
    #endregion Callbacks

    /// <summary>
    /// Description: Toggles Pause State.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_pause">Pause State</param>
    public void Pause(bool _pause)
    {
        pause = _pause;
    }

    /// <summary>
    /// Description: Rotates NPC to face player.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Update()
    {
        if (pause)
            return;

        if (pm)
        {
            Vector3 dir = pm.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
        }
    }

    public Dialogue dialogue;
    public Dialogue currentDialogue;

    /// <summary>
    /// Description: Passes dialogue to dialogue manager.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void Interact(InputState type)
    {
        if (currentDialogue.inUse || !pm)
            return;

        CallbackHandler.instance.SetDialogue(currentDialogue);
        CallbackHandler.instance.Pause(true);

        CallbackHandler.instance.HideSpeech();
    }

    #region Triggers
    /// <summary>
    /// Description: Gets player reference to rotate towards, ends dialogue on trigger exit.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="other">Triggering GameObject</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();

            CallbackHandler.instance.SpeechInRange(dialogueTransform);
            CallbackHandler.instance.ShowSpeech();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = null;
            CallbackHandler.instance.StopDialogue();

            CallbackHandler.instance.SpeechOutOfRange();
            CallbackHandler.instance.HideSpeech();
        }
    }
    #endregion Triggers
}
