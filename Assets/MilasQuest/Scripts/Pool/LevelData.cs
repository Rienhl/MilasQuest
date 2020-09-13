using MilasQuest.Grids.GameData;
using MilasQuest.Stats;
using UnityEngine;

namespace MilasQuest.GameData
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "MilasQuest/Data/Create New Level Data")]
    public class LevelData : BaseData
    {
        public GridViewSettings gridViewSettings;
        public ScoreValuesData scoreValuesData;
        public GridSettings gridConfigurationData;
        [HideInInspector]
        public EndLevelData endLevelData;
    }
}

