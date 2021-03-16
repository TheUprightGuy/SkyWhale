using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Island.CollectableIsland
{
    public class IslandTrailing : MonoBehaviour
    {
        [Header("Follow Point Variables")]
        public Transform followPoint;
        public float minDistance = 20f;
        private Vector3 _flockingVector = Vector3.zero;
        public float flockingStrength = 1.0f;
        
        [Header("Island Variables")]
        public Rigidbody islandRigidBody;
        public float followStrength = 100f;
        public float yDistanceBelowIsland = 15.0f;
        
        //Constant variables
        private const float SlowingDistanceMultiplier = 1.75f;
        
        [Header("Bobbing Variables")]
        public float bobAmount = 0.5f;
        public float bobTime = 3f;

        [Header("Bobbing Variables")] 
        private LTDescr _bobTween;

        private void Start()
        {
            //Set follow point
            //Start looping bobbing motion using LeanTween library
            transform.position = new Vector3(transform.position.x, followPoint.position.y - yDistanceBelowIsland,
                transform.position.z);
            StartCoroutine(BobIsland(1f));
            //_rotateTween = RotateTowardsIsland();
            RotateTowardsIsland();
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
            RotateTowardsIsland();
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
                Vector3 forceDirection = followPointVector - _flockingVector;
                forceDirection.Normalize();
                islandRigidBody.AddForce(forceDirection.normalized * followStrength, ForceMode.Impulse);
            }
        }
        
        /// <summary>
        /// Causes the island to rotate towards its follow point
        /// </summary>
        /// <returns>Lean tween descriptor so the update function can tell when the tweening is complete</returns>
        private void RotateTowardsIsland()
        {
            //Calculate euler angle rotation required to rotate to face follow point
            var islandRotationVector = Vector3.RotateTowards(Vector3.one, followPoint.position, 1f, 10f);
            var transformRotation = Quaternion.LookRotation(islandRotationVector);
            transformRotation.x = 0f;
            transformRotation.z = 0f;
            transform.rotation = transformRotation;
        }

        public void Flock(List<GameObject> trailingIslands)
        {
            Vector3 separation = CalculateSeparation(trailingIslands) * 1.5f;

            Vector3 alignment = CalculateAlignment(trailingIslands);

            Vector3 cohesion = CalculateCohesion(trailingIslands);

            _flockingVector = separation + alignment + cohesion;
            _flockingVector.Normalize();
            _flockingVector *= flockingStrength;
        }

        private Vector3 CalculateCohesion(List<GameObject> trailingIslands)
        {
            //Calculate cohesion
            //Calculate average position
            float neighbourDist = 70;
            int count = 0;
            Vector3 sum = Vector3.zero;
            //Sum positions from neighbouring islands (within neighbour distance)
            //Ignore own position
            foreach (var trailingIsland in from trailingIsland in trailingIslands 
                where trailingIsland.GetInstanceID() != gameObject.GetInstanceID() 
                let distance = Vector3.Distance(transform.position, trailingIsland.transform.position) 
                where distance > 0 && distance < neighbourDist 
                select trailingIsland)
            {
                sum += trailingIsland.transform.position;
                count++;
            }

            if (count > 0)
            {
                //Divide by count to get average
                sum /= trailingIslands.Count;
            }
            
            //Return vector to average position
            return transform.position - sum;
        }

        private Vector3 CalculateAlignment(List<GameObject> trailingIslands)
        {
            //Calculate alignment
            //Calculate average velocity
            float neighbourDist = 70;
            int count = 0;
            Vector3 sum = Vector3.zero;
            //Sum velocities from neighbouring islands (within neighbour distance)
            //Ignore own velocity
            foreach (var trailingIsland in from trailingIsland in trailingIslands 
                where trailingIsland.GetInstanceID() != gameObject.GetInstanceID() 
                let distance = Vector3.Distance(transform.position, trailingIsland.transform.position) 
                where distance > 0 && distance < neighbourDist 
                select trailingIsland)
            {
                sum += trailingIsland.GetComponent<Rigidbody>().velocity;
                count++;
            }

            if (count > 0)
            {
                //Divide by count to get average
                sum /= trailingIslands.Count;
            }
            return sum;
        }

        private Vector3 CalculateSeparation(List<GameObject> trailingIslands)
        {
            //Calculate separation force
            float desiredSep = gameObject.transform.localScale.x * 50f / 3f;
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (var trailingIsland in trailingIslands)
            {
                if (trailingIsland.GetInstanceID() == gameObject.GetInstanceID()) continue;
                float distance = Vector3.Distance(transform.position, trailingIsland.transform.position);
                if (!(distance > 0) || !(distance < desiredSep)) continue;
                Vector3 difference = transform.position - trailingIsland.transform.position;
                difference.Normalize();
                difference /= distance;
                sum += difference;
                count++;
            }

            if (count > 0)
            {
                sum /= count;
            }

            return sum;
        }
    }
}
