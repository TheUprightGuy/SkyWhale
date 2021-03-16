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
