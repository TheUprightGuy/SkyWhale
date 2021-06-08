using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectableInfo
{
    public CollectableInfo(string _name, bool collected = false)
    {
        Name = _name;
        Collected = collected;
    }
    public string Name;
    public bool Collected;
}
[ExecuteAlways]
public class CollectableHandler : MonoBehaviour
{

    public List<CollectableInfo> Collectables = new List<CollectableInfo>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            CollectablesTrigger[] collectablesTriggers = GetComponentsInChildren<CollectablesTrigger>();
            int cc = Collectables.Count;
            int tc = collectablesTriggers.Length;

            if (cc < tc)
            {
                for (int i = cc; i < tc - cc; i++) //for each one until amount in childcount
                {
                    CollectableInfo newInfo = new CollectableInfo(collectablesTriggers[i].gameObject.name);
                    collectablesTriggers[i].handlerIndex = i;
                    Collectables.Add(newInfo);
                }
            }

            if (cc > tc)
            {
                for (int i = cc; i > tc; i--)
                {
                    Collectables.RemoveAt(i - 1);
                }
            }
        }
        
#endif

    }
}
