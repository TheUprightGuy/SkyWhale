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


    public MeshRenderer front;
    public MeshRenderer rear;

    public Sprite TokenImageOverload = null;
    public CollectableInfo Collectable;
    private void Awake()
    {
        if (transform.parent !=null)
        {
            collectableHandler = transform.parent.GetComponent<CollectableHandler>();

            Texture applyTex = (TokenImageOverload != null) ? (TokenImageOverload.texture) : (Collectable.UISprite.texture);

            front.material.SetTexture("_BaseMap", applyTex);
            rear.material.SetTexture("_BaseMap", applyTex);
        }

        ps = GetComponentInChildren<ParticleSystem>();

        renderers = GetComponentsInChildren<MeshRenderer>();
        
    }


    private void Update()
    {

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
            
            collectableHandler.ItemCollected(handlerIndex);
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
