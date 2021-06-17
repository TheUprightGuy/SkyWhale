using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CollectableInfo
{
    public CollectableInfo(string _name, bool collected = false)
    {
        Name = _name;
        Collected = collected;
    }

    //[HideInInspector]
    public bool Collected;
    [Space]
    public string Name;
    [TextArea]
    public string Description;
    public Sprite UISprite = null;

    [HideInInspector]
    public GameObject UIObject = null;
    [HideInInspector]
    public TextMeshProUGUI UIText = null;
    [HideInInspector]
    public Image UIImage = null;

}


public class CollectableHandler : MonoBehaviour
{
   
    public GameObject UIPrefab;
    public GameObject UIParent;

    public GameObject ScrollObj;

    public GameObject ShowcasePanel;
    public GameObject CollectableTut;
    public GameObject RewardPrompts;

    public GameObject NormMenu;
    public GameObject RewardMenu;
    TextMeshProUGUI lpTitle;
    Image lpImage;
    TextMeshProUGUI lpDesc;

    CollectablesTrigger[] collectablesTriggers;
    // Start is called before the first frame update
    void Awake()
    {
        collectablesTriggers = GetComponentsInChildren<CollectablesTrigger>();
        SetUpUI();
        AssignListeners();

        if (ShowcasePanel != null)
        {
            TextMeshProUGUI[] texts = ShowcasePanel.GetComponentsInChildren<TextMeshProUGUI>();
            lpTitle = texts[0];
            lpDesc = texts[1];

            lpImage = ShowcasePanel.GetComponentsInChildren<Image>()[1];
            ShowcasePanel.SetActive(false);

        }

        if (CollectableTut != null)
        {
            CollectableTut.SetActive(false);
        }

        if (RewardPrompts != null)
        {
            RewardPrompts.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CollectableTut != null && CollectableTut.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CallbackHandler.instance.Pause(false);
                CollectableTut.SetActive(false);
                RewardPrompts.SetActive(false);
            }
        }
    }

    public void SetUpUI()
    {
        int tc = collectablesTriggers.Length;

        for (int i = 0; i < tc; i++) //for each one until amount in childcount
        {
            //CollectableInfo newInfo = collectablesTriggers[i].Collectable;
            collectablesTriggers[i].handlerIndex = i;

            GameObject NewPanel = Instantiate(UIPrefab, UIParent.transform);
            collectablesTriggers[i].Collectable.UIObject = NewPanel;
            collectablesTriggers[i].Collectable.UIImage = NewPanel.GetComponentsInChildren<Image>()[1];
            collectablesTriggers[i].Collectable.UIText = NewPanel.GetComponentInChildren<TextMeshProUGUI>();

        }
    }

    public void AssignListeners()
    {
        for (int i = 0; i < collectablesTriggers.Length; i++)
        {
            int test = i;
            Button butt = collectablesTriggers[i].Collectable.UIObject.GetComponentInChildren<Button>();
            butt.onClick.AddListener(delegate {TriggerLargePanel(test);});
        }
    }

    bool firstCollected = false;
    void CollectionPrompt()
    {
        CollectableTut.SetActive(true);
    }

    int numCollected = 0;
    public void ItemCollected(int _index)
    {
        if (!firstCollected)
        {
            firstCollected = true;
            CallbackHandler.instance.Pause(true);
            CollectionPrompt();
        }

        collectablesTriggers[_index].Collectable.Collected = true;
        collectablesTriggers[_index].Collectable.UIText.text = collectablesTriggers[_index].Collectable.Name;
        collectablesTriggers[_index].Collectable.UIImage.sprite = collectablesTriggers[_index].Collectable.UISprite;

        
        //JACOB add sounds here pls thx :)
        numCollected++;

        if (numCollected >= collectablesTriggers.Length)
        {
            AllCollected();
        }
    }

    public void AllCollected()
    {
        //Good job
        RewardPrompts.SetActive(true);
        NormMenu.SetActive(false); ;
        RewardMenu.SetActive(true); ;
    }

    string codeinput = "";
    const string coderequired = "7567";
    public void shhh(int input)
    {
        codeinput += input.ToString();

        if (codeinput == coderequired)
        {
            codeinput = "";
            firstCollected = true; //Force turn off the tut
            for (int i = 0; i < collectablesTriggers.Length; i++)
            {
                ItemCollected(i);
            }
        }
        else if (codeinput.Length >= coderequired.Length)
        {
            codeinput = "";
        }
    }
    public void TriggerLargePanel(int _index)
    {

        if (!collectablesTriggers[_index].Collectable.Collected)
        {
            return;
        }
        if (ScrollObj!=null)
        {
            ScrollObj.SetActive(false);
        }
        Debug.Log("Collectable UI " + collectablesTriggers[_index].Collectable.Name + " clicked");
        lpImage.sprite = collectablesTriggers[_index].Collectable.UISprite;
        lpTitle.text = collectablesTriggers[_index ].Collectable.Name;
        lpDesc.text = collectablesTriggers[_index].Collectable.Description;

        ShowcasePanel.SetActive(true);

    }
}
