using MilasQuest.Grids.GameData;
using System;
using System.Collections.Generic;

namespace MilasQuest.Stats
{
    public class Stat
    {
        private float _currentValue;
        private float _prevValue;
        private STAT_TYPE _statType;

        public Action<Stat> OnStatUpdated;

        public Stat(STAT_TYPE statType, float startingValue = 0)
        {
            _statType = statType;
            _currentValue = startingValue;
        }

        public void DoOperation(ARITHMETIC_OPERATOR operation, float value)
        {
            _prevValue = _currentValue;
            switch (operation)
            {
                case ARITHMETIC_OPERATOR.ADD:
                    _currentValue += value;
                    break;
                case ARITHMETIC_OPERATOR.MULT:
                    _currentValue *= value;
                    break;
                case ARITHMETIC_OPERATOR.DIV:
                    _currentValue /= value;
                    break;
                default:
                    break;
            }

            if (_currentValue == _prevValue)
                return;

            OnOperationDone();
        }

        public float GetCurrentValue()
        {
            return _currentValue;
        }
        public STAT_TYPE GetStatType()
        {
            return _statType;
        }

        protected virtual void OnOperationDone()
        {
            OnStatUpdated?.Invoke(this);
        }

    }

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
            return StatUtils.DoRelationalOperation(_targetStat.GetCurrentValue(), _targetValue, _op);
        }
    }

    //No need to override GetHashCode nor Equals since this will not be checked for equality (so far)
    public struct StatModifier
    {
        public STAT_TYPE targetStat;
        public ARITHMETIC_OPERATOR operation;
        public float value;
    }

    public class EndLevelConditionData
    {
        public STAT_TYPE statType;
        public RELATIONAL_OPERATOR relationalOperator;
        public float targetValue;
    }

    public class LevelStats
    {
        public Dictionary<STAT_TYPE, Stat> Stats { get; private set; }

        public Dictionary<STAT_TYPE, List<Action<Stat>>> OnStatUpdatedListeners; //pseudo observer pattern implementation

        public delegate void OnStatUpdated(Stat stat);

        public LevelStats ()
        {
            Stats = new Dictionary<STAT_TYPE, Stat>();
            OnStatUpdatedListeners = new Dictionary<STAT_TYPE, List<Action<Stat>>>();
            foreach (STAT_TYPE statType in Enum.GetValues(typeof(STAT_TYPE)))
            {
                InitStat(statType);
            }
        }

        public void ClearStats()
        {
            foreach(Stat stat in Stats.Values)
            {
                stat.OnStatUpdated -= HandleOnStatUpdated;
            }

            OnStatUpdatedListeners.Clear();
            Stats.Clear();
        }

        public void ProcessStatModifier(StatModifier statModifier)
        {
            if (Stats.TryGetValue(statModifier.targetStat, out Stat stat))
            {
                stat.DoOperation(statModifier.operation, statModifier.value);
            }
        }

        private void InitStat(STAT_TYPE statType)
        {
            Stat newStat = new Stat(statType);
            newStat.OnStatUpdated += HandleOnStatUpdated;
            Stats.Add(statType, newStat);
            OnStatUpdatedListeners.Add(statType, new List<Action<Stat>>());
        }

        private void HandleOnStatUpdated(Stat stat)
        {
            if (OnStatUpdatedListeners.TryGetValue(stat.GetStatType(), out List<Action<Stat>> callbacks))
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    callbacks[i]?.Invoke(stat);
                }
            }
        }
    }

    public class GatheredCellsEndLevelConditionData : EndLevelConditionData
    {
        public CellTypeData cellTypeData;
    }

    public enum STAT_TYPE
    {
        REMAINING_MOVES,
        GATHERED_CELLS_OF_TYPE,
        TOTAL_SCORE
    }

    public enum RELATIONAL_OPERATOR
    {
        EQUAL,
        NOT_EQUAL,
        GREATER_THAN,
        LESS_THAN,
        GREATER_OR_EQUAL_TO,
        LESS_OR_EQUAL_TO
    }

    public static class StatUtils
    {
        public static bool DoRelationalOperation(float a, float b, RELATIONAL_OPERATOR op)
        {
            bool result = false;
            switch (op)
            {
                case RELATIONAL_OPERATOR.EQUAL:
                    result = a == b;
                    break;
                case RELATIONAL_OPERATOR.NOT_EQUAL:
                    result = a != b;
                    break;
                case RELATIONAL_OPERATOR.GREATER_THAN:
                    result = a > b;
                    break;
                case RELATIONAL_OPERATOR.LESS_THAN:
                    result = a < b;
                    break;
                case RELATIONAL_OPERATOR.GREATER_OR_EQUAL_TO:
                    result = a >= b;
                    break;
                case RELATIONAL_OPERATOR.LESS_OR_EQUAL_TO:
                    result = a <= b;
                    break;
                default:
                    break;
            }
            return result;
        }
    }

}