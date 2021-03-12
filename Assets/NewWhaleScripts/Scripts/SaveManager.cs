using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private void Awake()
    {
        // temp
        instance = this;
    }

    public SaveContainer saves;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Load();
        }
    }

    public void SaveElement(SaveMe _save)
    {
        saves.Save(_save);
    }

    public SaveInfo LoadElement(SaveInfo _save)
    {
        for (int i = 0; i < saves.saveStates.Count; i++)
        {
            if (saves.saveStates[i].uniqueID == _save.uniqueID)
            {
                return (saves.saveStates[i]);
            }
        }

        return (_save);
    }


    public event Action save;
    public void Save()
    {
        ScreenCapture.CaptureScreenshotIntoRenderTexture(saves.screenRender);   

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
}
