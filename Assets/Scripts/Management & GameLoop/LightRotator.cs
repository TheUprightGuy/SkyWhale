using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotator : MonoBehaviour
{
    public float rotSpeed;
    #region Singleton
    public static LightRotator instance;
    //[HideInInspector]
    public Light worldLight;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one World Light exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        worldLight = GetComponentInChildren<Light>();
    }
    #endregion Singleton

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed);
    }
}
