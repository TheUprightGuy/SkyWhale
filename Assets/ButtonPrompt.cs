using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPrompt : MonoBehaviour
{
    Animator animator;
    TMPro.TextMeshProUGUI text;
    bool show;
    string currentAction;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Show", show);
    }

    public void Show(bool _toggle, KeyCode _key, string _action, string _subName)
    {
        show = _toggle;
        currentAction = _action;
        text.SetText("Press " + "<color=#FFE900>'" + _key.ToString() + "'</color> to " + (_subName == "" ? _action : _subName) + ".");
    }

    public void Hide(string _action)
    {
        if (currentAction == _action)
            show = false;
    }
}
