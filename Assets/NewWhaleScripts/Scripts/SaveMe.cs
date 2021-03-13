using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMe : MonoBehaviour
{
    [HideInInspector] public SaveInfo info;

    private void Start()
    {       
        SaveManager.instance.save += SavePosition;
        SaveManager.instance.load += LoadPosition;

        info.uniqueID = SaveManager.instance.GetID();
    }
    private void OnDestroy()
    {
        SaveManager.instance.save -= SavePosition;
        SaveManager.instance.load -= LoadPosition;
    }

    public void SavePosition()
    {
        info.position = transform.position;
        info.rotation = transform.rotation;
        SaveManager.instance.SaveElement(this);
    }

    public void LoadPosition()
    {
        if (SaveManager.instance.LoadElement(info) != null)
        {
            info = SaveManager.instance.LoadElement(info);
            transform.position = info.position;
            transform.rotation = info.rotation;
        }
    }
}
