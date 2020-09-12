using MilasQuest.Grids.GameData;
using MilasQuest.Stats;
using UnityEngine;

namespace MilasQuest.GameData
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "MilasQuest/Data/Create New Level Data")]
    public class LevelData : BaseData
    {
        public GridConfigurationData gridConfigurationData;
        public GridViewConfigurationData gridViewConfigurationData;
        public ScoreValuesData scoreValuesData;
        [HideInInspector]
        public EndLevelData endLevelData;
    }
}

