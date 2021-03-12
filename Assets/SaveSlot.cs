using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public SaveContainer save;

    public TMPro.TextMeshProUGUI saveName;
    public TMPro.TextMeshProUGUI timePlayed;
    public Image renderImage;

    private void Awake()
    {
        saveName.SetText(save.saveName);
        timePlayed.SetText(save.timePlayed.ToString());
        // render image
    }
}
