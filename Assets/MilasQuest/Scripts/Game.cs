using MilasQuest.GameData;
using MilasQuest.Grids;
using MilasQuest.InputManagement;
using MilasQuest.Pools;
using MilasQuest.Stats;
using MilasQuest.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private StatsPanel statsPanel;

        [SerializeField] private GridView gridView;
        [SerializeField] private PoolData[] _pools;
        [SerializeField] private LevelData _levelData;

        private InputHandler _inputHandler;
        private GridInputConversor _gridInputConversor;
        private GridState _grid;
        private LevelStats _levelStats;
        private LevelEnder _levelEnder;
        private ScoreSolver _scoreSolver;

        private StatModifier _moveConsumedModifier;

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
            _scoreSolver = new ScoreSolver(_levelData.scoreValuesData);
            _moveConsumedModifier = new StatModifier() { targetStat = STAT_TYPE.TOTAL_MOVES, operation = ARITHMETIC_OPERATOR.ADD, value = 1 };
            statsPanel.Setup(_levelStats);
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
            _grid.ProcessCurrentLink();
        }

        private void HandleOnStartedUpdatingGrid(List<Cell> removedCells)
        {
            _levelStats.ProcessStatModifier(new StatModifier() { targetStat = STAT_TYPE.TOTAL_SCORE, operation = ARITHMETIC_OPERATOR.ADD, value = _scoreSolver.SolveScore(removedCells)});
            _levelStats.ProcessStatModifier(_moveConsumedModifier);
            _levelStats.ProcessStatModifier(new StatModifier() { targetStat = STAT_TYPE.GATHERED_CELLS_OF_TYPE, operation = ARITHMETIC_OPERATOR.ADD, value = removedCells.Count });
            UnregisterGridInputActions();
            gridView.OnGridViewUpdated += HandleGridViewUpdated;
        }

        private void HandleGridViewUpdated()
        {
            gridView.OnGridViewUpdated -= HandleGridViewUpdated;
            _grid.CheckForDeadlock();
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