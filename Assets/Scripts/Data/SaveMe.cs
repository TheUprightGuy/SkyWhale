/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   SaveMe.cs
  Description :   An Object that requires its information saved/loaded. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMe : MonoBehaviour
{
    [HideInInspector] public SaveInfo info;

    #region Callbacks
    /// <summary>
    /// Description: Setup Callbacks.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    private void Start()
    {       
        SaveManager.instance.save += SavePosition;
        SaveManager.instance.load += LoadPosition;

        info.uniqueID = SaveManager.instance.GetID();
    }
    private void OnDestroy()
    {
        SaveManager.instance.save -= SavePosition;
        SaveManager.instance.load -= LoadPosition;
    }
    #endregion Callbacks

    /// <summary>
    /// Description: Saves this objects position.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void SavePosition()
    {
        info.position = transform.position;
        info.rotation = transform.rotation;
        SaveManager.instance.SaveElement(this);
    }

    /// <summary>
    /// Description: Loads this objects position.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>  
    /// </summary>
    public void LoadPosition()
    {
        if (SaveManager.instance.LoadElement(info) != null)
        {
            info = SaveManager.instance.LoadElement(info);
            transform.position = info.position;
            transform.rotation = info.rotation;
        }
    }
}
