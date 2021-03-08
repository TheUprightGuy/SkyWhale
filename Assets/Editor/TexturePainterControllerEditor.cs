using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;

[CustomEditor(typeof(TexturePainterController))]
public class TexturePainterControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TexturePainterController t = target as TexturePainterController;

        if (GUILayout.Button("ReGen"))
        {
            t.UpdateDefaultColors();
        }
    }

    double lastCheck = 0.0f;
    bool mouseHeld = false;
     void OnSceneGUI()
     {
        if ( Event.current.button == 0)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    mouseHeld = true;
                    break;
                case EventType.MouseUp:
                    mouseHeld = false;
                    break;
                default:
                    break;
            }
            
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
         RaycastHit hit;

        TexturePainterController t = target as TexturePainterController;
        Handles.color = Color.white;
         if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Island")) && !Event.current.control && !Event.current.alt)
         {
             int id = GUIUtility.GetControlID(FocusType.Passive);
             HandleUtility.AddDefaultControl(id);
             Tools.hidden = true;

            Handles.Disc(Quaternion.identity, hit.point, hit.normal, 1, false, 1);
            t.Hitpoint = hit.point;
            //SHandles.Disc(Quaternion.identity, t.transform.position, new Vector3(1, 1, 0), 5, false, 1);

            if (mouseHeld && (EditorApplication.timeSinceStartup - lastCheck) > (1 / 60))
            {
                lastCheck = EditorApplication.timeSinceStartup;
                Debug.Log("Left-Mouse Down"/*EditorApplication.timeSinceStartup*/);
                t.UpdateBrush();
            }
        }
         else
         {
             Tools.hidden = false;
             t.Hitpoint = Vector3.positiveInfinity;
         }
     
         

        
        //if (t.ModifyVoxels())
        //{
        //    Handles.color = Color.red;
        //}

        SceneView.RepaintAll();
     }
}
