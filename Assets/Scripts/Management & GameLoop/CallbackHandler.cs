/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   CallbackHandler.cs
  Description :   Handles a number of different callbacks between unrelated game elements. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CallbackHandler : MonoBehaviour
{
    #region Singleton
    public static CallbackHandler instance;
    /// <summary>
    /// Description: Singleton Setup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Callback Handler already exists!");
            Destroy(this.gameObject);
        }
        instance = this;
    }
    #endregion Singleton

    public event Action resetActionTimer;
    public void ResetActionTimer()
    {
        if (resetActionTimer != null)
        {
            resetActionTimer();
        }
    }

    public event Action<bool> cinematicPause;
    public void CinematicPause(bool _pause)
    {
        if (cinematicPause != null)
        {
            cinematicPause(_pause);
        }
    }

    public event Action gliderPartsCollected;
    public void GliderPartsCollected()
    {
        if (gliderPartsCollected != null)
        {
            gliderPartsCollected();
        }
    }

    public void AddGliderPartsCollectedCallbackForNpc(GameObject npc)
    {
        gliderPartsCollected += npc.GetComponent<NPCScript>().SwitchDialogue;
    }
    
    public event Action movePilotNPCs;
    public void MovePilotNPCs()
    {
        if (movePilotNPCs != null)
        {
            movePilotNPCs();
        }
    }

    public void AddMovePilotNPCsCallbackForNpc(GameObject npc)
    {
        movePilotNPCs += npc.GetComponent<NPCScript>().SwitchDialogue;
    }

    public event Action<bool> pause;
    /// <summary>
    /// Description: Pauses all movement based objects.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_pause">Pause State</param>
    public void Pause(bool _pause)
    {
        if (pause != null)
        {
            pause(_pause);
        }
    }

    public Action<GameObject> setDestination;
    /// <summary>
    /// Description: Sets whales destination for pickup.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_destination">Object to find path to</param>
    public void SetDestination(GameObject _destination)
    {
        if (setDestination != null)
        {
            setDestination(_destination);
        }
    }

    public Action setOrbit;
    /// <summary>
    /// Description: Sets whales object to orbit.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_orbit">Object to orbit</param>
    public void SetOrbit()
    {
        if (setOrbit != null)
        {
            setOrbit();
        }
    }

    public Action<GameObject> setNewOrbitRef;
    public void SetNewOrbitRefer(GameObject _ref)
    {
        if (setNewOrbitRef != null)
        {
            setNewOrbitRef(_ref);
        }
    }
    
    public Action onGrappleJump;
    /// <summary>
    /// Description: Callback on grapple jump
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 05/12/2021</br>
    /// </summary>
    public void OnGrappleJump()
    {
        if (onGrappleJump != null)
        {
            onGrappleJump();
        }
    }
    
    public Action<GrappleChallengeMaster> updateClosestGrappleChallenge;
    /// <summary>
    /// Description: Updates which grapple challenge checkpoints/start points player should respawn to
    /// when they fall below the islands.
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 05/11/2021</br>
    /// </summary>
    /// <param name="_grappleChallengeMaster">Closest grapple challenge</param>
    public void UpdateClosestGrappleChallenge(GrappleChallengeMaster _grappleChallengeMaster)
    {
        if (updateClosestGrappleChallenge != null)
        {
            updateClosestGrappleChallenge(_grappleChallengeMaster);
        }
    }

    public event Action spawnCollectableIsland;
    /// <summary>
    /// Description: Spawns Collectable Island.
    /// <br>Author: Jacob Gallagher</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void SpawnCollectableIsland()
    {
        if (spawnCollectableIsland != null)
        {
            spawnCollectableIsland();
        }
    }


    public event Action<Dialogue> setDialogue;
    /// <summary>
    /// Description: Passes dialogue to dialogue manager.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_dialogue">Dialogue to Start</param>
    public void SetDialogue(Dialogue _dialogue)
    {
        if (setDialogue != null)
        {
            setDialogue(_dialogue);
            SpeechOutOfRange();
        }
    }

    public event Action stopDialogue;
    /// <summary>
    /// Description: Stops current dialogue.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void StopDialogue()
    {
        if (stopDialogue != null)
        {
            stopDialogue();
        }
    }

    public event Action<Transform> speechInRange;
    /// <summary>
    /// Description: Prompts the speech ui element to show.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    /// <param name="_dialogueTransform"></param>
    public void SpeechInRange(Transform _dialogueTransform)
    {
        if (speechInRange != null)
        {
            speechInRange(_dialogueTransform);
        }
    }

    public event Action speechOutOfRange;
    /// <summary>
    /// Description: Prompts the speech ui element to hide.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    public void SpeechOutOfRange()
    {
        if (speechOutOfRange != null)
        {
            speechOutOfRange();
        }
    }

    public event Action<Transform> puzzleInRange;
    /// <summary>
    /// Description: Prompts the speech ui element to show.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    /// <param name="_position"></param>
    public void PuzzleInRange(Transform _position)
    {
        if (puzzleInRange != null)
        {
            puzzleInRange(_position);
        }
    }

    public event Action puzzleOutOfRange;
    /// <summary>
    /// Description: Prompts the speech ui element to hide.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 16/04/2021</br>
    /// </summary>
    public void PuzzleOutOfRange()
    {
        if (puzzleOutOfRange != null)
        {
            puzzleOutOfRange();
        }
    }

    public event Action<PromptType> displayPrompt;
    public void DisplayPrompt(PromptType _type)
    {
        if (displayPrompt != null)
        {
            displayPrompt(_type);
        }
    }

    public event Action<PromptType> hidePrompt;
    public void HidePrompt(PromptType _type)
    {
        if (hidePrompt != null)
        {
            hidePrompt(_type);
        }
    }

    public event Action resetCamera;
    public void ResetCamera()
    {
        if (resetCamera != null)
        {
            resetCamera();
        }
    }


    public event Action<Quest> startTrackingQuest;
    public void StartTrackingQuest(Quest _quest)
    {
        if (startTrackingQuest != null)
        {
            startTrackingQuest(_quest);
        }
    }

    public event Action updateObjectives;
    public void UpdateObjectives()
    {
        if (updateObjectives != null)
        {
            updateObjectives();
        }
    }

    public event Action<int> changeMouseSensitivity;
    public void ChangeMouseSensitivity(int _value)
    {
        if (changeMouseSensitivity != null)
        {
            changeMouseSensitivity(_value);
        }
    }

    public event Action<InputState> dismount;
    public void Dismount(InputState _input)
    {
        if (dismount != null)
        {
            dismount(_input);
        }
    }

    public event Action fadeIn;
    public void FadeIn()
    {
        if (fadeIn != null)
        {
            fadeIn();
        }
    }

    public event Action fadeOut;
    public void FadeOut()
    {
        if (fadeOut != null)
        {
            fadeOut();
        }
    }

    public event Action<bool> toggleRain;
    public void ToggleRain(bool _toggle)
    {
        if (toggleRain != null)
        {
            toggleRain(_toggle);
        }
    }
}
