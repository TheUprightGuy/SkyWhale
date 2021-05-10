using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrackingUI : MonoBehaviour
{
    public GameObject questUIPrefab;

    private void Start()
    {
        CallbackHandler.instance.startTrackingQuest += StartTrackingQuest;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.startTrackingQuest -= StartTrackingQuest;
    }

    public void StartTrackingQuest(Quest _quest)
    {
        Instantiate(questUIPrefab, this.transform).GetComponent<ObjectiveUIElement>().Setup(_quest);
    }
}
