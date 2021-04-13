using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoadSceneLoader : MonoBehaviour
{
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
        ClearLog();
        SceneManager.LoadScene("PreloadScene");
    }
    
    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
