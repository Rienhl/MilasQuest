using MilasQuest.Stats;
using System.Collections.Generic;

namespace MilasQuest.GameData
{
    [System.Serializable]
    public class EndLevelSuccessData
    {
        public List<GeneralStatConditionData> generalConditions = new List<GeneralStatConditionData>();
        public List<GatheredCellsConditionData> gatheredCellsConditions = new List<GatheredCellsConditionData>();
    }
}