using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PopUp
{
    public string PopUpText = "";
    public KeyCode KeyCheck = KeyCode.None;
    public float TimeCheck = Mathf.Infinity;
    
}
public class PopUpHandler : MonoBehaviour
{
    Text textToChange;
    Animator transitions;
    List<PopUp> PopUpQueue = new List<PopUp>();

    #region Singleton
    public static PopUpHandler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PopUpHandler exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion Singleton

    bool bPlaying = false;
    public float timer = 0.0f;

    private void Start()
    {
        transitions = GetComponent<Animator>();
        textToChange = GetComponentInChildren<Text>();
        //EventHandler.instance.endEstablishingShot += BasePopups;
    }
    // Update is called once per frame
    void Update()
    {
        if (PopUpQueue.Count > 0)
        {
            timer += Time.deltaTime / PopUpQueue[0].TimeCheck;

            //Play next prompt in Queue (make sure has gone to start)
            if (!bPlaying && transitions.GetCurrentAnimatorStateInfo(0).IsName("Start"))
            {
                timer = 0.0f;
                textToChange.text = PopUpQueue[0].PopUpText;
                transitions.SetTrigger("FadeIn");
                bPlaying = true;
            }
            else //
            {

                
                if (Input.GetKeyDown(PopUpQueue[0].KeyCheck) || timer > 1.0f)
                {
                    timer = 0.0f;
                    PopUpQueue.RemoveAt(0);
                    //play end anim
                    transitions.SetTrigger("FadeOut");
                    bPlaying = false;
                }
                
            }
        }
    }

    public void QueuePopUp(string _text, KeyCode _key)
    {
        PopUp newPopUp = new PopUp();
        newPopUp.PopUpText = _text;
        newPopUp.KeyCheck = _key;

        PopUpQueue.Add(newPopUp);
    }

    public void QueuePopUp(string _text, float _time)
    {
        PopUp newPopUp = new PopUp();
        newPopUp.PopUpText = _text;
        newPopUp.TimeCheck = _time;

        PopUpQueue.Add(newPopUp);
    }

    bool bCalled = false;
    public void BasePopups(float time)
    {
        if (!bCalled)
        {
            bCalled = true;
            StartCoroutine(WaitForTime(time));
        }
        
    }

    private IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
        PopUpHandler.instance.QueuePopUp("Use the <b>WASD</b> keys to steer the whale \n Press <b>Space</b> to speed up", KeyCode.Space);

        PopUpHandler.instance.QueuePopUp("Hold <b>Left Click</b> to look around", KeyCode.Mouse0);
    }
}
