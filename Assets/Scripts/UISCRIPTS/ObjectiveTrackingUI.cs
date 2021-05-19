using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTrackingUI : MonoBehaviour
{
    public GameObject questUIPrefab;
    public Animator animator;

    List<ObjectiveUIElement> objs = new List<ObjectiveUIElement>();

    Image image;
    public Sprite onScreen;
    public Sprite offScreen;


    private void Start()
    {
        animator = GetComponent<Animator>();
        image = objMarker.GetComponent<Image>();

        CallbackHandler.instance.startTrackingQuest += StartTrackingQuest;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.startTrackingQuest -= StartTrackingQuest;
    }

    public void StartTrackingQuest(Quest _quest)
    {
        ObjectiveUIElement temp = Instantiate(questUIPrefab, this.transform).GetComponent<ObjectiveUIElement>();
        temp.Setup(_quest);
        objs.Add(temp);
    }

    private void Update()
    {
        // Change this to input listener
        if (Input.GetKey(KeyCode.Tab))
        {
            TabDisplay();
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            HideQuest();
        }

        if (objs.Count <= 0)
            return;

        Vector3 objLoc = objs[0].GetQuestLocation();
        if (objLoc == Vector3.zero)
        {
            objMarker.SetActive(false);
            distanceText.enabled = false;
            return;
        }

        distanceText.SetText(Mathf.RoundToInt(Vector3.Distance(objLoc, EntityManager.instance.player.transform.position)).ToString() + "m");

        Vector3 screenPos = Camera.main.WorldToScreenPoint(objLoc);

        if (screenPos.z > 0
            && screenPos.x > 0 && screenPos.x < Screen.width
            && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            objMarker.transform.position = screenPos;
            objMarker.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            image.sprite = onScreen;
        }
        else // OFFSCREEN
        {
            image.sprite = offScreen;

            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2.0f;

            // translate coords
            screenPos -= screenCenter;

            // find angle from center to mousepos
            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            angle -= 90.0f * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            screenPos = screenCenter + new Vector3(sin * 150.0f, cos * 150.0f, 0);

            float m = cos / sin;

            // get edge
            Vector3 screenBounds = screenCenter * 0.9f;

            // check vertical
            if (cos > 0)
            {
                screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            }
            else
            {
                screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
            }

            // check horizontal
            if (screenPos.x > screenBounds.x)
            {
                screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
            }
            else if (screenPos.x < -screenBounds.x)
            {
                screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
            }

            screenPos += screenCenter;
            objMarker.transform.position = screenPos;
            objMarker.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle * Mathf.Rad2Deg);
        }
    }

    public TMPro.TextMeshProUGUI distanceText;
    public GameObject objMarker;
    float timer;
    public void TabDisplay()
    {
        ShowQuest();
        timer = 5.0f;
    }

    public void ShowQuest()
    {
        animator.SetBool("Show", true);
    }

    public void HideQuest()
    {
        animator.SetBool("Show", false);
    }
}
