using MilasQuest.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MilasQuest.Stats
{
    public class LevelStats
    {
        public Action OnSuccess;
        public Action OnFailure;

        public Dictionary<STAT_TYPE, Stat> Stats { get; private set; }
        public Dictionary<CELL_TYPES, GatheredCellsStat> GatheredCells { get; private set; }

        private readonly List<EndLevelCondition> _successConditions;
        private readonly List<EndLevelCondition> _failureConditions;
        private ScoreSolver _scoreSolver;
        private int movesMultiplier = 1;

        public LevelStats (EndLevelData endLevelData, ScoreValuesData scoreValuesData)
        {
            Stats = new Dictionary<STAT_TYPE, Stat>();
            GatheredCells = new Dictionary<CELL_TYPES, GatheredCellsStat>();

            InitGeneralStat(STAT_TYPE.TOTAL_SCORE);
            InitGeneralStat(STAT_TYPE.TOTAL_GATHERED_CELLS);
            GeneralStatConditionData totalMoves = endLevelData.failureConditions.FirstOrDefault(condition => condition.statType == STAT_TYPE.TOTAL_MOVES);
            if (totalMoves != null)
            {
                movesMultiplier = -1;
                InitGeneralStat(STAT_TYPE.TOTAL_MOVES, totalMoves.targetValue);
            }
            else
            {
                movesMultiplier = 1;
                InitGeneralStat(STAT_TYPE.TOTAL_MOVES);
            }

            for (int i = 0; i < endLevelData.successConditions.gatheredCellsConditions.Count; i++)
            {
                InitGatheredCellsStat(endLevelData.successConditions.gatheredCellsConditions[i].cellType);
            }

            _successConditions = InitEndLevelConditions(endLevelData.successConditions.generalConditions);
            _successConditions = InitEndLevelConditions(endLevelData.successConditions.gatheredCellsConditions);
            _failureConditions = InitEndLevelConditions(endLevelData.failureConditions);

            _scoreSolver = new ScoreSolver(scoreValuesData);
        }

        public void ProcessStatModifier(StatModifier statModifier)
        {
            if (Stats.TryGetValue(statModifier.targetStat, out Stat stat))
            {
                stat.DoOperation(statModifier.operation, statModifier.value);
            }
        }
        public void ProcessAction(CELL_TYPES cellType, int count)
        {
            Stats[STAT_TYPE.TOTAL_GATHERED_CELLS].DoOperation(ARITHMETIC_OPERATOR.ADD, count);
            Stats[STAT_TYPE.TOTAL_SCORE].DoOperation(ARITHMETIC_OPERATOR.ADD, _scoreSolver.SolveScore(count));
            Stats[STAT_TYPE.TOTAL_MOVES].DoOperation(ARITHMETIC_OPERATOR.ADD, movesMultiplier);
            if (GatheredCells.TryGetValue(cellType, out GatheredCellsStat stat))
                stat.DoOperation(ARITHMETIC_OPERATOR.ADD, count);
        }

        private List<EndLevelCondition> InitEndLevelConditions(List<GeneralStatConditionData> conditionDatas)
        {
            List<EndLevelCondition> conditions = new List<EndLevelCondition>();
            for (int i = 0; i < conditionDatas.Count; i++)
            {
                if (conditionDatas[i].statType == STAT_TYPE.TOTAL_MOVES && movesMultiplier == -1) //if we are counting down from total moves, we have to change the runtime values for the end condition
                    conditions.Add(new EndLevelCondition(Stats[conditionDatas[i].statType], RELATIONAL_OPERATOR.LESS_OR_EQUAL_TO, 0));
                else
                    conditions.Add(new EndLevelCondition(Stats[conditionDatas[i].statType], conditionDatas[i].relationalOperator, conditionDatas[i].targetValue));
            }
            return conditions;
        }

        private List<EndLevelCondition> InitEndLevelConditions(List<GatheredCellsConditionData> conditionDatas)
        {
            List<EndLevelCondition> conditions = new List<EndLevelCondition>();
            for (int i = 0; i < conditionDatas.Count; i++)
            {
                GatheredCells[conditionDatas[i].cellType].SetMaxValue(conditionDatas[i].targetValue);
                conditions.Add(new EndLevelCondition(GatheredCells[conditionDatas[i].cellType], conditionDatas[i].relationalOperator, conditionDatas[i].targetValue));
            }
            return conditions;
        }

        private void InitGeneralStat(STAT_TYPE statType, float startingValue = 0)
        {
            Stat newStat = new Stat(statType, startingValue);
            newStat.OnStatUpdated += HandleOnStatUpdated;
            Stats.Add(statType, newStat);
        }

        private void InitGatheredCellsStat(CELL_TYPES cellType)
        {
            GatheredCellsStat newStat = new GatheredCellsStat(cellType);
            newStat.OnStatUpdated += HandleOnStatUpdated;
            GatheredCells.Add(cellType, newStat);
        }


        private void HandleOnStatUpdated(Stat stat)
        {
            CheckEndLevelConditions();
        }

        private void CheckEndLevelConditions()
        {
            for (int i = _successConditions.Count - 1; i >= 0; i--)
            {
                if (_successConditions[i].Evaluate())
                    _successConditions.RemoveAt(i);
            }
            if (_successConditions.Count == 0)
                OnSuccess?.Invoke();
            for (int i = 0; i < _failureConditions.Count; i++)
            {
                if (_failureConditions[i].Evaluate())
                    OnFailure?.Invoke();
            }
        }
    }
}