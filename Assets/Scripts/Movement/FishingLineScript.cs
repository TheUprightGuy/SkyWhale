using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLineScript : MonoBehaviour
{
    public GameObject lure;
    LineRenderer line;

    public Transform point0, point1, controlPoint;

    private int numPoints = 50;
    private Vector3[] positions = new Vector3[50];

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = numPoints;
        point0 = line.transform;
        point1 = lure.transform;
    }

    // Update is called once per frame
    void Update()
    {
        DrawQuadraticCurve();
    }

    void DrawQuadraticCurve()
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateQuadraticBezierPoint(t, point0.position, point1.position, controlPoint.position);
        }
        line.SetPositions(positions);
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 _point0, Vector3 _point1, Vector3 _controlPoint)
    {
        // B(t) = (1-t)^2 * Point0 + 2 * (1 - t) * t * point1 + t^ 2 * point2
        float u = 1 - t;
        Vector3 temp = (u * u) * _point0 + 2 * u * t * _controlPoint + (t * t) * _point1;

        return temp;
    }
}
