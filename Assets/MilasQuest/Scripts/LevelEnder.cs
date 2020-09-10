using MilasQuest.Stats;
using UnityEngine;

namespace MilasQuest
{
    public class LevelEnder
    {
        private EndLevelCondition[] _successConditions;
        private EndLevelCondition[] _failureConditions;
        private LevelStats _levelStats;

        public LevelEnder(EndLevelData endLevelData, LevelStats levelStats)
        {
            _levelStats = levelStats;
            _successConditions = new EndLevelCondition[endLevelData.successConditions.Length];
            _failureConditions = new EndLevelCondition[endLevelData.failureConditions.Length];

            for (int i = 0; i < endLevelData.successConditions.Length; i++)
            {
                _successConditions[i] = new EndLevelCondition(levelStats.Stats[endLevelData.successConditions[i].statType], endLevelData.successConditions[i].relationalOperator, endLevelData.successConditions[i].targetValue);
                _levelStats.OnStatUpdatedListeners[endLevelData.successConditions[i].statType].Add(HandleTrackedStatUpdate);
            }

            for (int i = 0; i < endLevelData.failureConditions.Length; i++)
            {
                _failureConditions[i] = new EndLevelCondition(levelStats.Stats[endLevelData.failureConditions[i].statType], endLevelData.failureConditions[i].relationalOperator, endLevelData.failureConditions[i].targetValue);
                _levelStats.OnStatUpdatedListeners[endLevelData.failureConditions[i].statType].Add(HandleTrackedStatUpdate);

            }
        }

        private void HandleTrackedStatUpdate(Stat _)
        {
            bool success = true;
            for (int i = 0; i < _successConditions.Length; i++)
            {
                if (!_successConditions[i].Evaluate())
                {
                    success = false;
                    break;
                }
            }
            if (success == true)
            {
                Debug.Log("SUCCESS");
            }
            else
            {
                for (int i = 0; i < _failureConditions.Length; i++)
                {
                    if (_failureConditions[i].Evaluate())
                    {
                        Debug.Log("FAILED");
                        break;
                    }
                }
            }
        }
    }
}