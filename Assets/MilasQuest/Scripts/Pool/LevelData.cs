using MilasQuest.Grids.GameData;
using UnityEngine;

namespace MilasQuest.GameData
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "MilasQuest/Data/Create New Level Data")]
    public class LevelData : ScriptableObject
    {
        public GridConfigurationData gridConfigurationData;
        public GridViewConfigurationData gridViewConfigurationData;
    }
}

