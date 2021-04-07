using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ObjectFinder
{
    private static List<GameObject> actors;

    public static GameObject FindObject(string name)
    {
        return GetAllObjectsOnlyInScene().FirstOrDefault(go => go.name == name);
    }
    public static List<GameObject> GetAllObjectsOnlyInScene()
    {
        var objectsInScene = new List<GameObject>();

        foreach (var go in (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            var isFoundOnDisk = EditorUtility.IsPersistent(go.transform.root.gameObject);
            var notEditableOrHidden = go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave;
            if (!isFoundOnDisk && !notEditableOrHidden) objectsInScene.Add(go);
        }

        return objectsInScene;
    }
    public static IEnumerable<GameObject> FindAllObjectsWithTag(string tag)
    {
        if (tag == null) return null;
        Initialize();
        var objectsToSearchThrough = Object.FindObjectsOfType<GameObject>();
        foreach (var objectToSearch in objectsToSearchThrough)
        {
            if (objectToSearch.CompareTag(tag)) actors.Add(objectToSearch);
            GetChildrenObjectsWithTag(objectToSearch.transform, tag);
        }
        return actors;
    }

    private static void Initialize()
    {
        if (actors == null) actors = new List<GameObject>();
        actors.Clear();
    }

    private static void GetChildrenObjectsWithTag(Transform parent, string tag)
    {
        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.CompareTag(tag))
            {
                actors.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                GetChildrenObjectsWithTag(child, tag);
            }
        }
    }
}