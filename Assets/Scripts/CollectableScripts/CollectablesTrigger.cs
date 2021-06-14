using System.Collections;
using System.Collections.Generic;
using Audio;
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


    //private void Update()
    //{

    //    if (Collectable.Collected && !ps.isPlaying)
    //    {
    //        this.gameObject.SetActive(false);
    //    }
    //}

    public void TriggerLargePanel()
    {
        collectableHandler.TriggerLargePanel(handlerIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collectableHandler != null && !Collectable.Collected)
        {
            Collectable.Collected = true;
            collectableHandler.ItemCollected(handlerIndex);
            ps.Play();
            AudioManager.instance.PlaySound("Collect");
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }
            //this.gameObject.SetActive(false);
        }
    }
}
