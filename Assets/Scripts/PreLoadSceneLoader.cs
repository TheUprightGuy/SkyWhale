using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PreLoadSceneLoader : MonoBehaviour
{
    public bool startNewGameImmediately;
    //Uses start so that errors appear before scene change and log clearing
    #region Singleton
    public static PreLoadSceneLoader instance;
    private void Start()
    {
        if (instance != null)
        {
            Debug.Log("PreLoadSceneLoader already exists!");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        OnStart();
    }
    #endregion Singleton
    //Debug script to automatically load preload scene
    private void OnStart()
    {
        Debug.Log("Test preload scene logs");
        //ClearLog();
        SceneManager.LoadScene("PreloadScene");
    }
    
    private void Awake()
    {
        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
    }

    private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
    {
        //Return if not in menu scene or if immediate start is set to false
        if(SceneManager.GetActiveScene().name != "MainMenu" || !startNewGameImmediately) return;
        //Trigger immediate game start
        GameObject.FindGameObjectWithTag("MainMenuCanvas").BroadcastMessage("ToggleMenuOption", MenuOptions.NewGame);
    }
    
    /*public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }*/
}
