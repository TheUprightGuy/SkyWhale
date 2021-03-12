using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Island.CollectableIsland
{
    public class ChainController : MonoBehaviour
    {
        public Transform previousIslandPosition1;
        public Transform previousIslandPosition2;

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
        public void SetPreviousIslandPosition(Transform position, int index)
        {
            switch (index)
            {
                //return previous island's chain connect positions (this is the whale connect points if no islands have been connected)
                case 0:
                    previousIslandPosition1 = position;
                    break;
                case 1:
                    previousIslandPosition2 = position;
                    break;
            }
        }
    }
}