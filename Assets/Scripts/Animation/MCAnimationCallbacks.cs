using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCAnimationCallbacks : MonoBehaviour
{
    public TestMovement player;

    public void UnfreezeMC()
    {
        player.freezeMe = false;
    }
}
