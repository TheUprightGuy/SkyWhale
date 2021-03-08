using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbackHandler : MonoBehaviour
{
    #region Singleton
    public static CallbackHandler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Callback Handler exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            OnAwake();
        }
    }
    #endregion Singleton
    #region Setup
    public WhaleInfo whaleInfo;
    private void OnAwake()
    {
        whaleInfo.ResetOnPlay();
    }
    #endregion Setup

    private void Update()
    {
        whaleInfo.UpdateHunger(Time.deltaTime / 2);
    }


    public event Action<GameObject> setQuestObjective;
    public void SetQuestObjective(GameObject _target)
    {
        if (setQuestObjective != null)
        {
            setQuestObjective(_target);
        }
    }

    public event Action cutCam;
    public void CutCam()
    {
        if (cutCam != null)
        {
            cutCam();
        }
    }

    public event Action lerpCam;
    public void LerpCam()
    {
        if (lerpCam != null)
        {
            lerpCam();
        }
    }

    #region Puzzles
    public event Action openDoors;
    public void OpenDoors()
    {
        if (openDoors != null)
        {
            openDoors();
        }
    }
    #endregion Puzzles
    #region ResourcePopups
    public event Action<string> supplyPopUp;
    public void SupplyPopUp(string _supplies)
    {
        if (supplyPopUp != null)
        {
            supplyPopUp(_supplies);
        }
    }

    public event Action<string> provisionPopUp;
    public void ProvisionPopUp(string _provisions)
    {
        if (supplyPopUp != null)
        {
            provisionPopUp(_provisions);
        }
    }
    #endregion ResourcePopups
    #region Shop
    public bool inShopRange;

    public event Action<Item, ShopItem> showDetails;
    public void ShowDetails(Item _item, ShopItem _shopItem)
    {
        if (showDetails != null)
        {
            showDetails(_item, _shopItem);
        }
    }

    public event Action<bool> toggleShop;
    public void ToggleShop(bool _toggle)
    {
        if (toggleShop != null)
        {
            toggleShop(_toggle);
        }
    }

    public event Action hideDetails;
    public void HideDetails()
    {
        if (hideDetails != null)
        {
            hideDetails();
        }
    }

    public event Action<bool> toggleLamp;
    public void ToggleLamp(bool _toggle)
    {
        if (toggleLamp != null)
        {
            toggleLamp(_toggle);
        }
    }

    public event Action unlockSaddle;
    public void UnlockSaddle()
    {
        if (unlockSaddle != null)
        {
            unlockSaddle();
        }
    }

    public event Action buyItem;
    public void BuyItem()
    {
        if (buyItem != null)
        {
            buyItem();
        }
    }
    #endregion Shop
    #region Interaction
    public event Action<string, string> setDialogue;
    public void SetDialogue(string _speaker, string _dialogue)
    {
        if (setDialogue != null)
        {
            ToggleText();
            setDialogue(_speaker, _dialogue);
        }
    }

    public event Action toggleText;
    public void ToggleText()
    {
        if (toggleText != null)
        {
            toggleText();
        }
    }

    public event Action interact;
    public void Interact()
    {
        if (interact != null)
        {
            interact();
        }
    }
    #endregion Interaction

    #region Tutorial
    public event Action startTutorial;
    public void StartTutorial()
    {
        if (startTutorial != null)
        {
            startTutorial();
        }
    }
    public event Action<TutorialMessage> setTutorialMessage;
    public void SetTutorialMessage(TutorialMessage _msg)
    {
        if (setTutorialMessage != null)
        {
            ToggleText();
            setTutorialMessage(_msg);
        }
    }
    public event Action nextMessage;
    public void NextMessage()
    {
        if (nextMessage != null)
        {
            nextMessage();
        }
    }
    public event Action<TutorialMessage> addMessage;
    public void AddMessage(TutorialMessage _msg)
    {
        if (addMessage != null)
        {
            addMessage(_msg);
        }
    }
    public event Action addCollectableMan;
    public void AddCollectableMan()
    {
        if (addCollectableMan != null)
        {
            addCollectableMan();
        }
    }

    public event Action showEndScreen;
    public void ShowEndScreen()
    {
        if (showEndScreen != null)
        {
            showEndScreen();
        }
    }

    #endregion Tutorial
}
