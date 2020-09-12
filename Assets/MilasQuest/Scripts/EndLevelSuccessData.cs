using System.Collections.Generic;

namespace MilasQuest.Stats
{
    [System.Serializable]
    public class EndLevelSuccessData
    {
        public List<GeneralStatConditionData> generalConditions = new List<GeneralStatConditionData>();
        public List<GatheredCellsConditionData> gatheredCellsConditions = new List<GatheredCellsConditionData>();
    }
}