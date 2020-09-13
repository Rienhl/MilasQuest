using MilasQuest.Stats;

namespace MilasQuest.GameData
{
    [System.Serializable]
    public class GeneralStatConditionData
    {
        public STAT_TYPE statType;
        public RELATIONAL_OPERATOR relationalOperator;
        public float targetValue;
    }
}