using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleCamp : MonoBehaviour
{
    public List<GameObject> objects;

    // Start is called before the first frame update
    void Start()
    {
        ToggleObjects(false);
        EventManager.StartListening("ReturnWife", ToggleCampOn);
    }

    public void ToggleCampOn()
    {
        ToggleObjects(true);
        EventManager.StopListening("ReturnWife", ToggleCampOn);
    }

    public void ToggleObjects(bool _toggle)
    {
        foreach (GameObject n in objects)
        {
            n.SetActive(_toggle);
        }
    }
}
