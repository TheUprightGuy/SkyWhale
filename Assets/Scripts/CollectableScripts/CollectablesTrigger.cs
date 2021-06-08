using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CollectablesTrigger : MonoBehaviour
{
    ParticleSystem ps;

    //[HideInInspector]
    public int handlerIndex;

    CollectableHandler collectableHandler = null;

    MeshRenderer[] renderers;
    private void Awake()
    {
        if (transform.parent !=null)
        {
            collectableHandler = transform.parent.GetComponent<CollectableHandler>();

        }
        ps = GetComponentInChildren<ParticleSystem>();
        prevName = gameObject.name;

        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    string prevName = "";

    private void Update()
    {
        if (collectableHandler != null && prevName != gameObject.name)
        {
            collectableHandler.Collectables[handlerIndex].Name = gameObject.name;
            prevName = gameObject.name;
        }

        if (collected && !ps.isPlaying)
        {
            this.gameObject.SetActive(false);
        }
    }

    bool collected = false;
    private void OnTriggerEnter(Collider other)
    {
        if (collectableHandler != null)
        {
            collectableHandler.Collectables[handlerIndex].Collected = true;
            collected = true;
            ps.Play();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }
            //this.gameObject.SetActive(false);
        }
    }
}
