using UnityEngine;
using System.Collections;

public class SkyboxBlender : MonoBehaviour
{
    public Material material;
    float timeOfDay = 0.0f;
    public float blendSpeed = 1.0f;

    public bool raining;

    public Color outputColor;
    public Color color1;
    public Color color2;
    public Color color3;
    public Color fogBlend;// = new Color(70, 149, 202);
    public Color lightBlend;
    Color rainColor;

    public Light[] lights;

   /* public Color color1;
    public Color color2;
    public Color color;

    Light worldLight;*/

    void Start()
    {
        lights = GetComponentsInChildren<Light>();

        rainColor = material.GetColor("RainColor");
        Invoke("RainOff", 0.1f);

        //worldLight = LightRotator.instance.worldLight;
        //material.SetFloat("_Blend", 0f);
        //color1 = material.GetColor("_Tint1");
        //color2 = material.GetColor("_Tint2");

        CallbackHandler.instance.toggleRain += ToggleRain;
    }

    private void OnDestroy()
    {
        CallbackHandler.instance.toggleRain -= ToggleRain;
    }

    void RainOff()
    {
        CallbackHandler.instance.ToggleRain(false);
    }

    void ToggleRain(bool _toggle)
    {
        raining = _toggle;
    }

    void SetLightColor()
    {
        foreach(Light n in lights)
        {
            n.color = Color.Lerp(outputColor, lightBlend, 0.8f);
        }
    }

    void SetFogColor()
    {
        if (timeOfDay < 0.5f)
        {
            outputColor = Color.Lerp(color1, color2, timeOfDay * 2.0f) * (raining ? rainColor : Color.white);
            RenderSettings.fogColor = Color.Lerp(outputColor, fogBlend, 0.5f);
        }
        else if (timeOfDay < 1.0f)
        {
            outputColor = Color.Lerp(color2, color3, (timeOfDay - 0.5f) * 2.0f) * (raining ? rainColor : Color.white);
            RenderSettings.fogColor = Color.Lerp(outputColor, fogBlend, 0.5f);
        }
        else if (timeOfDay < 1.5f)
        {
            outputColor = Color.Lerp(color3, color1, (timeOfDay - 1.0f) * 2.0f) * (raining ? rainColor : Color.white);
            RenderSettings.fogColor = Color.Lerp(outputColor, fogBlend, 0.5f);
        }
    }


    void Update()
    {
        timeOfDay += Time.deltaTime * blendSpeed;
        if (timeOfDay >= 1.5f)
        {
            timeOfDay = 0.0f;
        }

        material.SetFloat("_Vec1TimeOfDay", timeOfDay);
        material.SetInt("Raining", raining ? 1 : 0);

        SetFogColor();
        SetLightColor();

       // count += Time.deltaTime * blendSpeed;

        //blend = Mathf.PingPong(count, 1.0f);
       // material.SetFloat("_Blend", blend);
        //color = Color.Lerp(color1, color2, blend);
       // worldLight.color = color;
    }
}

