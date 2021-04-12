using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoadSceneLoader : MonoBehaviour
{
    #region Singleton
    public static PreLoadSceneLoader instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("PreLoadSceneLoader already exists!");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        OnAwake();
    }
    #endregion Singleton
    //Debug script to automatically load preload scene
    private void OnAwake()
    {
        SceneManager.LoadScene("PreloadScene");
    }
}
