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
        public float validInputRatio;
        public PoolData cellPoolData;
    }
}