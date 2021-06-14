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
        EventManager.StartListening("ReturnWife2", ToggleCampOn);
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleCampOn();
        }
    }*/

    public void ToggleCampOn()
    {
        ToggleObjects(true);
        CallbackHandler.instance.SpawnCollectableIsland();
        EventManager.StopListening("ReturnWife2", ToggleCampOn);
    }

    public void ToggleObjects(bool _toggle)
    {
        foreach (GameObject n in objects)
        {
            n.SetActive(_toggle);
        }
    }
}
