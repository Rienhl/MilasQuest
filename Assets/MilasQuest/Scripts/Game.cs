using MilasQuest.GameData;
using MilasQuest.Grids;
using MilasQuest.InputManagement;
using MilasQuest.Pools;
using MilasQuest.Stats;
using System;
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

    public class Game : MonoBehaviour
    {
        [SerializeField]
        private GridView gridView;

        [SerializeField]
        private PoolData[] _pools;
        [SerializeField]
        private LevelData _levelData;

        private InputHandler _inputHandler;
        private GridInputConversor _gridInputConversor;
        private GridState _grid;
        private LevelStats _levelStats;
        private LevelEnder _levelEnder;

        private void Start()
        {
            for (int i = 0; i < _pools.Length; i++)
            {
                Pool.CreatePool(_pools[i]);
            }
            _grid = new GridState(_levelData.gridConfigurationData);
            _grid.OnStartedUpdatingGrid += HandleOnStartedUpdatingGrid;
            gridView.Init(_grid, _levelData.gridViewConfigurationData);
            _inputHandler = SolveInputHandler();
            _gridInputConversor = new GridInputConversor(_inputHandler, gridView, Camera.main);
            _gridInputConversor.Enable(true);
            RegisterGridInputActions();
            _levelStats = new LevelStats();
            _levelEnder = new LevelEnder(_levelData.endLevelData, _levelStats);
            _levelStats.OnStatUpdatedListeners[STAT_TYPE.TOTAL_SCORE].Add((Stat s) => Debug.Log(s.GetCurrentValue()));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _grid.CheckForDeadlock();
            }
        }

        private void RegisterGridInputActions()
        {
            _gridInputConversor.OnGridInputStarted += HandleOnGridInputStarted;
            _gridInputConversor.OnGridInputUpdated += HandleOnGridInputUpdated;
            _gridInputConversor.OnGridInputEnded += HandleOnGridInputEnded;
            _gridInputConversor.OnGridInputCancelled += HandleOnGridInputCancelled;
        }

        private void UnregisterGridInputActions()
        {
            _gridInputConversor.OnGridInputStarted -= HandleOnGridInputStarted;
            _gridInputConversor.OnGridInputUpdated -= HandleOnGridInputUpdated;
            _gridInputConversor.OnGridInputEnded -= HandleOnGridInputEnded;
            _gridInputConversor.OnGridInputCancelled -= HandleOnGridInputCancelled;
        }

        private void HandleOnGridInputStarted(PointInt2D newPoint)
        {
            _grid.AddCellAtPoint(newPoint);
        }

        private void HandleOnGridInputUpdated(PointInt2D newPoint)
        {
            _grid.AddCellAtPoint(newPoint);
        }

        private void HandleOnGridInputEnded(PointInt2D newPoint)
        {
            _levelStats.ProcessStatModifier(new StatModifier() { operation = ARITHMETIC_OPERATOR.ADD, targetStat = STAT_TYPE.TOTAL_SCORE, value = 5 });
            _grid.ProcessCurrentLink();
        }

        private void HandleOnStartedUpdatingGrid()
        {
            UnregisterGridInputActions();
            gridView.OnGridViewUpdated += HandleGridViewUpdated;
        }

        private void HandleGridViewUpdated()
        {
            gridView.OnGridViewUpdated -= HandleGridViewUpdated;
            //gridView.SetViewResponsiveness(false);
            _grid.CheckForDeadlock();
            //gridView.SetViewResponsiveness(true);
            RegisterGridInputActions();
        }

        private void HandleOnGridInputCancelled()
        {
            throw new NotImplementedException();
        }

        public InputHandler SolveInputHandler()
        {
#if UNITY_EDITOR
            return gameObject.AddComponent<MouseInputHandler>();
#else
            return gameObject.AddComponent<TouchInputHandler>()
#endif
        }
    }
}