using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverHighlight : MonoBehaviour
{
    #region Singleton
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
    #endregion Singleton

    public LayerMask layerMask;
    public PuzzleSwitch highlightedSwitch;

    // Update is called once per frame
    void Update()
    {
        highlightedSwitch = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

        foreach (RaycastHit n in hits)
        {
            PuzzleSwitch tempSwitch = n.collider.gameObject.GetComponent<PuzzleSwitch>();

            if (tempSwitch)
            {
                highlightedSwitch = tempSwitch;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (highlightedSwitch)
            {
                highlightedSwitch.Use();
            }
        }
    }
}
