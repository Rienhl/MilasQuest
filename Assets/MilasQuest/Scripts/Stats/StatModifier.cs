namespace MilasQuest.Stats
{
    //No need to override GetHashCode nor Equals since this will not be checked for equality (so far)
    public struct StatModifier
    {
        public STAT_TYPE targetStat;
        public ARITHMETIC_OPERATOR operation;
        public float value;
    }
}