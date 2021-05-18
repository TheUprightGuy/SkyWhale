// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   LevelBoundaryChecker.cs
// Description :   Mono behaviour that checks if player is outside level boundaries and triggers callback to mount whale when this happens. 
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz

using System;
using Audio;
using UnityEngine;

namespace Movement
{
    public class LevelBoundaryChecker : MonoBehaviour
    {
        private float timer;
        #region Inspector Variables
            public int yLowestBoundary; //When player y position is lower than this, they are teleported to the whale
            public GrappleChallengeMaster ClosestGrappleChallengeMaster;    //Updated when entering a checkpoint/start/end point

        #endregion

        private void Start()
        {
            CallbackHandler.instance.updateClosestGrappleChallenge += UpdateClosestGrappleChallenge;
        }

        private void OnDestroy()
        {
            CallbackHandler.instance.updateClosestGrappleChallenge -= UpdateClosestGrappleChallenge;
        }

        private void UpdateClosestGrappleChallenge(GrappleChallengeMaster grappleChallengeMaster)
        {
            ClosestGrappleChallengeMaster = grappleChallengeMaster;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!(transform.position.y < yLowestBoundary)) return;
            timer -= Time.deltaTime;
            //Ensure whale is not also below this boundary
            if (EntityManager.instance.whale.transform.position.y < yLowestBoundary)
            {
                //Respawn player and move the whale above boundary
                EntityManager.instance.MoveWhaleAboveBoundary(yLowestBoundary);
                return;
            }
            //MovePlayerToWhale();
            //Respawn player at last checkpoint
            if(timer > 0f) return;
            EntityManager.instance.player.layer = LayerMask.NameToLayer("Player");
            ClosestGrappleChallengeMaster.ResetChallenge();
            AudioManager.instance.PlaySound("Fail");
            timer = 1f;
        }

        private static void MovePlayerToWhale()
        {
            //Respawn player by moving them to the whale
            EntityManager.instance.player.layer = LayerMask.NameToLayer("PlayerFromWhale");
            EntityManager.instance.grappleHook.gameObject.layer = LayerMask.NameToLayer("HookFromWhale");
            EntityManager.instance.TogglePlayer(false);
            EntityManager.instance.OnPlayerLowerThanBoundary();
        }
    }
}
