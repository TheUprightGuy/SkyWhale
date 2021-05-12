using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpinningMovement : MonoBehaviour
{
    public float spinTime = 3f;
    private float angle;
    private LTDescr _spinTween;
    
    // Start is called before the first frame update
    void Start()
    {
        angle = 179f;
        Spin();
    }
    
    private void Spin()
    {
        angle = Math.Abs(angle - 180f) < 2f ? 0f : 180f;
        _spinTween = LeanTween.rotateZ(gameObject, angle, spinTime).setOnComplete(Spin);
    }
}
