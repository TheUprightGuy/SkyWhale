/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   PuzzleSwitch.cs
  Description :   Handles individual switches in the switch puzzle. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/


using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class PuzzleSwitch : MonoBehaviour
{
    public List<PuzzleSwitch> adjacentSwitches;

    //[HideInInspector] 
    public bool active = true;
    [HideInInspector] public Material on;
    [HideInInspector] public Material off;

    public List<MeshRenderer> glowMeshes;
    public List<MeshRenderer> highlightMeshes;
    public bool highlight;

    /// <summary>
    /// Description: Switches self and adjacent switches.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void Use()
    {
        Switch();

        foreach (PuzzleSwitch n in adjacentSwitches)
        {
            n.Switch();
        }

        SwitchPuzzleMaster.instance.CheckComplete();

    }

    /// <summary>
    /// Description: Switches this on/off.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void Switch()
    {
        if (!SwitchPuzzleMaster.instance.complete)
        {
            active = !active;
            foreach (MeshRenderer n in highlightMeshes)
            {
                n.material = active ? on : off;
            }

            AudioManager.instance.PlaySound("Switch");
        }
    }
}
