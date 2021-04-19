using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

[CustomEditor(typeof(ObjDistribuition))]
public class ObjDistribuitionEditor : Editor
{
    private Bounds bounds = new Bounds();
    void OnSceneGUI()
    {


        ObjDistribuition myObj = target as ObjDistribuition;
        
        ////Old Stuff
        ///**********************************/
        Handles.color = Color.red;
        Handles.DrawWireCube(myObj.transform.position, myObj.transform.localScale);
        ///**********************************/

        ////New Stuff
        ///**********************************/
        Handles.color = Color.blue;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && !Event.current.control)
        {
            /* Used to disable the tool 
            int id = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(id);
            Tools.hidden = true;
            */
            Vector3Int chunk = myObj.GetChunk(hit.point);
            Handles.Label(hit.point, chunk.ToString());

            float cs = myObj.ChunkSize;
            float halfSize = cs * 0.5f;
            Handles.DrawWireCube(new Vector3(chunk.x * cs + halfSize, chunk.y * cs + halfSize, chunk.z * cs + halfSize), 
                                    new Vector3(myObj.ChunkSize, myObj.ChunkSize, myObj.ChunkSize));

            Handles.Disc(Quaternion.identity, hit.point, hit.normal, 5, false, 1);
            //SHandles.Disc(Quaternion.identity, t.transform.position, new Vector3(1, 1, 0), 5, false, 1);
        }
        else
        {
            //Tools.hidden = false; //Used to re-enable the tool
        }

        SceneView.RepaintAll(); //I dunno if this is healthy tbh lmao
        Handles.color = Color.white;//Reset to default
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
                        //t.PlaceObjMesh();
                    }
                    else if (t.density > 100)
                    {
                        //t.RenderType = ObjDistribuition.RenderingMode.BATCHED;
                        //t.PlaceObjMesh();
                    }
                    else
                    {
                        //t.PlaceObjMesh();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
