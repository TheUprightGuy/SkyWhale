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
    private float _spawnHeightDifference = 15f;
    public GameObject whale;
    
    // Start is called before the first frame update
    void Start()
    {
        //CallbackHandler.instance.spawnCollectableIsland += SpawnIsland;
        NewCallbackHandler.instance.spawnCollectableIsland += SpawnIsland;
        _spawnHeightDifference = 15f;
        trailingIslands.Add(whale);
    }

    private void OnDestroy()
    {
        NewCallbackHandler.instance.spawnCollectableIsland -= SpawnIsland;
    }

    private void Update()
    {
        foreach (var trailingIsland in trailingIslands.Where(trailingIsland => trailingIsland.GetComponent<IslandTrailing>() != null))
        {
            trailingIsland.GetComponent<IslandTrailing>().Flock(trailingIslands);
        }
    }

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
    }
}
