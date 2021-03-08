using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

[CustomEditor(typeof(ObjDistribuition))]
public class ObjDistributionEditor : Editor
{
    private Bounds bounds = new Bounds();
    void OnSceneGUI()
    {
        ObjDistribuition myObj = target as ObjDistribuition;

        Handles.DrawWireCube(myObj.transform.position, myObj.transform.localScale);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ObjDistribuition t = target as ObjDistribuition;
        if (GUILayout.Button("Reload"))
        {
           

            switch (t.RenderType)
            {
                case ObjDistribuition.RenderingMode.BATCHED:
                    t.PlaceObjMesh();
                    break;
                case ObjDistribuition.RenderingMode.INDIVIDUAL:
                    if (t.density > 100 && 
                        EditorUtility.DisplayDialog("Oh god consider what you are doing please",
                                                    "Are you sure you want to place " + (t.density * t.density) +
                                                    " individual objects?", "I know what I'm doing :)", "Oh god no take me back"))
                    {
                        t.PlaceObjMesh();
                    }
                    else if (t.density > 100)
                    {
                        t.RenderType = ObjDistribuition.RenderingMode.BATCHED;
                        t.PlaceObjMesh();
                    }
                    else
                    {
                        t.PlaceObjMesh();
                    }
                    break;
                default:
                    break;
            }
        }


        
    }
}
