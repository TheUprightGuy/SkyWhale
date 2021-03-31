using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    PlayerMovement pm;
    bool pause;


    public virtual void Start()
    {
        dialogue.StartUp();
        currentDialogue = dialogue;
        CallbackHandler.instance.pause += Pause;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.pause -= Pause;
    }

    public void Pause(bool _pause)
    {
        pause = _pause;
    }

    private void Update()
    {
        if (pause)
            return;

        if (pm)
        {
            Vector3 dir = pm.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.E) && !currentDialogue.inUse)
            {
                Interact();
            }
        }
    }

    public Dialogue dialogue;
    public Dialogue currentDialogue;

    void Interact()
    {
        CallbackHandler.instance.SetDialogue(currentDialogue);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = other.GetComponent<PlayerMovement>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            pm = null;
            CallbackHandler.instance.StopDialogue();
        }
    }
}
