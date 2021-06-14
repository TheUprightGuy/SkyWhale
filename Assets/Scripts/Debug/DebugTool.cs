using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugTool : MonoBehaviour
{
    #region Singleton

    public static DebugTool instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Debug tool already exists");
            Destroy(gameObject);
            return;
        }

        instance = this;
        OnAwake();
    }

    #endregion
    
    public List<Transform> teleportLocations;
    public GameObject genericButtonTemplate;
    private bool _debugToolsActive;
    private bool _grappleEnabled;
    private bool _gliderEnabled;
    public float boostAmount;

    // Start is called before the first frame update
    void OnAwake()
    {
        foreach (var location in teleportLocations)
        {
            AddNewButton(location);
        }
    }

    private void AddNewButton(Transform location)
    {
        var newButton = Instantiate(genericButtonTemplate, transform.GetChild(0));
        newButton.transform.GetChild(0).GetComponent<Text>().text = location.name;
        newButton.GetComponent<DebugTeleportButton>().locationToTeleport = location;
    }

    public void AddNewTeleportLocation(Transform location)
    {
        //Add new button if not already added
        if(!teleportLocations.Contains(location)) AddNewButton(location);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.B))
        {
            EntityManager.instance.whale.GetComponent<WhaleMovement>().boost = boostAmount;
        }*/
        if (!Input.GetKeyDown(KeyCode.BackQuote)) return;
        ToggleDebugToolMenu();
    }

    public void ToggleDebugToolMenu()
    {
        _debugToolsActive = !_debugToolsActive;
        Cursor.lockState = _debugToolsActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _debugToolsActive;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(_debugToolsActive);
        }
    }
    
    public void EnableGrapple()
    {
        _grappleEnabled = !_grappleEnabled;
        EntityManager.instance.player.GetComponent<GrappleScript>().enabled = _grappleEnabled;
        ToggleDebugToolMenu();
        Destroy(EntityManager.instance.player.GetComponent<GrappleTutorial>());
    }
    
    public void EnableGlider()
    {
        _gliderEnabled = !_gliderEnabled;
        EntityManager.instance.player.GetComponent<GliderMovement>().unlocked = _gliderEnabled;
        ToggleDebugToolMenu();
    }

    public void ReleaseWhale()
    {
        EventManager.TriggerEvent("WhaleCinematic");
    }

    public void SetDay()
    {
        CallbackHandler.instance.SetTimeOfDay(0.25f);
    }

    public void SetNight()
    {
        CallbackHandler.instance.SetTimeOfDay(0.9f);
    }

    public void SetRain()
    {
        CallbackHandler.instance.ToggleRain(true);
    }

    public void StopRain()
    {
        CallbackHandler.instance.ToggleRain(false);
    }
}
