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
        if (RefreshTracker == null)
        {
            RefreshTracker = new Stopwatch();
        }


        Handles.color = Color.blue;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,thisObj.RayCastToHit.value, 
            (thisObj.CastForTriggers) ? (QueryTriggerInteraction.Collide) : (QueryTriggerInteraction.Ignore)) && 
            !Event.current.control && thisObj.grassContainer != null)
        {
            //Used to disable the tool
            int id = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(id);
            Tools.hidden = true;



            RefreshTracker.Start();
            DrawChunkBorders(hit.point);


            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0) //left
                    {
                        BrushHandling(hit.point, hit.normal);
                    }
                    break;
                case EventType.MouseDrag:
                    if (RefreshTracker.ElapsedMilliseconds > thisObj.RefreshRateInMs &&
                        e.button == 0) //Can draw after elapsed refresh
                    {
                        RefreshTracker.Restart(); //Set vals back to 0
                        BrushHandling(hit.point, hit.normal);
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

    public void BrushHandling(Vector3 _point, Vector3 _normal)
    {

        if (!thisObj.VerifyVariables())
        {
            return;
        }
        

        
        if (thisObj.DrawBrush)
        {
            thisObj.PointGen(_point, _normal , thisObj.BrushRadius);
        }
        else
        {
            thisObj.PointDelete(_point, thisObj.BrushRadius);
        }

        thisObj.BuildMesh();
    }
    public override void OnInspectorGUI()
    {
        thisObj.grassContainer = (GrassContainer)EditorGUILayout.ObjectField( "Grass Container", thisObj.grassContainer, typeof(GrassContainer), false);

        if (thisObj.grassContainer != null)
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Save"))
            {
                AssetDatabase.Refresh();
                UnityEditor.EditorUtility.SetDirty(thisObj.grassContainer);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {

            if (GUILayout.Button("New Grass Container"))
            {
                string fp = "Assets/" + thisObj.transform.name + "GrassInstance.asset"; // Probs want to make this a public at some point
                GrassContainer newContainer = ScriptableObject.CreateInstance<GrassContainer>();

                AssetDatabase.CreateAsset(newContainer, fp);
                AssetDatabase.SaveAssets();
                thisObj.grassContainer = (GrassContainer)AssetDatabase.LoadAssetAtPath(fp, typeof(GrassContainer)); //Retrieve the new container 

                UnityEditor.EditorUtility.SetDirty(thisObj.grassContainer);
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
