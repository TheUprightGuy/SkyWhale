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
    private void Awake()
    {
        // temp
        if (instance != null)
        {
            Debug.LogError("More than one Save Manager exists!");
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion Singleton

    public int saveToUse = 0;
    public List<SaveContainer> saves;

    public void LoadScene(int _save)
    {
        saveToUse = _save;

        SceneManager.LoadScene(1);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Invoke("Load", 0.1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }
    }

    public void SaveElement(SaveMe _save)
    {
        saves[saveToUse].Save(_save);
    }

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


    void TakeScreenShot()
    {
        String imagePath = Application.persistentDataPath + "/Resources/" + saves[saveToUse].saveName + ".png";
        StartCoroutine(captureScreenshot(imagePath));
    }

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
    public void Save()
    {
        TakeScreenShot();      

        if (save != null)
        {
            save();
        }
    }
    public event Action load;
    public void Load()
    {
        if (load != null)
        {
            load();
        }      
    }

    int uniqueID = 0;
    public int GetID()
    {
        uniqueID++;
        return uniqueID;
    }
}
