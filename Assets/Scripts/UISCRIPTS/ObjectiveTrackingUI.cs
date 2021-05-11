using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrackingUI : MonoBehaviour
{
    public GameObject questUIPrefab;
    Animator animator;

    List<ObjectiveUIElement> objs = new List<ObjectiveUIElement>();


    private void Start()
    {
        animator = GetComponent<Animator>();

        CallbackHandler.instance.startTrackingQuest += StartTrackingQuest;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.startTrackingQuest -= StartTrackingQuest;
    }

    public void StartTrackingQuest(Quest _quest)
    {
        ObjectiveUIElement temp = Instantiate(questUIPrefab, this.transform).GetComponent<ObjectiveUIElement>();
        temp.Setup(_quest);
        objs.Add(temp);
    }

    private void Update()
    {
        // Change this to input listener
        if (Input.GetKey(KeyCode.Tab))
        {
            TabDisplay();
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            HideQuest();
        }

        if (objs.Count <= 0)
            return;

        Vector3 objLoc = objs[0].GetQuestLocation();
        Vector3 newPoint = Camera.main.WorldToScreenPoint(objLoc);
        objMarker.transform.position = newPoint;

        float dist = Mathf.RoundToInt(Vector3.Distance(EntityManager.instance.player.transform.position, objLoc));

        distanceText.SetText(dist.ToString() + "m");

        objMarker.SetActive(newPoint.z > 0);
        distanceText.enabled = newPoint.z > 0;
    }

    public TMPro.TextMeshProUGUI distanceText;
    public GameObject objMarker;
    float timer;
    public void TabDisplay()
    {
        ShowQuest();
        timer = 5.0f;
    }

    public void ShowQuest()
    {
        animator.SetBool("Show", true);
    }

    public void HideQuest()
    {
        animator.SetBool("Show", false);
    }
}
