using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePuzzle : MonoBehaviour
{
    private void OnMouseDown()
    {
        Rotate();
    }

    [HideInInspector] public CirclePuzzleMaster parent;
    [HideInInspector] public bool rotating;
    [HideInInspector] public float rotato;
    [HideInInspector] public float speed;

    public List<CircleSymbol> sharedObjects;
    public RingPFX ringPFX;

    public void Rotate()
    {
        if (!rotating && !parent.inUse && !parent.completed)
        {
            foreach (CircleSymbol n in sharedObjects)
            {
                n.gameObject.transform.parent = this.transform;
            }

            transform.position += Vector3.up * 0.01f;
            rotato = 0;
            rotating = true;
            parent.inUse = true;
            ringPFX.StartPFX();
        }
    }

    private void Update()
    {
        if (rotating)
        {
            if (rotato + Time.deltaTime * speed <= 90)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * speed);
                rotato += Time.deltaTime * speed;
            }
            else if (rotato + Time.deltaTime * speed > 90)
            {
                transform.Rotate(Vector3.up * (90 - rotato));
                rotating = false;
                parent.inUse = false;
                parent.CheckComplete();
                transform.position -= Vector3.up * 0.01f;
                ringPFX.StopPFX();
            }
        }
    }

    public bool CheckFull()
    {
        Symbol temp = sharedObjects[0].symbol;
        if (temp == Symbol.Gray)
            return false;

        foreach (CircleSymbol n in sharedObjects)
        {
            if (n.symbol != temp)
            {
                return false;
            }
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CircleSymbol temp = other.gameObject.GetComponent<CircleSymbol>();

        if (temp && !sharedObjects.Contains(temp))
        {
            sharedObjects.Add(temp);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CircleSymbol temp = other.gameObject.GetComponent<CircleSymbol>();

        if (temp && sharedObjects.Contains(temp))
        {
            sharedObjects.Remove(temp);
        }
    }
}
