using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Island.CollectableIsland
{
    public class IslandChain : MonoBehaviour
    {
        public ChainController chainController;
        LineRenderer line;

        public Transform point0, point1, controlPoint;

        private int numPoints = 50;
        private Vector3[] positions = new Vector3[50];

        public int index;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.positionCount = numPoints;
            point0 = line.transform;
            point1 = chainController.GetPreviousIslandPosition(index);
            chainController.SetPreviousIslandPosition(point0, index);
        }

        // Update is called once per frame
        void Update()
        {
            DrawQuadraticCurve();
        }

        void DrawQuadraticCurve()
        {
            //Update control point to be between point0 and point1
            controlPoint.position = point0.position + ((point1.position - point0.position) * 0.5f) + new Vector3(0f, -20f, 0f);
            for (int i = 1; i < numPoints + 1; i++)
            {
                float t = i / (float)numPoints;
                positions[i - 1] = CalculateQuadraticBezierPoint(t, point0.position, point1.position, controlPoint.position);
            }
            line.SetPositions(positions);
        }

        Vector3 CalculateQuadraticBezierPoint(float t, Vector3 _point0, Vector3 _point1, Vector3 _controlPoint)
        {
            /*//u = B - A, v = C - A
            Vector3 u = _controlPoint - _point0;
            Vector3 v = _point1 - _point0;
            
            // w = Cross product of u and v
            Vector3 w = Vector3.Cross(u, v);
            w.Normalize();*/
            
            //Distance from origin to plane is dot product of w and 
            // B(t) = (1-t)^2 * Point0 + 2 * (1 - t) * t * point1 + t^ 2 * point2
            float u = 1 - t;
            Vector3 temp = (u * u) * _point0 + 2 * u * t * _controlPoint + (t * t) * _point1;

            return temp;
        }
    }
}