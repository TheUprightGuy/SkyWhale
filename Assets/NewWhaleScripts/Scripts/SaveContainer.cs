using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveInfo
{
    public int uniqueID;
    public Vector3 position;
}


[CreateAssetMenu(fileName = "Container", menuName = "Data/Container", order = 1)]
public class SaveContainer : ScriptableObject
{
    public RenderTexture screenRender;
    public string saveName;
    public int timePlayed;


    [HideInInspector] public List<SaveInfo> saveStates;

    public void Save(SaveMe _save)
    {
        for (int i = 0; i < saveStates.Count; i++)
        {
            if (saveStates[i].uniqueID == _save.info.uniqueID)
            {
                saveStates.RemoveAt(i);
            }
        }
        saveStates.Add(_save.info);
    }
}
