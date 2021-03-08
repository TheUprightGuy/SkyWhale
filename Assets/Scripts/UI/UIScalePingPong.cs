using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScalePingPong : MonoBehaviour
{
    [Header("Setup Fields")]
    public Thought id;
    
    // Local Variables
    RectTransform scale;
    float scalar = 0.0f;
    float maxScalar = 1.0f;
    float timer = 0.0f;
    float upTime = 0.0f;
    [HideInInspector] public bool showMe = false;
    [HideInInspector] public bool hideMe = false;
    [HideInInspector] public bool ready = false;

    #region Callbacks
    private void Start()
    {
        scale = GetComponent<RectTransform>();
        ThoughtsScript.instance.showThought += ShowThought;
    }
    private void OnDestroy()
    {
        ThoughtsScript.instance.showThought -= ShowThought;
    }
    #endregion Callbacks

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);    

        if (showMe)
        {
            ShowMe();
        }
        else if (hideMe)
        {
            HideMe();
        }
        else if (ready)
        {
            PingPong();
        }
    }

    public void ShowThought(Thought _id, bool _toggle)
    {
        if (id == _id && _toggle)
        {
            if (_toggle)
            {
                showMe = true;
                upTime = 30.0f;
            }
        }
        else
        {
            hideMe = true;
        }
    }

    public void ShowMe()
    {
        scalar += Time.deltaTime * 4;
        scale.localScale = new Vector3(-scalar, scalar, 1.0f);
        if (scalar >= maxScalar + 2.5f)
        {
            scalar = maxScalar + 2.5f;
            showMe = false;
            ready = true;
            timer = maxScalar * 2;
        }
    }

    public void HideMe()
    {
        scalar -= Time.deltaTime * 4;
        scale.localScale = new Vector3(-scalar, scalar, 1.0f);
        if (scalar <= 0)
        {
            scalar = 0;
            scale.localScale = new Vector3(-scalar, scalar, 1.0f);
            hideMe = false;
            ready = false;
        }
    }

    public void PingPong()
    {
        scalar = Mathf.PingPong(timer / 2, maxScalar) + 2.5f;
        timer += Time.deltaTime;

        scale.localScale = new Vector3(-scalar, scalar, 1.0f);

        upTime -= Time.deltaTime;
        if (upTime <= 0.0f)
        {
            hideMe = true;
        }
    }
}
