using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public GameObject player;
    public GameObject mainCam;
    public GameObject whaleCam;
    public GameObject whaleGrappleCam;
    public GameObject whaleGrappleUIElement;
    public NewGrappleHook grappleHook;

    private void Start()
    {
        CallbackHandler.instance.dismountPlayer += OndismountPlayer;
        CallbackHandler.instance.grappleFromWhale += OngrappleFromWhale;
        CallbackHandler.instance.grappleHitFromWhale += OngrappleHitFromWhale;
    }

    private void OngrappleFromWhale(Transform grapplePos)
    {
        whaleGrappleCam.SetActive(true);
        whaleCam.SetActive(false);
    }

    private void OngrappleHitFromWhale(Transform grapplePos)
    {
        player.transform.position = grapplePos.position;
        player.transform.rotation = Quaternion.LookRotation(grappleHook.transform.position.normalized, Vector3.up);


        player.SetActive(true);

        mainCam.SetActive(true);
        whaleGrappleCam.SetActive(false);
        whaleCam.SetActive(false);

        whaleGrappleUIElement.SetActive(false);

        grappleHook.connected = true;
        grappleHook.pc = player.transform;

        //player.GetComponent<NewGrappleScript>().ToggleAim(false); 
    }

    private void OndismountPlayer(Transform dismountPosition)
    {
        player.transform.position = dismountPosition.position;
        player.transform.rotation = Quaternion.identity;
        player.SetActive(true);

        mainCam.SetActive(true);

        whaleCam.SetActive(false);
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.dismountPlayer -= OndismountPlayer;
        CallbackHandler.instance.grappleFromWhale -= OngrappleFromWhale;
        CallbackHandler.instance.grappleHitFromWhale -= OngrappleHitFromWhale;
    }
} 
