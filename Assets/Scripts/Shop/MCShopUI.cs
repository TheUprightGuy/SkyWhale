using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MCShopUI : MonoBehaviour
{
    #region Setup&Callbacks
    public static MCShopUI instance;

    public Animator animator;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one ShopUI exists!");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CallbackHandler.instance.showDetails += ShowDetails;
        CallbackHandler.instance.hideDetails += HideDetails;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.showDetails -= ShowDetails;
        CallbackHandler.instance.hideDetails -= HideDetails;
    }
    #endregion Setup&Callbacks

    public Item item;
    public ShopItem shopItem;
    public bool shopWindowOpen;

    public TMPro.TextMeshProUGUI cost;
    public TMPro.TextMeshProUGUI name;
    public TMPro.TextMeshProUGUI description;
    public Image image;

    public TMPro.TextMeshProUGUI resourceCount;
    public Button buyButton;

    bool tutMsg;
    Color valid = new Color(1.0f, 1.0f, 1.0f, 0.2f);
    Color invalid = new Color(1.0f, 0.0f, 0.0f, 0.2f);


    public void ShowDetails(Item _item, ShopItem _shopItem)
    {
        item = _item;
        shopItem = _shopItem;
        animator.ResetTrigger("Show");
        if (shopWindowOpen)
        {
            HideDetails();
            animator.SetTrigger("Show");
        }
        else
        {
            UpdateItemDetails();
            animator.SetTrigger("Show");
        }
    }

    public void UpdateItemDetails()
    {
        cost.SetText(item.cost.ToString() + "x");
        name.SetText(item.name);
        description.SetText(item.description);
        image.sprite = item.image;

        buyButton.GetComponent<Image>().color = (ResourceDisplayScript.instance.supplies < item.cost) ? invalid : valid;
    }

    public void HideDetails()
    {
        if (shopWindowOpen)
        {
            animator.ResetTrigger("Hide");
            animator.SetTrigger("Hide");
        }
    }

    public void Open()
    {
        shopWindowOpen = true;
        UpdateItemDetails();
    }
    public void Closed()
    {
        shopWindowOpen = false;
        UpdateItemDetails();
    }

    public void BuyItem()
    {
        switch (item.type)
        {
            case ItemType.Lantern:
            {
                if (ResourceDisplayScript.instance.SpendSupplies(2))
                {
                    if (!tutMsg)
                    {
                        TutorialMessage cloudTutorial = new TutorialMessage();
                        cloudTutorial.message = "Hey, I've never noticed that island before.";
                        cloudTutorial.timeout = 5.0f;
                        cloudTutorial.key = KeyCode.E;

                        // CHANGE OBJECTIVE HERE
                        MapHandler.instance.SetActiveObjective(2);

                        CallbackHandler.instance.AddMessage(cloudTutorial);
                        CallbackHandler.instance.NextMessage();
                        tutMsg = true;
                    }

                    CallbackHandler.instance.ToggleLamp(true);
                    Destroy(shopItem.gameObject);
                }
                break;
            }
            case ItemType.Provisions:
            {
                if (ResourceDisplayScript.instance.SpendSupplies(1))
                {
                    ResourceDisplayScript.instance.AddProvisions(1);
                }
                break;
            }
            case ItemType.Saddle:
            {
                if (ResourceDisplayScript.instance.SpendSupplies(2))
                {
                    CallbackHandler.instance.UnlockSaddle();
                    Destroy(shopItem.gameObject);
                }
                break;
            }
        }

        HideDetails();
        CallbackHandler.instance.BuyItem();
    }
}
