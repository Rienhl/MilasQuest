using MilasQuest.Stats;
using System.Collections.Generic;

namespace MilasQuest.GameData
{
    [System.Serializable]
    public class EndLevelData
    {
        public EndLevelSuccessData successConditions;
        public List<GeneralStatConditionData> failureConditions = new List<GeneralStatConditionData>();
    }
}