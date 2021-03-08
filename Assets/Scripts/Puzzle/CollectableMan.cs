using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableMan : MonoBehaviour
{
    Animator animator;
    TestMovement player;
    public float distToPlayer;
    bool waving;

    public string speaker;
    public string[] dialogue;
    public int dialogueIndex;
    bool done;

    #region Setup
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    #endregion Setup

    private void Update()
    {
        if (player)
        {
            distToPlayer = Vector3.Distance(player.transform.position, this.transform.position);
            waving = distToPlayer >= 5.0f;
        }

        UpdateAnimationState();
    }

    public void FinishDialogue()
    {
        if (!done)
        {
            CallbackHandler.instance.AddCollectableMan();
            Destroy(this.gameObject);
            done = true;
        }
    }

    public void Talk()
    {
        if (player && !waving)
        {
            CallbackHandler.instance.SetDialogue(speaker, dialogue[dialogueIndex]);
            CallbackHandler.instance.NextMessage();
            animator.SetTrigger("Talking");
            Invoke("FinishDialogue", 10.0f);
        }
    }

    void UpdateAnimationState()
    {
        animator.SetBool("Waving", waving);
    }

    private void OnTriggerEnter(Collider other)
    {
        player = other.gameObject.GetComponent<TestMovement>();
    }

    private void OnTriggerExit(Collider other)
    {
        player = null;
    }
}
