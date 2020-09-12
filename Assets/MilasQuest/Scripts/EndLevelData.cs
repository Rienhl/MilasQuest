using System.Collections.Generic;

namespace MilasQuest.Stats
{
    [System.Serializable]
    public class EndLevelData
    {
        public EndLevelSuccessData successConditions;
        public List<GeneralStatConditionData> failureConditions = new List<GeneralStatConditionData>();
    }
}