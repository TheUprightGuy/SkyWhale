using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Item
{
    public string name;
    public string description;
    public int cost;
    public Sprite image;
    public ItemType type;
}

public enum ItemType
{
    Provisions,
    Lantern,
    Saddle
}


public class ShopItem : MonoBehaviour
{
    public bool active;
    public Material highlightMaterial;  

    public MeshRenderer meshRenderer;
    public bool highlight;

    public Item item;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        highlightMaterial = Material.Instantiate(highlightMaterial);
        highlightMaterial.SetFloat("Boolean_4735036D", meshRenderer.material.mainTexture ? 1.0f : 0.0f);
        highlightMaterial.SetTexture("Texture2D_F5691932", meshRenderer.material.mainTexture);

        meshRenderer.material = highlightMaterial;
    }

    private void Update()
    {
        if (MouseOverHighlight.instance.highlightedShopItem == this)
        {
            meshRenderer.material.SetFloat("Boolean_55E471DA", 1.0f);
        }
        else
        {
            meshRenderer.material.SetFloat("Boolean_55E471DA", 0.0f);
        }
    }

    public void ShowUI()
    {
        CallbackHandler.instance.ShowDetails(item, this);
    }
}
