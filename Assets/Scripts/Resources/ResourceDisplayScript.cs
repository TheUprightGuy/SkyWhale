using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDisplayScript : MonoBehaviour
{
    #region Singleton
    public static ResourceDisplayScript instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one ResourceDisplay exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion Singleton

    // temp
    [HideInInspector] public int supplies;
    [HideInInspector] public int suppliesMax;
    float suppliesPercentage;
    [HideInInspector] public int provisions;
    [HideInInspector] public int provisionsMax;
    float provisionsPercentage;

    //public ResourceFeedBack rfb;
    public List<GameObject> supplyObjs;
    public List<GameObject> provisionObjs;
    public bool tut;

    private void Start()
    {
        suppliesMax = supplyObjs.Count;
        provisionsMax = provisionObjs.Count;
        DisplayResources();
        MCShopUI.instance.resourceCount.SetText(supplies.ToString());
    }

    private void Update()
    {
        // testing
    }

    public void DisplayResources()
    {
        DisplayProvisions();
        DisplaySupplies();
    }

    public bool MaxSupplies()
    {
        return (supplies >= suppliesMax);
    }

    public bool MaxProvisions()
    {
        return (provisions >= provisionsMax);
    }

    public void DisplaySupplies()
    {
        for (int i = 0; i < supplyObjs.Count; i++)
        {
            if (i < Mathf.FloorToInt((float)supplyObjs.Count * suppliesPercentage))
            {
                supplyObjs[i].SetActive(true);
            }
            else
            {
                supplyObjs[i].SetActive(false);
            }
        }
    }

    public void DisplayProvisions()
    {
        for (int i = 0; i < provisionObjs.Count; i++)
        {
            if (i < Mathf.FloorToInt((float)provisionObjs.Count * provisionsPercentage))
            {
                provisionObjs[i].SetActive(true);
            }
            else
            {
                provisionObjs[i].SetActive(false);
            }
        }
    }


    public void AddSupplies(int _supplies)
    {
        if (!tut)
        {
            MapHandler.instance.SetActiveObjective(1);
            // Update Objective Here
            tut = true;
        }

        supplies += _supplies;
        if (supplies > suppliesMax)
        {
            supplies -= supplies % suppliesMax;
        }
        else
        {
            CallbackHandler.instance.SupplyPopUp("+" + _supplies.ToString());
            //rfb.SupplyPopUp("+" + _supplies.ToString());
        }
        suppliesPercentage = (float)supplies / (float)suppliesMax;
        DisplaySupplies();
        MCShopUI.instance.resourceCount.SetText(supplies.ToString());
    }

    public void AddProvisions(int _provisions)
    {
        if (!tut)
        {
            MapHandler.instance.SetActiveObjective(1);
            // Update Objective Here
            tut = true;
        }

        provisions += _provisions;
        if (provisions > provisionsMax)
        {
            provisions -= provisions % provisionsMax;
        }
        else
        {
            CallbackHandler.instance.ProvisionPopUp("+" + _provisions.ToString());
            //rfb.ProvPopUp("+" + _provisions.ToString());
        }
        provisionsPercentage = (float)provisions / (float)provisionsMax;
        DisplayProvisions();
    }

    public bool SpendSupplies(int _supplies)
    {
        if (supplies - _supplies < 0)
        {
            return false;
        }
        else
        {
            CallbackHandler.instance.SupplyPopUp("-" + _supplies.ToString());
            //rfb.ProvPopUp("-" + _supplies.ToString());
        }
        supplies -= _supplies;
        suppliesPercentage = (float)supplies / (float)suppliesMax;
        DisplaySupplies();
        MCShopUI.instance.resourceCount.SetText(supplies.ToString());
        return true;
    }

    public bool SpendProvisions(int _provisions)
    {
        if (provisions - _provisions < 0)
        {
            return false;
        }
        else
        {
            CallbackHandler.instance.ProvisionPopUp("-" + _provisions.ToString());
            //rfb.ProvPopUp("-" + _provisions.ToString());
        }

        provisions -= _provisions;
        DisplayProvisions();
        provisionsPercentage = (float)provisions / (float)provisionsMax;
        return true;
    }

}
