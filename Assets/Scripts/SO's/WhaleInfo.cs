using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WhaleData", menuName = "InfoObjects/WhaleData")]
public class WhaleInfo : ScriptableObject
{
    public bool leashed = false;
    public GameObject whale;

    public Thought currentThought;

    public float hunger = 0.0f;
    public float hungerModifier;
    public float weight = 0.0f;

    public bool foodPopupDone = false;
    public void UpdateHunger(float _time)
    {
        hunger -= _time;
        if (hunger < 0){hunger = 0;}

        if (hunger < 20)
        {
            // Overwrite any other thoughts
            if (currentThought != Thought.Food)
            {
                //ThoughtsScript.instance.ShowThought(Thought.Food, true);

                if (!foodPopupDone)
                {
                    foodPopupDone = true;
                    //PopUpHandler.instance.QueuePopUp("When the whale gets hungry, press <b>1</b> to feed it from provisions", KeyCode.Alpha1);
                }
            }
            hungerModifier = 0.75f;
        }
        else if (hunger < 70)
        {
            hungerModifier = 1.0f;
        }
        else
        {
            hungerModifier = 1.25f;
        }

        // Not currently Used
        CheckWeight();
    }

    public void CheckWeight()
    {
        // Too much Weight & not Hungry
        if (weight > 70.0f && ((currentThought != Thought.Weight) && (currentThought != Thought.Food)))
        {
            ThoughtsScript.instance.ShowThought(Thought.Weight, true);
        }
        if (weight <= 70.0f && currentThought == Thought.Weight)
        {
            ThoughtsScript.instance.ShowThought(Thought.None, true);
        }
    }

    public void FeedWhale()
    {
        hunger += 100.0f;
        if (hunger > 100)
        {
            hunger = 100;
        }
    }

    public void ResetOnPlay()
    {
        leashed = false;
        whale = null;
        hunger = 69.0f; // LMAO
        weight = 0.0f;
        currentThought = Thought.None;
        foodPopupDone = false;
    }

    public void ToggleLeashed(bool _toggle)
    {
        leashed = _toggle;
    }
}
