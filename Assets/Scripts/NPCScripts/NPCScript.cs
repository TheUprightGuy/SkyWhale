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
using Audio;
using UnityEngine;

public enum NPCType
{
    BS,
    PL,
    SH,
    Inanimate
}


public class NPCScript : MonoBehaviour
{
    public  PlayerMovement pm;
    [HideInInspector] public bool pause;
    protected Transform dialogueTransform;
    [HideInInspector] public Animator anim;
    public Cinemachine.CinemachineVirtualCamera cam;
    public NPCType type;
    private GameObject audioSource;

    private void Awake()
    {
        dialogueTransform = this.transform.GetChild(0);
        cam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        anim = GetComponent<Animator>();

        if (!anim)
            return;

        if (type != NPCType.Inanimate)
            anim.SetBool(type.ToString(), true);
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
        CallbackHandler.instance.resetCamera += ResetCamera;
        VirtualInputs.GetInputListener(InputType.PLAYER, "Interact").MethodToCall.AddListener(Interact);

        if (callbackToSwitchDialogue != "" && dialoguesToSwitchTo.Count>0)
        {
            CallbackHandler.instance.BroadcastMessage(callbackToSwitchDialogue, gameObject);
        }
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.pause -= Pause;
        CallbackHandler.instance.resetCamera -= ResetCamera;
    }
    #endregion Callbacks

    public void SwitchDialogue()
    {
        if (callbackToSwitchDialogue == "" || dialoguesToSwitchTo.Count <= 0) return;
        if (!dialoguesToSwitchTo.Contains(currentDialogue))
        {
            currentDialogue = dialoguesToSwitchTo[0];
            return;
        }
        for (var i = 0; i < dialoguesToSwitchTo.Count; i++)
        {
            if (currentDialogue != dialoguesToSwitchTo[i]) continue;
            if(i+1 >= dialoguesToSwitchTo.Count) return;
            currentDialogue = dialoguesToSwitchTo[i + 1];
            return;
        }     
    }

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

        if (pm && type != NPCType.Inanimate)
        {
            Vector3 dir = pm.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
        }
    }

    public Dialogue dialogue;
    public Dialogue currentDialogue;
    public List<Dialogue> dialoguesToSwitchTo;
    public string callbackToSwitchDialogue;

    /// <summary>
    /// Description: Passes dialogue to dialogue manager.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public virtual void Interact(InputState type)
    {
        if (currentDialogue.inUse || !pm || pause)
            return;

        if (pm && pm.cinematicPause)
            return;

        cam.m_Priority = 2;

        CallbackHandler.instance.SetDialogue(currentDialogue);
        CallbackHandler.instance.Pause(true);
        CallbackHandler.instance.HidePrompt(PromptType.Speech);

        anim.SetBool("Talk", true);
        AudioManager.instance.PlaySound("Click");
    }

    public void ResetCamera()
    {
        cam.m_Priority = 0;
        anim.SetBool("Talk", false);
    }

    #region Triggers
    /// <summary>
    /// Description: Gets player reference to rotate towards, ends dialogue on trigger exit.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="other">Triggering GameObject</param>
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();

            CallbackHandler.instance.SpeechInRange(dialogueTransform);

            if (!anim)
                return;

            anim.SetBool("Wave", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = null;
            CallbackHandler.instance.StopDialogue();

            CallbackHandler.instance.SpeechOutOfRange();

            if (!anim)
                return;

            anim.SetBool("Wave", false);
            anim.SetBool("Talk", false);
        }
    }
    #endregion Triggers
    
    public void TriggerSound(string soundName)
    {
        if (audioSource == null)
        {
            audioSource = GetComponentInChildren<AudioSource>().gameObject;
        }
        AudioManager.instance.PlaySound(audioSource.GetInstanceID().ToString());
    }
}
