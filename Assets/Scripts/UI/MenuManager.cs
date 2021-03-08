using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Audio;

public class MenuManager : MonoBehaviour
{
    private GameState _gameState;
    public GameObject menuCanvas;
    public GameObject playButton;

    private void Awake()
    {
        playButton.GetComponent<Button>().onClick.AddListener(OnPlayPressed);
    }

    void Start()
    {
        _gameState = EventHandler.instance.gameState;
        //On start menu will always be open except for testing
        _gameState.inMenu = true;
        EventHandler.instance.menuOpened += OnMenuOpened;
        Invoke("GoSit", 0.1f);
    }

    private void OnDestroy()
    {
        EventHandler.instance.menuOpened -= OnMenuOpened;
    }

    public void GoSit()
    {
        WhaleHandler.instance.MoveToFire();
    }

    static bool popupDone = false;
    public void OnPlayPressed()
    {
        EventHandler.instance.OnPlayPressed();
        PlayUISound();
        foreach (Transform n in playButton.transform)
        {
            n.GetComponent<TMPro.TextMeshProUGUI>().text = "RESUME";
        }

        if (!popupDone)
        {
            //PopUpHandler.instance.BasePopups(8);
            CallbackHandler.instance.StartTutorial();
            popupDone = true;
        }

        var butComp = playButton.GetComponent<Button>();
        butComp.onClick.RemoveListener(OnPlayPressed);
        playButton.GetComponent<Button>().onClick.AddListener(OnResumePressed);
        playButton.name = "RESUME";
        menuCanvas.SetActive(false);
        WhaleHandler.instance.MoveToSaddle();

        CompassRotation.instance.Show();
    }

    private void OnMenuOpened()
    {
        menuCanvas.SetActive(true);
        WhaleHandler.instance.MoveToFire();

        CompassRotation.instance.Hide();
    }

    private void OnResumePressed()
    {
        PlayUISound();
        EventHandler.instance.resumePressed?.Invoke();
        EventHandler.instance.gameState.gamePaused = false;
        EventHandler.instance.gameState.inMenu = false;
        menuCanvas.SetActive(false);
        WhaleHandler.instance.MoveToSaddle();

        CompassRotation.instance.Show();
    }

    public void OnQuitPressed()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void PlayUISound()
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.PlaySound("crackle");
        }
    }
}
