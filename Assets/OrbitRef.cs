using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRef : MonoBehaviour
{
    public GameObject heightRef;

    private void OnTriggerEnter(Collider other)
    {
        OrbitScript temp = other.GetComponent<OrbitScript>();
        if (temp)
        {
            temp.orbit = heightRef;
        }
    }
}
