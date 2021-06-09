using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CollectableInfo
{
    public CollectableInfo(string _name, bool collected = false)
    {
        Name = _name;
        Collected = collected;
    }

    [HideInInspector]
    public bool Collected;
    [Space]
    public string Name;
    public string Description;
    public Sprite UISprite = null;

    [HideInInspector]
    public GameObject UIObject = null;
    [HideInInspector]
    public Text UIText = null;
    [HideInInspector]
    public Image UIImage = null;

}


public class CollectableHandler : MonoBehaviour
{
   
    public GameObject UIPrefab;
    public GameObject UIParent;


    public GameObject ShowcasePanel;
    public GameObject CollectableTut;

    Text lpTitle;
    Image lpImage;
    Text lpDesc;

    CollectablesTrigger[] collectablesTriggers;
    // Start is called before the first frame update
    void Awake()
    {
        collectablesTriggers = GetComponentsInChildren<CollectablesTrigger>();
        SetUpUI();
        AssignListeners();

        if (ShowcasePanel != null)
        {
            Text[] texts = ShowcasePanel.GetComponentsInChildren<Text>();
            lpTitle = texts[0];
            lpDesc = texts[1];

            lpImage = ShowcasePanel.GetComponentsInChildren<Image>()[1];
            ShowcasePanel.SetActive(false);

        }

        if (CollectableTut != null)
        {
            CollectableTut.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CollectableTut != null && CollectableTut.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                CollectableTut.SetActive(false);
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
            collectablesTriggers[i].Collectable.UIText = NewPanel.GetComponentInChildren<Text>();

           
        }
    }

    public void AssignListeners()
    {
        for (int i = 0; i < collectablesTriggers.Length; i++)
        {
            Button butt = collectablesTriggers[i].Collectable.UIObject.GetComponentInChildren<Button>();
            butt.onClick.AddListener(() => TriggerLargePanel(i));
        }
    }

    bool firstCollected = false;
    void CollectionPrompt()
    {
        CollectableTut.SetActive(true);
    }


    public void ItemCollected(int _index)
    {
        if (!firstCollected)
        {
            firstCollected = true;
            CollectionPrompt();
        }

        collectablesTriggers[_index].Collectable.Collected = true;
        collectablesTriggers[_index].Collectable.UIText.text = collectablesTriggers[_index].Collectable.Name;
        collectablesTriggers[_index].Collectable.UIImage.sprite = collectablesTriggers[_index].Collectable.UISprite;
    }

    public void TriggerLargePanel(int _index)
    {

        if (!collectablesTriggers[_index - 1].Collectable.Collected)
        {
            return;
        }

        Debug.Log("Collectable UI " + collectablesTriggers[_index - 1].Collectable.Name + " clicked");
        lpImage.sprite = collectablesTriggers[_index - 1].Collectable.UISprite;
        lpTitle.text = collectablesTriggers[_index - 1].Collectable.Name;
        lpDesc.text = collectablesTriggers[_index - 1].Collectable.Description;

        ShowcasePanel.SetActive(true);

    }
}
