using System.Collections;
using System.Collections.Generic;
using Island.CollectableIsland;
using UnityEngine;

public class IslandSpawner : MonoBehaviour
{
    public Transform followPoint;
    public GameObject islandPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        CallbackHandler.instance.spawnCollectableIsland += SpawnIsland;
    }

    public void SpawnIsland()
    {
        //Calculate spawn position/rotation
        Vector3 prevIslandPos = gameObject.GetComponent<ChainController>().GetPreviousIslandPosition(0).position;
        Vector3 spawnPos = prevIslandPos - gameObject.GetComponent<ChainController>().GetPreviousIslandPosition(0).forward * 80f;
        var islandRotationVector = Vector3.RotateTowards(Vector3.one, prevIslandPos, 7f, 100f);
        var islandRotation = Quaternion.FromToRotation(transform.forward, islandRotationVector);
        
        //Spawn island
        var island = Instantiate(islandPrefab, spawnPos, islandRotation);
        
        //Set follow point for new island and update follow point for next island
        island.GetComponent<IslandTrailing>().followPoint = followPoint;
        followPoint = island.transform;
    }
}
