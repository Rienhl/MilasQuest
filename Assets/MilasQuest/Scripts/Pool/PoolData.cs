using UnityEngine;

namespace MilasQuest.Pools
{
    [CreateAssetMenu(fileName = "New Pool Data", menuName = "MilasQuest/Data/Pool")]
    public class PoolData : ScriptableObject
    {
        public GameObject prefab;
        public int initAmount = 20;
        public int increaseAmount = 5;
    }
}

