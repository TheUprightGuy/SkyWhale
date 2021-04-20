using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugTool : MonoBehaviour
{
    public List<Transform> teleportLocations;
    public GameObject genericButtonTemplate;
    private bool _debugToolsActive;
    private bool _grappleEnabled;
    private bool _gliderEnabled;
    // Start is called before the first frame update
    void Start()
    {
        var index = 0;
        foreach (var location in teleportLocations)
        {
            var newButton = Instantiate(genericButtonTemplate, transform.GetChild(0));
            newButton.transform.GetChild(0).GetComponent<Text>().text += index;
            newButton.GetComponent<DebugTeleportButton>().locationToTeleport = location;
            index++;
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.BackQuote)) return;
        ToggleDebugToolMenu();
    }

    public void ToggleDebugToolMenu()
    {
        _debugToolsActive = !_debugToolsActive;
        Cursor.lockState = _debugToolsActive ? CursorLockMode.None : CursorLockMode.Locked;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(_debugToolsActive);
        }
    }
    
    public void EnableGrapple()
    {
        _grappleEnabled = !_grappleEnabled;
        EntityManager.instance.player.GetComponent<GrappleScript>().enabled = _grappleEnabled;
    }
    
    public void EnableGlider()
    {
        _gliderEnabled = !_gliderEnabled;
        EntityManager.instance.player.GetComponent<GliderMovement>().unlocked = _gliderEnabled;
    }
}
