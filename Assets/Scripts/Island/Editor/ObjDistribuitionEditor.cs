using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Diagnostics;

[CustomEditor(typeof(ObjDistribuition))]
public class ObjDistribuitionEditor : Editor
{
    private ObjDistribuition thisObj;
    private Bounds bounds = new Bounds();

    Stopwatch RefreshTracker;
    void OnSceneGUI()
    {

        
        thisObj = target as ObjDistribuition;



        Handles.color = Color.blue;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && !Event.current.control)
        {
            //Used to disable the tool
            int id = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(id);
            Tools.hidden = true;




            DrawChunkBorders(hit.point);


            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    BrushHandling(hit.point);
                    break;
                case EventType.MouseDrag:
                    if (RefreshTracker.ElapsedMilliseconds > thisObj.RefreshRateInMs) //Can draw after elapsed refresh
                    {
                        RefreshTracker.Restart(); //Set vals back to 0
                        BrushHandling(hit.point);
                    }
                    break;
                case EventType.MouseUp:
                    {
                        RefreshTracker.Stop();
                        RefreshTracker.Reset();
                    }
                    break;
            }

            Handles.Disc(Quaternion.identity, hit.point, hit.normal, thisObj.BrushRadius, false, 1);
            //SHandles.Disc(Quaternion.identity, t.transform.position, new Vector3(1, 1, 0), 5, false, 1);
        }
        else
        {
            Tools.hidden = false; //Used to re-enable the tool
        }

        SceneView.RepaintAll(); //I dunno if this is healthy tbh lmao
        //bHandles.color = Color.white;//Reset to default
    }

    public void BrushHandling(Vector3 _point)
    {

        if (RefreshTracker == null)
        {
            RefreshTracker = new Stopwatch();
        }
        RefreshTracker.Start();

        thisObj.VerifyVariables();
        if (thisObj.DrawBrush)
        {
            thisObj.PointGen(_point, thisObj.BrushRadius);
        }
        else
        {
            thisObj.PointDelete(_point, thisObj.BrushRadius);
        }

        thisObj.BuildMesh();
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
                    //t.PlaceObjMesh();
                    thisObj.BuildMesh(true);
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

   
    private void DrawChunkBorders(Vector3 _point)
    {
        if (thisObj.ShowChunkBorders)
        {
            Vector3Int hitchunk = thisObj.GetChunk(_point);
            //Handles.Label(hit.point, chunk.ToString());
            float cs = thisObj.ChunkSize;
            float halfSize = cs * 0.5f;
            
            Vector3 drawChunk = hitchunk;
            drawChunk.x = hitchunk.x - 1;
            Handles.DrawWireCube(new Vector3(drawChunk.x * cs + halfSize, drawChunk.y * cs + halfSize, drawChunk.z * cs + halfSize),
                                new Vector3(thisObj.ChunkSize, thisObj.ChunkSize, thisObj.ChunkSize));

            drawChunk = hitchunk;
            drawChunk.x = hitchunk.x + 1;
            Handles.DrawWireCube(new Vector3(drawChunk.x * cs + halfSize, drawChunk.y * cs + halfSize, drawChunk.z * cs + halfSize),
                                new Vector3(thisObj.ChunkSize, thisObj.ChunkSize, thisObj.ChunkSize));

            drawChunk = hitchunk;
            drawChunk.z = hitchunk.z - 1;
            Handles.DrawWireCube(new Vector3(drawChunk.x * cs + halfSize, drawChunk.y * cs + halfSize, drawChunk.z * cs + halfSize),
                                new Vector3(thisObj.ChunkSize, thisObj.ChunkSize, thisObj.ChunkSize));

            drawChunk = hitchunk;
            drawChunk.z = hitchunk.z + 1;
            Handles.DrawWireCube(new Vector3(drawChunk.x * cs + halfSize, drawChunk.y * cs + halfSize, drawChunk.z * cs + halfSize),
                                new Vector3(thisObj.ChunkSize, thisObj.ChunkSize, thisObj.ChunkSize));

            drawChunk = hitchunk;
            drawChunk.y = hitchunk.y + 1;
            Handles.DrawWireCube(new Vector3(drawChunk.x * cs + halfSize, drawChunk.y * cs + halfSize, drawChunk.z * cs + halfSize),
                                new Vector3(thisObj.ChunkSize, thisObj.ChunkSize, thisObj.ChunkSize));

            drawChunk = hitchunk;
            drawChunk.y = hitchunk.y - 1;
            Handles.DrawWireCube(new Vector3(drawChunk.x * cs + halfSize, drawChunk.y * cs + halfSize, drawChunk.z * cs + halfSize),
                                new Vector3(thisObj.ChunkSize, thisObj.ChunkSize, thisObj.ChunkSize));

        }
    }
}
