using System.Collections;
using UnityEngine;

namespace Island.CollectableIsland
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
        
        [Header("Bobbing Variables")]
        public float bobAmount = 0.5f;
        public float bobTime = 3f;

        [Header("Bobbing Variables")] 
        public float rotationTime = 0.5f;
        private LTDescr _rotateTween;
        private LTDescr _bobTween;

        private void Start()
        {
            //Start looping bobbing motion using LeanTween library
            transform.position = new Vector3(transform.position.x, followPoint.position.y - yDistanceBelowIsland,
                transform.position.z);
            StartCoroutine(BobIsland(1f));
            _rotateTween = RotateTowardsIsland();
        }

        /// <summary>
        /// Recursive coroutine which causes the island to bob around its follow point's height
        /// </summary>
        /// <param name="direction">Whether its currently bobbing upwards or downwards (Alternates every function call)</param>
        /// <returns>After a set amount of seconds (bob time)</returns>
        private IEnumerator BobIsland(float direction)
        {
            _bobTween = LeanTween.moveY(gameObject, followPoint.position.y + bobAmount * direction - yDistanceBelowIsland, bobTime)
                .setEase(LeanTweenType.easeInOutQuad);
            yield return new WaitForSeconds(bobTime);
            StartCoroutine(BobIsland(direction*-1f));
        }

        private void Update()
        {
            if (!followPoint) return;
            //Calculate distance to follow point
            Vector3 followPointVector =
                followPoint.position - (transform.position - Vector3.down * yDistanceBelowIsland);
            float distanceToFollowPoint = followPointVector.magnitude;
            float slowingDistance = minDistance * SlowingDistanceMultiplier;
            TrailFollowPoint(distanceToFollowPoint, slowingDistance, followPointVector);
            
            //Rotate to face island
            if (!LeanTween.isTweening(_rotateTween.id))
            {
                _rotateTween = RotateTowardsIsland();
            }
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
                //Add trailing force to move in the direction of the follow point
                islandRigidBody.AddForce(followPointVector * followStrength, ForceMode.Impulse);
            }
        }
        
        /// <summary>
        /// Causes the island to rotate towards its follow point
        /// </summary>
        /// <returns>Lean tween descriptor so the update function can tell when the tweening is complete</returns>
        private LTDescr RotateTowardsIsland()
        {
            //Calculate euler angle rotation required to rotate to face follow point
            var islandRotationVector = Vector3.RotateTowards(Vector3.one, followPoint.position, 7f, 100f);
            var islandRotationEuler = Quaternion.FromToRotation(transform.forward, islandRotationVector).eulerAngles;
            
            //Ignore rotation on x and z axis
            islandRotationEuler.x = 0f;
            islandRotationEuler.z = 0f;
            
            //Start rotation tweening utilizing lean tween (rotates over the rotation time)
            return LeanTween.rotate(this.gameObject, islandRotationEuler, rotationTime);
        }
    }
}
