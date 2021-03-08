using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveMovePoint : MonoBehaviour
{
    public Transform moveToTransform;
    public Transform lookAtTransform;
    public int ObjectiveIndex;
    public CinematicController.ObjectiveData objectiveData;
    void Start()
    {
        objectiveData = new CinematicController.ObjectiveData
        {
            moveTo = moveToTransform, lookAt = lookAtTransform, index = ObjectiveIndex
        };
        CinematicController.instance.objectives.Add(objectiveData);
    }
}
