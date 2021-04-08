// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   IslandSpawner.cs
// Description :   Mono behaviour that handles spawning new islands behind the whale or the last island that is trailing.
//                 It also updates the flocking behaviour in each island. 
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Island.CollectableIsland;
using UnityEngine;

public class IslandSpawner : MonoBehaviour
{
    public List<GameObject> trailingIslands;
    public Transform followPoint;
    public GameObject islandPrefab;
    public GameObject islandColliderPrefab;
    private float _spawnHeightDifference = 2.0f;
    public GameObject whale;
    
    /// <summary>
    /// Add callback and initialise trailing islands with the whale
    /// </summary>
    void Start()
    {
        //CallbackHandler.instance.spawnCollectableIsland += SpawnIsland;
        CallbackHandler.instance.spawnCollectableIsland += SpawnIsland;
        trailingIslands.Add(whale);
    }

    /// <summary>
    /// Cleanup callback when destroyed
    /// </summary>
    private void OnDestroy()
    {
        CallbackHandler.instance.spawnCollectableIsland -= SpawnIsland;
    }

    /// <summary>
    /// Update flocking in each island
    /// </summary>
    private void Update()
    {
        foreach (var trailingIsland in trailingIslands.Where(trailingIsland => trailingIsland.GetComponent<IslandTrailing>() != null))
        {
            trailingIsland.GetComponent<IslandTrailing>().Flock(trailingIslands);
        }
    }

    /// <summary>
    /// Spawns an island behind the last island in the island chain and set it's transform and rotation, also sets up it's follow point
    /// </summary>
    public void SpawnIsland()
    {
        //Calculate spawn position/rotation
        GameObject prevIsland = trailingIslands[trailingIslands.Count - 1];
        Vector3 prevIslandPos = prevIsland.transform.position;
        Vector3 spawnPos = prevIslandPos - whale.transform.forward * 40f;
        if (_spawnHeightDifference == 0)//Island being spawned isn't the first island
        {
            //Spawn island along vector between it's transform and follow point
            Vector3 prevIslandFollowPoint = prevIsland.GetComponent<IslandTrailing>().followPoint.position;
            spawnPos = prevIslandPos + (prevIslandPos - prevIslandFollowPoint);
        }
       
        /*var islandRotationVector = Vector3.RotateTowards(Vector3.one, prevIslandPos, 7f, 100f);
        var islandRotation = Quaternion.FromToRotation(transform.forward, islandRotationVector);*/

        var islandRotation = prevIsland.transform.rotation;
        
        //Spawn island
        var island = Instantiate(islandPrefab, spawnPos, islandRotation);
        
        //Set follow point for new island and update follow point for next island
        var islandTrailing = island.GetComponent<IslandTrailing>();
        islandTrailing.followPoint = followPoint;
        followPoint = island.transform;
        
        //If spawned island is the first then adjust follow height by spawnHeightDifference
        islandTrailing.yDistanceBelowIsland = _spawnHeightDifference;
        //All additional islands follow at the same height as their follow point
        _spawnHeightDifference = 0f;
        
        trailingIslands.Add(island);
        /*IslandColliderScript temp = Instantiate(islandColliderPrefab, island.transform.position, island.transform.rotation).GetComponent<IslandColliderScript>();
        temp.island = island.transform;*/
    }
}
