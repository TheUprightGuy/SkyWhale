// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
// (c) 2020 Media Design School
// File Name   :   IslandChain.cs
// Description :   Mono behaviour that handles setting the chains start, end and control point, calculating all
//                 the other points along a quadratic curve and updating the line renderer to render the chain using
//                 these points.
// Author      :   Jacob Gallagher
// Mail        :   Jacob.Gallagher1.@mds.ac.nz
using UnityEngine;

namespace Island.CollectableIsland
{
    public class IslandChain : MonoBehaviour
    {
        private LineRenderer _line;

        public Transform point0, point1, controlPoint;

        private const int NumPoints = 50;
        private readonly Vector3[] _positions = new Vector3[50];

        public int index;    //Which chain on the island is this (as island has 2 chains this can be 0 or 1)

        /// <summary>
        /// Set up variables and update previous island position in chain controller
        /// </summary>
        private void Start()
        {
            _line = GetComponent<LineRenderer>();
            _line.positionCount = NumPoints;
            point0 = _line.transform;
            point1 = ChainController.instance.GetPreviousIslandPosition(index);
            ChainController.instance.SetPreviousIslandPosition(point0, index);
        }

        // Update is called once per frame
        private void Update()
        {
            DrawQuadraticCurve();
        }

        /// <summary>
        /// Calculate and draw the chains curve using both the points as anchors and
        /// a point below the middle of both of them as the control point
        /// </summary>
        private void DrawQuadraticCurve()
        {
            //Update control point to be between point0 and point1
            //Move this point downwards on the y axis
            controlPoint.position = point0.position + ((point1.position - point0.position) * 0.5f) + new Vector3(0f, -20f, 0f);
            for (int i = 1; i < NumPoints + 1; i++)
            {
                float t = i / (float)NumPoints;
                _positions[i - 1] = CalculateQuadraticBezierPoint(t, point0.position, point1.position, controlPoint.position);
            }
            _line.SetPositions(_positions);
        }

        /// <summary>
        /// Calculate a specific point along the chains curve
        /// </summary>
        /// <param name="t">Point on the chain</param>
        /// <param name="_point0">Connect point 0(Current island's chain connect point)</param>
        /// <param name="_point1">Connect point 1(Previous island's or whale's chain connect point)</param>
        /// <param name="_controlPoint">Lowest point on the chain(midway point between connect points)</param>
        /// <returns>Vector3 position of point t (To be used by the line renderer)</returns>
        Vector3 CalculateQuadraticBezierPoint(float t, Vector3 _point0, Vector3 _point1, Vector3 _controlPoint)
        {
            // B(t) = (1-t)^2 * Point0 + 2 * (1 - t) * t * point1 + t^ 2 * point2
            float u = 1 - t;
            Vector3 temp = (u * u) * _point0 + 2 * u * t * _controlPoint + (t * t) * _point1;

            return temp;
        }
    }
}