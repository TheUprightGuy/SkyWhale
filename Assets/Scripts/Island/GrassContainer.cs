using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MeshChunk
{
    public MeshChunk(Vector3Int _index)
    {
        index = _index;
        pointList = new List<Vector3>();
        normalList = new List<Vector3>();
        mesh = null;
        mat = null;
        Rebuild = false;


    }

    public Vector3Int index;

    public List<Vector3> pointList; //Don't like this but gotta do for deleting grass
    public List<Vector3> normalList;
    public Mesh mesh;
    public Material mat;
    public bool Rebuild;
}
public class GrassContainer : ScriptableObject
{
    
    public List<MeshChunk> GrassChunks;

    public bool GrassChunkAtIndex(Vector3Int _index, out MeshChunk chunk)
    {
        chunk = new MeshChunk();

        foreach (var item in GrassChunks)
        {
            if (item.index == _index)
            {
                chunk = item;
                return true;
            }
        }

        return false;
    }

    public int GrassChunkAtIndex(Vector3Int _index)
    {
        for (int i = 0; i < GrassChunks.Count; i++)
        {
            if (GrassChunks[i].index == _index)
            {
                return i;
            }
        }
    
        return -1;
    }
}
