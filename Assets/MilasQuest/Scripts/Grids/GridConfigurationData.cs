using MilasQuest.Grids.LinkableRules;
using UnityEngine;

namespace MilasQuest.Grids.GameData
{
    /// <summary>
    /// Contains all the necessary information for setting up a grid, independent from it's visuals
    /// </summary>
    [CreateAssetMenu(fileName = "New Grid Configuration Data", menuName = "MilasQuest/Data/Create New Grid Configuration Data")]
    public class GridConfigurationData : ScriptableObject
    {
        public PointInt2D gridDimension;
        public CELL_LINKING_RULE[] linkingRules;
        public CellTypeData[] cellTypes;
    }
}