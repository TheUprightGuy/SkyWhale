using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMe : MonoBehaviour
{
    [HideInInspector] public SaveInfo info;

    private void Start()
    {
        info.uniqueID = transform.GetInstanceID();

        SaveManager.instance.save += SavePosition;
        SaveManager.instance.load += LoadPosition;
    }
    private void OnDestroy()
    {
        SaveManager.instance.save -= SavePosition;
        SaveManager.instance.load -= LoadPosition;
    }

    public void SavePosition()
    {
        info.position = transform.position;
        SaveManager.instance.SaveElement(this);
    }

    public void LoadPosition()
    {
        if (SaveManager.instance.LoadElement(info) != null)
        {
            info = SaveManager.instance.LoadElement(info);
            transform.position = info.position;
        }
    }
}
