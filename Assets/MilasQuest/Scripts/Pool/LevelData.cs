using MilasQuest.Grids.GameData;
using MilasQuest.Stats;
using UnityEngine;

namespace MilasQuest.GameData
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "MilasQuest/Data/Create New Level Data")]
    public class LevelData : BaseData
    {
        public GridSettings gridConfigurationData;
        public GridViewSettings gridViewSettings;
        public ScoreValuesData scoreValuesData;
        [HideInInspector]
        public EndLevelData endLevelData;
    }
}

