using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingDissolver : MonoBehaviour
{
    Material mat;
    public GameObject ring;
    private void Awake()
    {
        mat = ring.GetComponent<MeshRenderer>().sharedMaterial = Instantiate(ring.GetComponent<MeshRenderer>().sharedMaterial);
        startPos = transform.position.x;
        fadeAmount = startPos;
    }

    public bool fade;
    public float fadeAmount;
    float startPos;

    private void Update()
    {
        fadeAmount = fade ? fadeAmount - Time.deltaTime * 30.0f : fadeAmount + Time.deltaTime * 30.0f;
        fadeAmount = Mathf.Clamp(fadeAmount, startPos, startPos + 65.0f);

        mat.SetFloat("CutoffHeight", fadeAmount);
    }

    private void OnTriggerEnter(Collider other)
    {
        WhaleMovement wm = other.GetComponent<WhaleMovement>();
        if (wm)
            fade = true;
    }

    private void OnTriggerExit(Collider other)
    {
        WhaleMovement wm = other.GetComponent<WhaleMovement>();
        if (wm)
            fade = false;
    }
}
