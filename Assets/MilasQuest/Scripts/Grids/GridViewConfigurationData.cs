using MilasQuest.Pools;
using UnityEngine;

namespace MilasQuest.Grids.GameData
{
    /// <summary>
    /// Contains any visual configuration for our current grid state
    /// This allows for different grid views applied to the same grid state
    /// </summary>
    [CreateAssetMenu(fileName = "New Grid View Configuration Data", menuName = "MilasQuest/Data/Create New Grid View Configuration Data")]
    public class GridViewConfigurationData : ScriptableObject
    {
        public float cellSize;
        [Range(0,1)]
        public float cellContentScale;
        public float validInputRatio;
        public float cellDestructionDelay = 0.2f;
        public float backgroundConstructionDelay = 0.1f;
        public PoolData cellPoolData;
        public PoolData[] backgroundTilesPoolDatas;
    }
}