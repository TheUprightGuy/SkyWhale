using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverHighlight : MonoBehaviour
{
    public static MouseOverHighlight instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one MouseOverHighlight Exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public LayerMask layerMask;
    public PuzzleSwitch highlightedSwitch;
    public ShopItem highlightedShopItem;
    public ShopOwner shopOwner;
    public CollectableMan collectableMan;
    public CirclePuzzle circle;


    static bool tutMessage = false;

    // Update is called once per frame
    void Update()
    {
        highlightedSwitch = null;
        highlightedShopItem = null;
        shopOwner = null;
        collectableMan = null;
        circle = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

        foreach (RaycastHit n in hits)
        {
            PuzzleSwitch tempSwitch = n.collider.gameObject.GetComponent<PuzzleSwitch>();
            ShopItem tempItem = null;
            if (CallbackHandler.instance.inShopRange)
            {
                if (!tutMessage)
                {
                    TutorialMessage shopTutorial = new TutorialMessage();
                    shopTutorial.message = "Use your mouse to select and click items.";
                    shopTutorial.timeout = 5.0f;
                    shopTutorial.key = KeyCode.E;
                    CallbackHandler.instance.AddMessage(shopTutorial);
                    CallbackHandler.instance.NextMessage();

                    //PopUpHandler.instance.QueuePopUp("Use your mouse to select and click items", 7);
                }
                tutMessage = true;

                tempItem = n.collider.gameObject.GetComponent<ShopItem>();

            }
            if (!n.collider.isTrigger && n.collider.gameObject.GetComponent<ShopOwner>())
            {
                shopOwner = n.collider.gameObject.GetComponent<ShopOwner>();
            }
            if (!n.collider.isTrigger && n.collider.gameObject.GetComponent<CollectableMan>())
            {
                collectableMan = n.collider.gameObject.GetComponent<CollectableMan>();
            }
            if (!n.collider.isTrigger && n.collider.gameObject.GetComponent<CirclePuzzle>())
            {
                circle = n.collider.gameObject.GetComponent<CirclePuzzle>();
            }
            if (tempSwitch)
            {
                highlightedSwitch = tempSwitch;
            }
            if (tempItem)
            {
                highlightedShopItem = tempItem;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (highlightedSwitch)
            {
                highlightedSwitch.Use();
                CallbackHandler.instance.Interact();
            }
            if (highlightedShopItem)
            {
                highlightedShopItem.ShowUI();
                CallbackHandler.instance.Interact();
            }
            if (shopOwner)
            {
                shopOwner.Talk();
            }
            if (collectableMan)
            {
                collectableMan.Talk();
            }
            if (circle)
            {
                circle.Rotate();
            }
        }
    }
}
