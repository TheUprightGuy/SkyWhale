using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    PlayerMovement pm;

    private void Start()
    {
        dialogue.StartUp();
    }

    private void Update()
    {
        if (pm)
        {
            Vector3 dir = pm.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.E) && !dialogue.inUse)
            {
                Interact();
            }
        }
    }

    public Dialogue dialogue;

    void Interact()
    {
        CallbackHandler.instance.SetDialogue(dialogue);
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
