using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    #region Singleton
    public static InputHandler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Input Handler exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion Singleton

    // Whale
    public KeyCode move = KeyCode.Space;
    public KeyCode orbit = KeyCode.E;

    // MC
    public KeyCode interact = KeyCode.E;
    public KeyCode pickUp = KeyCode.F;
    public KeyCode jump = KeyCode.Space;
    public KeyCode run = KeyCode.LeftShift;
}
