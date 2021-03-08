using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject landingButton;
    public GameObject dialogueUI;
    public EndScreen endScreen;
    RectTransform scale;
    float scalar = 0.0f;
    float maxScalar = 0.2f;
    float timer = 0.0f;
    [HideInInspector] public bool showMe = false;
    [HideInInspector] public bool hideMe = false;
    [HideInInspector] public bool ready = false;

    public bool showing;

    // Start is called before the first frame update
    void Start()
    {
        WhaleHandler.instance.landingTooltip += LandingToggle;
        CallbackHandler.instance.toggleText += ToggleText;
        CallbackHandler.instance.showEndScreen += EndScreen;
        scale = landingButton.GetComponent<RectTransform>();
        Invoke("ToggleText", 0.1f);
        //ToggleText();
    }

    private void OnDestroy()
    {
        WhaleHandler.instance.landingTooltip -= LandingToggle;
        CallbackHandler.instance.toggleText -= ToggleText;
        CallbackHandler.instance.showEndScreen -= EndScreen;
    }

    public void LandingToggle(bool _toggle)
    {
        if (showing != _toggle)
        {
            if (_toggle)
            {
                showMe = true;
                hideMe = false;
            }
            else
            {
                hideMe = true;
                showMe = false;
            }
            showing = _toggle;
        }
    }

    private void Update()
    {
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

    public void ShowMe()
    {
        scalar += Time.deltaTime * 2;
        scale.localScale = new Vector3(scalar, scalar, 1.0f);
        if (scalar >= 1.0f)
        {
            scalar = 1.0f;
            showMe = false;
            ready = true;
            timer = scalar - 0.8f;
        }
    }

    public void HideMe()
    {
        scalar -= Time.deltaTime * 2;
        scale.localScale = new Vector3(scalar, scalar, 1.0f);
        if (scalar <= 0)
        {
            scalar = 0;
            scale.localScale = new Vector3(scalar, scalar, 1.0f);
            hideMe = false;
            ready = false;
        }
    }

    public void PingPong()
    {
        scalar = Mathf.PingPong(timer, maxScalar) + 0.8f;
        timer += Time.deltaTime / 4;

        scale.localScale = new Vector3(scalar, scalar, 1.0f);
    }

    public void ToggleText()
    {
        dialogueUI.SetActive(!dialogueUI.activeSelf);
    }

    public void EndScreen()
    {
        endScreen.gameObject.SetActive(true);
        endScreen.Show();
    }
}
