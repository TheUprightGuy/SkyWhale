using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandSlowDown : MonoBehaviour
{
    [Header("Setup Fields")]
    [HideInInspector] public bool playerInRange = false;
    public float colliderHeight;

    private void Start()
    {
        Invoke("SwitchConvex", 1.0f);
    }

    public void SwitchConvex()
    {
        GetComponent<MeshCollider>().convex = true;
    }

    static bool tutMessage = false;
    private void OnTriggerEnter(Collider other)
    {
        Movement player = other.GetComponent<Movement>();

        if (player)
        {

            if (!tutMessage)
            {
                TutorialMessage orbitTutorial = new TutorialMessage();
                orbitTutorial.message = "When close enough, press E to transfer to the island.";
                orbitTutorial.timeout = 5.0f;
                orbitTutorial.key = KeyCode.E;
                CallbackHandler.instance.AddMessage(orbitTutorial);
                CallbackHandler.instance.NextMessage();

                //PopUpHandler.instance.QueuePopUp("Press <b>E</b> to transfer to the island", KeyCode.E);
            }


            playerInRange = true;
            player.inRange = true;
            player.orbit.leashObject = this.gameObject;
            tutMessage = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Movement player = other.GetComponent<Movement>();

        if (player)
        {
            playerInRange = false;
            player.inRange = false;
        }
    }
}
