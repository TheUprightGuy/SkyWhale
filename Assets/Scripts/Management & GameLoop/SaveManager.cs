/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   SaveManager.cs
  Description :   Handles saving and loading of current gamestate. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SaveManager : MonoBehaviour
{
    #region Singleton
    public static SaveManager instance;
    /// <summary>
    /// Description: Setup Singleton.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void Awake()
    {
        // temp
        if (instance != null)
        {
            Debug.Log("More than one Save Manager exists!");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion Singleton
    #region Callbacks
    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    #endregion Callbacks
    /// <summary>
    /// Description: For preload scene.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void Start()
    {
        ReturnToMain();
    }

    public int saveToUse = 4;
    public List<SaveContainer> saves;

    /// <summary>
    /// Description: Loads game scene with current save state information.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <param name="_save">Save to use - 4 is new game</param>
    public void LoadScene(int _save)
    {
        saveToUse = _save;

        SceneManager.LoadScene(2);
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Description: Returns to main scene, resets default save to use to new game.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void ReturnToMain()
    {
        saveToUse = 4;

        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Description: Loads information from save after loading level.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <param name="scene">Scene Loaded</param>
    /// <param name="mode">Mode Loaded</param>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        uniqueID = 0;
        //Debug.Log("Loading with " + saveToUse);
        Invoke("Load", 0.1f);
    }

    /// <summary>
    /// Description: Save current game state to selected save slot.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <param name="_save"></param>
    public void SaveElement(SaveMe _save)
    {
        saves[saveToUse].Save(_save);
    }

    /// <summary>
    /// Description: Loads SaveInfo.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <param name="_save">Save to Use</param>
    /// <returns></returns>
    public SaveInfo LoadElement(SaveInfo _save)
    {
        for (int i = 0; i < saves[saveToUse].saveStates.Count; i++)
        {
            if (saves[saveToUse].saveStates[i].uniqueID == _save.uniqueID)
            {
                return (saves[saveToUse].saveStates[i]);
            }
        }

        return (_save);
    }

    /// <summary>
    /// Description: Writes screenshot for save slot use.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    void TakeScreenShot()
    {
        String imagePath = Application.persistentDataPath + "/Resources/" + saves[saveToUse].saveName + ".png";
        StartCoroutine(captureScreenshot(imagePath));
    }

    /// <summary>
    /// Description: Capture Component from above.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <param name="imagePath">Path to save Screen Shot</param>
    /// <returns></returns>
    IEnumerator captureScreenshot(String imagePath)
    {
        // Start after frame draw
        yield return new WaitForEndOfFrame();
        // Create Tex2D to received image
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        //Convert to png
        byte[] imageBytes = screenImage.EncodeToPNG();
        //Save image to file
        System.IO.File.WriteAllBytes(imagePath, imageBytes);

        // Load bytes
        byte[] bytes = System.IO.File.ReadAllBytes(imagePath);

        Texture2D tex2D = new Texture2D(screenImage.width, screenImage.height);
        tex2D.LoadImage(bytes);
        saves[saveToUse].texture = tex2D;        
    }

    public event Action save;
    /// <summary>
    /// Description: Tells all saveinfo components to save information and takes screenshot.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void Save()
    {
        PauseMenuCanvasController.instance.Pause(InputState.KEYDOWN);
        TakeScreenShot();      

        if (save != null)
        {
            save();
        }
    }

    public event Action load;
    /// <summary>
    /// Description: Tells all saveinfo components to load information.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void Load()
    {
        if (load != null)
        {
            load();
            CallbackHandler.instance.FadeIn();
        }
    }

    int uniqueID = 0;
    /// <summary>
    /// Description: Used for UI setup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    /// <returns></returns>
    public int GetID()
    {
        uniqueID++;
        return uniqueID;
    }
}
