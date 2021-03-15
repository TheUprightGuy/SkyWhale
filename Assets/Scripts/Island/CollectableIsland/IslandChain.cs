using UnityEngine;

namespace Island.CollectableIsland
{
    public class IslandChain : MonoBehaviour
    {
        public ChainController chainController;
        private LineRenderer _line;

        public Transform point0, point1, controlPoint;

        private const int NumPoints = 50;
        private readonly Vector3[] _positions = new Vector3[50];

        public int index;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _line.positionCount = NumPoints;
            point0 = _line.transform;
            point1 = chainController.GetPreviousIslandPosition(index);
            chainController.SetPreviousIslandPosition(point0, index);
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