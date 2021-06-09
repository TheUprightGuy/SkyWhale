using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveControl : MonoBehaviour
{
    Material dissolveMat;
    float startY, offsetY;
    public bool dissolve;
    private void Awake()
    {
        startY = transform.position.y + 0.5f;
        offsetY = startY;

        dissolveMat = GetComponent<MeshRenderer>().material;
        dissolveMat.SetFloat("CutoffHeight", offsetY);
        CallbackHandler.instance.resetCheckpointDissolve += () => dissolve = true;
    }

    private void Update()
    {
        if (!dissolve)
        {
            offsetY = Mathf.Lerp(offsetY, startY + 1.0f, Time.deltaTime);
            dissolveMat.SetFloat("CutoffHeight", offsetY);
            return;
        }

        offsetY = Mathf.Lerp(offsetY, startY - 2.0f, Time.deltaTime);
        dissolveMat.SetFloat("CutoffHeight", offsetY);
    }

}
