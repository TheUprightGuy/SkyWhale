using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectiveSpawner : MonoBehaviour
{
    public Transform playerSpawn;
    public float maxDistance = 50.0f;
    public float minDistance = 25.0f;
    private Transform _objectiveTransform;

    private void Awake()
    {
        _objectiveTransform = transform;
    }

    private void Start()
    {
        //Spawn objective in random direction (ignore y value)
        var position = Random.insideUnitSphere;
        position.y = 0f;
        position *= maxDistance;
        Vector3.ClampMagnitude(position, maxDistance);
        if (position.magnitude < minDistance)
        {
            Vector3.Normalize(position);
            position *= minDistance;
        }
        _objectiveTransform.position = position;
    }
}
