using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CollectableUIHandler : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    RectTransform thisRect = null;
    // Update is called once per frame
    void Update()
    {
        if (thisRect == null)
        {
           thisRect = this.GetComponent<RectTransform>();
        }

        float yBounds = (transform.childCount - (transform.childCount % 5)) / 5;
        yBounds *= 400;

        Vector2 rectsize = thisRect.sizeDelta;
        rectsize.y = yBounds;
        thisRect.sizeDelta = rectsize;
    }


   
}
