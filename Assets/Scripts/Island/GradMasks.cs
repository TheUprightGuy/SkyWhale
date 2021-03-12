using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GradientMask
{
    public Vector2 Position;
    public float size;
}

[ExecuteAlways]
public class GradMasks : MonoBehaviour
{
    public List<GradientMask> GradList = new List<GradientMask>();
    public int GradCount = 1;

    public float maxSize = 10.0f;
    public float minSize = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnValidate()
    {
        UpdateList();
    }

    void UpdateList()
    {
        GradList.Clear();
        while (GradList.Count < GradCount)
        {
            GradientMask randomMask;
            randomMask.size = Random.Range(minSize, maxSize);
            randomMask.Position.x = Random.Range(-1.0f, 1.0f);
            randomMask.Position.y = Random.Range(-1.0f, 1.0f);
            GradList.Add(randomMask);
        }
    }
}
