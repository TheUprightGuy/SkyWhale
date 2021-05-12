using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BobbingMovement : MonoBehaviour
{
    public float bobAmount = 0.5f;
    public float bobTime = 3f;
    private LTDescr _bobTween;
    private int direction = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        bobTime *= Random.Range(0.5f, 1.5f);
        BobIsland();
    }
    
    private void BobIsland()
    {
        _bobTween = LeanTween.moveY(gameObject,  transform.position.y + bobAmount * direction, bobTime)
            .setEase(LeanTweenType.easeInOutQuad).setOnComplete(BobIsland);
        direction *= -1;
    }
}
