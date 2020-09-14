using MilasQuest.Pools;
using UnityEngine;

namespace MilasQuest.Grids.GameData
{
    /// <summary>
    /// Contains any visual configuration for our current grid state
    /// This allows for different grid views applied to the same grid state
    /// </summary>
    [CreateAssetMenu(fileName = "New Grid View Settings Data", menuName = "MilasQuest/Data/Create New Grid View Settings Data")]
    public class GridViewSettings : ScriptableObject
    {
        public float cellSize;
        [Range(0, 1)]
        public float cellContentScale;
        [Range(0, 1)]
        public float validInputRatio;
        public float cellDestructionDelay = 0.2f;
        public float backgroundConstructionDelay = 0.1f;
        public PoolData cellPoolData;
        public PoolData[] backgroundTilesPoolDatas;
    }
}