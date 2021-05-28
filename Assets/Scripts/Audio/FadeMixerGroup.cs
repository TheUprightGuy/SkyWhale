// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   FadeMixerGroup.cs
// Description :   Coroutine that handles fading (lerping) the volume parameter of an audiomixer from it's current volume to its target volume
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public static class FadeMixerGroup
    {
        public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume, int ambientLayerIndex)
        {
            float currentTime = 0;
            audioMixer.GetFloat(exposedParam, out var currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);
            var targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                var newVol = Mathf.Lerp(currentVol, targetValue * AudioManager.instance.targetValueMultiplier[ambientLayerIndex], currentTime / duration);
                audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
                float val = Mathf.Log10(newVol) * 20;
                float orVal = Mathf.Pow(10, val / 20);
                Debug.Log(exposedParam + " :" + orVal + " or " + newVol);
                yield return null;
            }
        }
    }
}