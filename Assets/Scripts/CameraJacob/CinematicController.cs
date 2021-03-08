using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinematicController : MonoBehaviour
{
    #region Singleton
    public static CinematicController instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one CinematicController exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            OnAwake();
        }
    }
    #endregion Singleton

    [Header("Solo VCam's for cinematics")]
    //First cam should always be menuCam
    public List<CinemachineVirtualCamera> VCams;
    [Header("Cinematic Blend List Cameras")]
    public List<CinemachineBlendListCamera> BlendLists;
    public Transform whaleTransform;
    public Transform playerTransform;
    //Objectives order (object of focus in cinematic):
    //0 is Shop objective
    //1 is island objective
    //2 is final objective
    //3 is switch puzzle objective
    //4 is opened door
    public List<ObjectiveData> objectives;    //Old system, currently using map objectives
    public int currentObjectiveIndex;

    public struct ObjectiveData
    {
        public Transform lookAt;
        public Transform moveTo;
        public int index;
    }
    public void OnAwake()
    {
        currentObjectiveIndex = 0;
        objectives = new List<ObjectiveData>();
    }

    // Start is called before the first frame update
    void Start()
    {
        EventHandler.instance.menuClosed += OnMenuClosed;
        EventHandler.instance.startEstablishingShot += OnStartEstablishingShot;
        EventHandler.instance.endEstablishingShot += OnEndEstablishingShot;
        EventHandler.instance.menuOpened += OnMenuOpened;
        EventHandler.instance.resumePressed += OnResume;
        EventHandler.instance.gameStart += OnGameStart;
    }
    // Remember to add cleanup for callbacks
    private void OnDestroy()
    {
        EventHandler.instance.menuClosed -= OnMenuClosed;
        EventHandler.instance.startEstablishingShot -= OnStartEstablishingShot;
        EventHandler.instance.endEstablishingShot -= OnEndEstablishingShot;
        EventHandler.instance.menuOpened -= OnMenuOpened;
        EventHandler.instance.resumePressed -= OnResume;
        EventHandler.instance.gameStart -= OnGameStart;
    }

    private void OnMenuClosed()
    {
        VCams[0].m_Priority = 0;
    }

    private void OnGameStart()
    {
        //EventHandler.instance.menuOpened -= OnMenuOpened;
        EventHandler.instance.menuClosed -= OnMenuClosed;
    }

    private void OnResume()
    {
        VCams[0].m_Priority = 0;
    }
    
    private void OnMenuOpened()
    {
        if (EventHandler.instance.gameState.playerOnIsland) return;
        BlendLists[0].gameObject.SetActive(false);
        VCams[0].m_Priority = 11;
    }
    
    private void OnStartEstablishingShot()
    {
        //Find objective with current index
        foreach (var objectiveData in objectives)
        {
            if (objectiveData.index == currentObjectiveIndex)
            {
                //Set quest objective
                CallbackHandler.instance.SetQuestObjective(objectiveData.lookAt.gameObject);
                
                var lookAtObj = 
                    BlendLists[0].ChildCameras[1];
                lookAtObj.Follow = whaleTransform;
                lookAtObj.LookAt = objectives[currentObjectiveIndex].lookAt;
        
                var moveToObj = 
                    BlendLists[0].GetComponent<CinemachineBlendListCamera>().ChildCameras[2];
                moveToObj.Follow = objectives[currentObjectiveIndex].moveTo;
                moveToObj.LookAt = objectives[currentObjectiveIndex].lookAt;
        
                BlendLists[0].gameObject.SetActive(true);
                EventHandler.instance.gameState.inCinematic = true;
                break;
            }
        }
    }
    
    public void StartCinematicShot(GameObject target)
    {
        var objective = target.GetComponent<ObjectiveMovePoint>();
        var objData = objective.objectiveData;
                
        var gameView =  
            BlendLists[0].ChildCameras[0];
        
        var lookAtObj = 
            BlendLists[0].ChildCameras[1];
        if (EventHandler.instance.gameState.playerOnIsland)
        {
            gameView.Follow = playerTransform;
            lookAtObj.Follow = playerTransform;
        }
        else
        {
            gameView.Follow = whaleTransform;
            lookAtObj.Follow = whaleTransform;
        }
        lookAtObj.LookAt = objData.lookAt;
        
        var moveToObj = 
            BlendLists[0].GetComponent<CinemachineBlendListCamera>().ChildCameras[2];
        moveToObj.Follow = objData.moveTo;
        moveToObj.LookAt = objData.lookAt;

        BlendLists[0].Priority = 11;
        EventHandler.instance.gameState.inCinematic = true;
    }
    
    private void OnEndEstablishingShot()
    {
        BlendLists[0].Priority = 0;
        EventHandler.instance.gameState.inCinematic = false;
    }
}
