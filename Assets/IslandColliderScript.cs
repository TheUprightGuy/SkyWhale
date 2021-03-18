using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandColliderScript : MonoBehaviour
{
    public Transform island;

    private void Update()
    {
        transform.position = island.position;
        transform.rotation = island.rotation;
    }
}
