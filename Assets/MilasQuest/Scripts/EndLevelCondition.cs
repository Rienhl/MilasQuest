namespace MilasQuest.Stats
{
    public class EndLevelCondition
    {
        Stat _targetStat;
        RELATIONAL_OPERATOR _op;
        float _targetValue;

        public EndLevelCondition(Stat targetStat, RELATIONAL_OPERATOR op, float targetValue)
        {
            _targetStat = targetStat;
            _op = op;
            _targetValue = targetValue;
        }

        public bool Evaluate()
        {
            return DoEvaluate();
        }

        protected virtual bool DoEvaluate()
        {
            return StatUtils.DoRelationalOperation(_targetStat.CurrentValue, _targetValue, _op);
        }
    }
}