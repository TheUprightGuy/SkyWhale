using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class RawImageInstancer : MonoBehaviour
{
    private void Awake()
    {
        Material mat = Instantiate(GetComponent<RawImage>().material);
        GetComponent<RawImage>().material = mat;
    }
}
