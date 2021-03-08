using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IslandGen))]
public class IslandGenEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        IslandGen t = target as IslandGen;

        if (GUILayout.Button("ReGen"))
        {
            for (int i = t.transform.childCount; i > 0; --i)
                DestroyImmediate(t.transform.GetChild(0).gameObject);
            t.ReGen();
            t.ReBuild();
        }
    }


    void OnSceneGUI()
    {


        
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        IslandGen t = target as IslandGen;
        Handles.color = Color.white;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Island")) && !Event.current.control)
        {
            int id = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(id);
            Tools.hidden = true;

            
            t.Hitpoint = hit.point;
            //SHandles.Disc(Quaternion.identity, t.transform.position, new Vector3(1, 1, 0), 5, false, 1);
        }
        else
        {
            Tools.hidden = false;
            t.Hitpoint = Vector3.positiveInfinity;
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            Debug.Log("Left-Mouse Down");
            for (int i = t.transform.childCount; i > 0; --i)
                DestroyImmediate(t.transform.GetChild(0).gameObject);

            float digVal = t.BrushDensity;
            if (Event.current.shift)
            {
                digVal = -digVal;
            }

            t.ModifyVoxels(digVal);
            t.ReBuild();
        }

        //if (t.ModifyVoxels())
        //{
        //    Handles.color = Color.red;
        //}
        Handles.Disc(Quaternion.identity, ray.origin + (ray.direction * 10.0f), ray.direction, 1, false, 1);
        SceneView.RepaintAll();
    }
}
