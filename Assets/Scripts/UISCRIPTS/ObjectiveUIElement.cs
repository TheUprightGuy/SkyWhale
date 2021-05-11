using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveUIElement : MonoBehaviour
{
    public Quest currentQuest;

    public TMPro.TextMeshProUGUI questName;
    public TMPro.TextMeshProUGUI questDesc;

    private void Start()
    {
        CallbackHandler.instance.updateObjectives += UpdateObjective;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.updateObjectives -= UpdateObjective;
    }

    public void DisplayObjective()
    {
        questName.SetText(currentQuest.questName);
        questDesc.SetText(currentQuest.objectives[currentQuest.index].objDesc);
    }

    public void UpdateObjective()
    {
        if (currentQuest.FinishedQuest())
        {
            Destroy(this.gameObject);
            return;
        }

        questDesc.SetText(currentQuest.objectives[currentQuest.index].objDesc);
    }

    public void Setup(Quest _quest)
    {
        currentQuest = _quest;
        DisplayObjective();
    }

    public Vector3 GetQuestLocation()
    {
        return currentQuest.objectives[currentQuest.index].location;
    }
}
