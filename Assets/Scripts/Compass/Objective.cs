using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetObj", 0.1f);
    }

    public void SetObj()
    {
        CallbackHandler.instance.SetQuestObjective(gameObject);
    }
}
