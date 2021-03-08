using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopOwner : MonoBehaviour
{
    Animator animator;
    TestMovement player;
    public float distToPlayer;
    bool waving;

    public string speaker;
    public string[] dialogue;
    public int dialogueIndex;

    #region Setup
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CallbackHandler.instance.buyItem += BuyItem;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.buyItem -= BuyItem;
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

    public void BuyItem()
    {
        animator.SetTrigger("Interact");
    }

    public void Talk()
    {
        if (player && !waving)
        {
            CallbackHandler.instance.SetDialogue(speaker, dialogue[dialogueIndex]);
            animator.SetTrigger("Talking");
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
