using UnityEngine;

namespace CollectableIsland
{
    public class IslandTrailing : MonoBehaviour
    {
        [Header("Follow Point Variables")]
        public Transform followPoint;
        public float minDistance = 20f;
        [Header("Island Variables")]
        public Rigidbody islandRigidBody;
        public float followStrength = 0.01f;
        public float yDistanceBelowIsland = 15.0f;
        //Constant variables
        private const float SlowingDistanceMultiplier = 1.75f;
        private const float MAXDistanceMultiplier = 2.25f;
        [Header("Bobbing Variables")]
        public float bobAmount = 5f;
        public float bobTime = 3f;

        private void Start()
        {
            //Start looping bobbing motion using LeanTween library
            var bobTween = LeanTween.moveY(gameObject, transform.position.y + bobAmount, bobTime).setEase(LeanTweenType.easeInOutQuad);
            bobTween.setLoopPingPong();
        }

        private void Update()
        {
            if (!followPoint) return;
            //Calculate distance to follow point
            var followPointVector = followPoint.position - (transform.position - Vector3.down * yDistanceBelowIsland);
            var distanceToFollowPoint = followPointVector.magnitude;
            var slowingDistance = minDistance * SlowingDistanceMultiplier;
            TrailFollowPoint(distanceToFollowPoint, slowingDistance, followPointVector);
        }

        /// <summary>
        /// Function checks whether the island should:
        ///     Slow to avoid colliding with the whale, add trailing force to trail behind the whale or do nothing. 
        /// </summary>
        /// <param name="distanceToFollowPoint">Distance from island to whale</param>
        /// <param name="slowingDistance">Distance to whale that the island should begin slowing</param>
        /// <param name="followPointVector">Vector between island and whale</param>
        private void TrailFollowPoint(float distanceToFollowPoint, float slowingDistance, Vector3 followPointVector)
        {
            if (distanceToFollowPoint < slowingDistance)
            {
                //Decrease velocity when approaching the minimum radius
                islandRigidBody.velocity *= distanceToFollowPoint / slowingDistance;
            }
            else
            {
                //Ensure follow point is sufficiently far away before applying trailing force
                if (!(distanceToFollowPoint > minDistance * MAXDistanceMultiplier))
                {
                    return;
                }

                //Add trailing force to move in the direction of the follow point
                islandRigidBody.AddForce(followPointVector * followStrength, ForceMode.Impulse);
            }
        }
    }
}
