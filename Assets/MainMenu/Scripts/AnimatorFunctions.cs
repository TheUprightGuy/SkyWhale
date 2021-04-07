/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   AnimatorFunctions.cs
  Description :   Easy multiuse script for ui components. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    /// <summary>
    /// Description: Play SFX on mouseover/click.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_sound"></param>
    public void PlaySound(string _sound)
    {
        Audio.AudioManager.instance.PlaySound(_sound);
    }
}
