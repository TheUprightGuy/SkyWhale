using UnityEngine;
using System.Collections;

public class SkyboxBlender : MonoBehaviour
{
    public Material material;
    float timeOfDay = 0.0f;
    public float blendSpeed = 1.0f;

   /* public Color color1;
    public Color color2;
    public Color color;

    Light worldLight;*/

    void Start()
    {
       
        //worldLight = LightRotator.instance.worldLight;
        //material.SetFloat("_Blend", 0f);
        //color1 = material.GetColor("_Tint1");
        //color2 = material.GetColor("_Tint2");
    }


    void Update()
    {
        timeOfDay += Time.deltaTime * blendSpeed;
        if (timeOfDay >= 1.5f)
        {
            timeOfDay = 0.0f;
        }

        material.SetFloat("_Vec1TimeOfDay", timeOfDay);


       // count += Time.deltaTime * blendSpeed;

        //blend = Mathf.PingPong(count, 1.0f);
       // material.SetFloat("_Blend", blend);
        //color = Color.Lerp(color1, color2, blend);
       // worldLight.color = color;
    }
}

