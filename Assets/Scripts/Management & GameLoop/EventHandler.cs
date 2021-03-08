using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    #region Singleton
    public static EventHandler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one EventHandler in scene!");
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this.gameObject);
        // TEMP
        OnAwake();
    }

    

    #endregion
    [Header("Main Variables")]
    public GameState gameState;
    public GameObject stageEndCanvas;
    
    [Header("Cinematic durations")]
    public float establishingShotDuration;
    public float menuToGameViewDuration;
    public float freeRoamDuration = 60f;
    public float freeRoamAfterLampDuration = 15f;
    //Actions
    public Action menuClosed;
    public Action startEstablishingShot;
    public Action endEstablishingShot;
    public Action menuOpened;
    public Action resumePressed;
    public Action gameStart;

    private void OnAwake()
    {
        gameState.inMenu = true;
        gameState.gamePaused = true;
        gameState.inCinematic = false;
        gameState.playerOnIsland = false;
        gameState.objectivesHighlighted = new List<int>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameState.inCinematic)
        {
            gameState.inMenu = true;
            gameState.gamePaused = true;
            menuOpened?.Invoke();
            WhaleHandler.instance.LandingTooltip(false);
        }
    }

    public IEnumerator HighlightObjective(int index)
    {
        while (gameState.gamePaused)
        {
            yield return new WaitForSeconds(10f);    //Wait for the player to resume before highlighting the objective
        }
        //Don't check if it's already been highlighted
        if (gameState.objectivesHighlighted.Contains(index)) yield break;
        Debug.Log("Highlighting objective at index: " + index);
        gameState.objectivesHighlighted.Add(index);
        CinematicController.instance.currentObjectiveIndex = index;
        startEstablishingShot?.Invoke();
        gameState.gamePaused = true;
        StartCoroutine(WaitForEstablishingShot());
    }
    
    public IEnumerator HighlightObjective(GameObject target)
    {
        while (gameState.gamePaused)
        {
            yield return new WaitForSeconds(10f);    //Wait for the player to resume before highlighting the objective
        }

        CallbackHandler.instance.LerpCam();
        CinematicController.instance.StartCinematicShot(target);
        gameState.gamePaused = true;
        StartCoroutine(WaitForEstablishingShot());
    }

    public void OnPlayPressed()
    {
        Debug.Log("Starting shot");
        gameState.inMenu = false;
        StartCoroutine(MenuToGameViewShot());
        menuClosed?.Invoke();
    }

    public void OnLampBought()
    {
        StartCoroutine(FreeRoamTillFinalObjective());
    }
    
    private IEnumerator FreeRoamTillFinalObjective()
    {
        yield return new WaitForSeconds(freeRoamAfterLampDuration);
        StartCoroutine(HighlightObjective(2));    //Highlight final objective
        //WhaleMovementScript.instance.lamp.SetActive(true);
        AudioManager.instance.PlaySound("OtherWhaleSound");
    }
    
    private IEnumerator MenuToGameViewShot()
    {
        yield return new WaitForSeconds(menuToGameViewDuration);
        gameState.gamePaused = false;
        gameStart?.Invoke();
        /*
        yield return new WaitForSeconds(freeRoamDuration);
        StartCoroutine(HighlightObjective(0));    //Highlight shop objective
        */
    }

    private IEnumerator WaitForEstablishingShot()
    {
        yield return new WaitForSeconds(establishingShotDuration);
        gameState.gamePaused = false;
        endEstablishingShot?.Invoke();
        
    }

    public void OnEndTriggered()
    {
        stageEndCanvas.SetActive(true);
    }
}
