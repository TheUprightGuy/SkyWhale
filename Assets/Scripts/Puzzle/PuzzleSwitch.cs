using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSwitch : MonoBehaviour
{
    public List<PuzzleSwitch> adjacentSwitches;

    //[HideInInspector] 
    public bool active;
    [HideInInspector] public Material on;
    [HideInInspector] public Material off;

    public List<MeshRenderer> glowMeshes;
    public List<MeshRenderer> highlightMeshes;
    public bool highlight;

    public void Use()
    {
        Switch();

        foreach (PuzzleSwitch n in adjacentSwitches)
        {
            n.Switch();
        }

        if (SwitchPuzzleMaster.instance.CheckComplete())
        {
            Debug.Log("Complete!");
        }
    }

    public void Switch()
    {
        if (!SwitchPuzzleMaster.instance.complete)
        {
            active = !active;
            foreach (MeshRenderer n in highlightMeshes)
            {
                n.material = active ? on : off;
            }
        }
    }

    private void Update()
    {
        if (MouseOverHighlight.instance.highlightedSwitch == this)
        {
            foreach (MeshRenderer n in glowMeshes)
            {
                n.material.SetFloat("Boolean_55E471DA", 1.0f);
            }
        }
        else
        {
            foreach (MeshRenderer n in glowMeshes)
            {
                n.material.SetFloat("Boolean_55E471DA", 0.0f);
            }
        }
    }
}
