/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   GunMeshSwitch.cs
  Description :   Switches the grapple gun mesh depending on loaded state. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMeshSwitch : MonoBehaviour
{
    #region Setup
    MeshFilter mf;
    MeshRenderer mr;
    /// <summary>
    /// Description: Gets Component References.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
    }
    #endregion Setup

    public Transform shootPoint;


    public Mesh loadedMesh;
    public Mesh emptyMesh;
    public Material loadedMat;
    public Material emptyMat;

    /// <summary>
    /// Description: Toggles Mesh.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_toggle">Loaded?</param>
    public void Loaded(bool _toggle)
    {
        mf.mesh = _toggle ? loadedMesh : emptyMesh;
        mr.material = _toggle ? loadedMat : emptyMat;
    }

    public void ToggleEnabled()
    {
        mr.enabled = true;
    }
}
