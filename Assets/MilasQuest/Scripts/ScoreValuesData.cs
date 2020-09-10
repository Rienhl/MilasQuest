using UnityEngine;

namespace MilasQuest
{
    [CreateAssetMenu(fileName = "New Score Values Data", menuName = "MilasQuest/Data/Create New Score Values Data")]
    public class ScoreValuesData : ScriptableObject
    {
        public int minLinkCount = 3;
        public int basicChainScore = 10;
        public float extraChainMultiplier;
    }
}