/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   SaveSlot.cs
  Description :   UI Element to display player save. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    SAVE,
    LOAD
}


public class SaveSlot : MonoBehaviour, IPointerDownHandler
{
    [Header("Save Container")]
    public int saveSlot;
    SaveContainer save;
    [Header("Required Fields")]
    public SlotType slotType;
    public TMPro.TextMeshProUGUI saveName;
    public TMPro.TextMeshProUGUI timePlayed;
    public Image renderImage;

    /// <summary>
    /// Description: Sets up slot to reflect content on the save.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Start()
    {
        save = SaveManager.instance.saves[saveSlot];
        if (save || save.saveName.Equals(""))
        {
            UpdateSlot();
        }
        else
        {
            //renderImage.enabled = false;
            if (slotType == SlotType.LOAD)
            {
                GetComponent<Image>().raycastTarget = false;
            }
            saveName.SetText("Empty");
        }
    }

    /// <summary>
    /// Description: Updates the slot with content from the save.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void UpdateSlot()
    {
        if (!save || save.saveName.Equals(""))
        {
            //renderImage.enabled = false;
            if (slotType == SlotType.LOAD)
            {
                GetComponent<Image>().raycastTarget = false;
            }
            saveName.SetText("Empty");
            return;
        }

        GetComponent<Image>().raycastTarget = true;
        saveName.SetText(save.saveName);
        timePlayed.SetText(save.timePlayed.ToString());

        renderImage.enabled = true;
        string imagePath = Application.persistentDataPath + "/Resources/" + save.saveName + ".png";
        // Load bytes
        if (System.IO.File.Exists(imagePath))
        { 
            byte[] bytes = System.IO.File.ReadAllBytes(imagePath);
  
            // Create texture of correct size then load bytes
            save.texture = new Texture2D(Screen.width, Screen.height);
            save.texture.LoadImage(bytes);
            // Create sprite to render on screen
            renderImage.sprite = Sprite.Create(save.texture, new Rect(0, 0, save.texture.width, save.texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    /// <summary>
    /// Description: Saves current world state, or loads depending on menu.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="eventData">Unused</param>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        switch (slotType)
        {
            case SlotType.LOAD:
            {
                SaveManager.instance.saveToUse = saveSlot;
                SaveManager.instance.LoadScene(saveSlot);
                break;
            }
            case SlotType.SAVE:
            {
                if (save.saveName.Length == 0)
                {
                    save.saveName = saveSlot.ToString();
                }

                SaveManager.instance.saveToUse = saveSlot;
                SaveManager.instance.Save();
                Invoke("UpdateSlot", 0.1f);
                break;
            }
        }
    }
}
