using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRadius : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TestMovement>())
        {
            CallbackHandler.instance.inShopRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<TestMovement>())
        {
            CallbackHandler.instance.inShopRange = false;
            CallbackHandler.instance.HideDetails();
        }
    }
}
