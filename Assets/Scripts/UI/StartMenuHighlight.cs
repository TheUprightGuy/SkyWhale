using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartMenuHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject highlight;

    // Start is called before the first frame update
    void Start()
    {
        highlight = transform.GetChild(0).gameObject;
        ToggleHighlight(false);
    }

    void ToggleHighlight(bool _toggle)
    {
        highlight.SetActive(_toggle);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ToggleHighlight(true);
        transform.localScale = new Vector3(1.05f, 1.05f, 1.0f);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ToggleHighlight(false);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
