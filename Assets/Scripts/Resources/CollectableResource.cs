using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableResource : MonoBehaviour
{
    public int suppliesCount;
    public int provisionsCount;


    private void OnTriggerEnter(Collider other)
    {
        TestMovement temp = other.GetComponent<TestMovement>();
        if (temp)
        {
            ResourceDisplayScript rds = ResourceDisplayScript.instance;
            if (!rds.MaxProvisions() && provisionsCount != 0)
            {
                rds.AddProvisions(provisionsCount);
                provisionsCount = 0;
            }
            if (!rds.MaxSupplies() && suppliesCount != 0)
            {
                rds.AddSupplies(suppliesCount);
                suppliesCount = 0;
            }

            // Nothing Left to Give
            if (suppliesCount == 0 && provisionsCount == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
