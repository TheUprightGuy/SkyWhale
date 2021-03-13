using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUIPrefab : MonoBehaviour
{
    public GameObject titlePrefab;
    public GameObject keybindPrefab;

    public void CreateUIElements(string _name, List<InputListener> _data)
    {
        Instantiate(titlePrefab, transform).GetComponent<TMPro.TextMeshProUGUI>().SetText(_name);

        foreach (InputListener n in _data)
        {
            Keybind temp = Instantiate(keybindPrefab, transform).GetComponentInChildren<Keybind>();
            temp.Setup(n);
        }
    }
}
