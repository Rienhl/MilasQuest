using MilasQuest.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

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
        private readonly ScoreSolver _scoreSolver;
        private readonly int movesMultiplier = 1;
        private int _statsToUpdate = 0;
        /// <summary>
        /// Each stat type has different conditions for initialization.
        /// This should be broken down into smaller classes that handle how we want to manage each stat depending on the level's objectives and fail conditions
        /// </summary>
        /// <param name="endLevelData"></param>
        /// <param name="scoreValuesData"></param>
        public LevelStats (EndLevelData endLevelData, ScoreValuesData scoreValuesData)
        {
            Stats = new Dictionary<STAT_TYPE, Stat>();
            GatheredCells = new Dictionary<CELL_TYPES, GatheredCellsStat>();

            InitGeneralStat(STAT_TYPE.TOTAL_SCORE);
            
            //Initializing the moves stat with a target value means that this level has a limit on the amount of moves
            //While this might happen always, we could have bonus or tutorial levels with no limit on moves
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

            //Total gathered cells is a simple internal stat
            InitGeneralStat(STAT_TYPE.TOTAL_GATHERED_CELLS);

            //Here we initialize each gathered cell stat that should be tracked on this level
            for (int i = 0; i < endLevelData.successConditions.gatheredCellsConditions.Count; i++)
            {
                InitGatheredCellsStat(endLevelData.successConditions.gatheredCellsConditions[i].cellType);
            }

            _successConditions = new List<EndLevelCondition>();
            _successConditions.AddRange(InitEndLevelConditions(endLevelData.successConditions.generalConditions));
            _successConditions.AddRange(InitEndLevelConditions(endLevelData.successConditions.gatheredCellsConditions));

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

        public void ProcessStatModifiers(List<StatModifier> statModifiers)
        {
            for (int i = 0; i < statModifiers.Count; i++)
            {
                ProcessStatModifier(statModifiers[i]);
            }
        }

        public void ProcessAction(CELL_TYPES cellType, int count)
        {
            List<StatModifier> statModifiers = new List<StatModifier>();
            statModifiers.Add(new StatModifier() { targetStat = STAT_TYPE.TOTAL_GATHERED_CELLS, operation = ARITHMETIC_OPERATOR.ADD, value = count });
            statModifiers.Add(new StatModifier() { targetStat = STAT_TYPE.TOTAL_SCORE, operation = ARITHMETIC_OPERATOR.ADD, value = _scoreSolver.SolveScore(count) });
            statModifiers.Add(new StatModifier() { targetStat = STAT_TYPE.TOTAL_MOVES, operation = ARITHMETIC_OPERATOR.ADD, value = movesMultiplier });
            _statsToUpdate = 3;
            if (GatheredCells.TryGetValue(cellType, out GatheredCellsStat stat))
            {
                _statsToUpdate++;
                stat.DoOperation(ARITHMETIC_OPERATOR.ADD, count);
            }
            ProcessStatModifiers(statModifiers);
        }

        private List<EndLevelCondition> InitEndLevelConditions(List<GeneralStatConditionData> conditionDatas)
        {
            List<EndLevelCondition> conditions = new List<EndLevelCondition>();
            for (int i = 0; i < conditionDatas.Count; i++)
            {
                if (conditionDatas[i].statType == STAT_TYPE.TOTAL_MOVES && movesMultiplier == -1) //if we are counting down from total moves, we have to change the runtime values for the end condition
                    conditions.Add(new EndLevelCondition(Stats[STAT_TYPE.TOTAL_MOVES], RELATIONAL_OPERATOR.LESS_OR_EQUAL_TO, 0));
                else
                    conditions.Add(new EndLevelCondition(Stats[conditionDatas[i].statType], conditionDatas[i].relationalOperator, conditionDatas[i].targetValue));

                if (conditionDatas[i].statType == STAT_TYPE.TOTAL_SCORE) //if we have a condition data targeting our score, add that value as the max value of our score stat
                    Stats[STAT_TYPE.TOTAL_SCORE].SetMaxValue(conditionDatas[i].targetValue);
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
            _statsToUpdate--;
            if (_statsToUpdate > 0)
                return;
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