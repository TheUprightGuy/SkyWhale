using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Island.CollectableIsland
{
    public class ChainController : MonoBehaviour
    {
        #region Singleton
        public static ChainController instance;
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than one ChainController exists!");
                Destroy(this);
            }
            else
            {
                instance = this;
                //OnAwake();
            }
        }
        #endregion Singleton
        public Transform previousIslandPosition1;
        public Transform previousIslandPosition2;

        /// <summary>
        /// Returns the transform of the connect points from the last island trailing the whale
        /// (this is the whale's connect points if there are no trailing islands)
        /// </summary>
        /// <param name="index">The index of the chain (O is for the left chain and 1 for the right chain)
        /// This determines whether it returns the transform for the left side or the right side
        /// </param>
        /// <returns>The appropriate connect point transform</returns>
        public Transform GetPreviousIslandPosition(int index)
        {
            switch (index)
            {
                //return previous island's chain connect positions (this is the whale connect points if no islands have been connected)
                case 0:
                    return previousIslandPosition1;
                case 1:
                    return previousIslandPosition2;
            }

            return null;
        }
        
        /// <summary>
        /// Updates the stored transforms to be the last island connected
        /// </summary>
        /// <param name="position">The position of one of the connect points for the connecting island</param>
        /// <param name="index">The index of the chain point</param>
        public void SetPreviousIslandPosition(Transform position, int index)
        {
            switch (index)
            {
                //Updates  previous island's chain connect positions
                case 0:
                    previousIslandPosition1 = position;
                    break;
                case 1:
                    previousIslandPosition2 = position;
                    break;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                CallbackHandler.instance.SpawnCollectableIsland();
            }
        }
    }
}