using System;
using UnityEngine;

namespace MilasQuest.Stats
{
    public class Stat
    {
        public float CurrentValue { get; protected set; }
        public float MinValue { get; protected set; }
        public float MaxValue { get; protected set; }
        public float PrevValue { get; private set; }
        public STAT_TYPE StatType { get; private set; }


        public Action<Stat> OnStatUpdated;
        public Action<Stat> OnStatReachedMin;
        public Action<Stat> OnStatReachedMax;

        public Stat(STAT_TYPE statType, float startingValue = 0, float minValue = Mathf.NegativeInfinity, float maxValue = Mathf.Infinity)
        {
            StatType = statType;
            CurrentValue = startingValue;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public void SetMinValue(float minValue)
        {
            MinValue = minValue;
        }

        public void SetMaxValue(float maxValue)
        {
            MaxValue = maxValue;
        }

        public void DoOperation(ARITHMETIC_OPERATOR operation, float value)
        {
            PrevValue = CurrentValue;
            switch (operation)
            {
                case ARITHMETIC_OPERATOR.ADD:
                    CurrentValue += value;
                    break;
                case ARITHMETIC_OPERATOR.MULT:
                    CurrentValue *= value;
                    break;
                case ARITHMETIC_OPERATOR.DIV:
                    CurrentValue /= value;
                    break;
                default:
                    break;
            }

            if (CurrentValue == PrevValue)
                return;

            OnOperationDone();
        }

        protected virtual void OnOperationDone()
        {
            if (CurrentValue >= MaxValue)
            {
                CurrentValue = MaxValue;
                OnStatReachedMax?.Invoke(this);
            }

            if (CurrentValue <= MinValue)
            {
                CurrentValue = MinValue;
                OnStatReachedMin?.Invoke(this);
            }
            OnStatUpdated?.Invoke(this);
        }
    }
}