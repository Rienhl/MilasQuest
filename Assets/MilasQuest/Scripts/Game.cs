using MilasQuest.GameData;
using MilasQuest.Grids;
using MilasQuest.InputManagement;
using MilasQuest.Pools;
using MilasQuest.Stats;
using MilasQuest.UI;
using System;
using System.Collections;
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

        private void Start()
        {
            for (int i = 0; i < _pools.Length; i++)
            {
                Pool.CreatePool(_pools[i]);
            }
            _inputHandler = SolveInputHandler();

            _levelStats = new LevelStats(_levelData.endLevelData, _levelData.scoreValuesData);
            statsPanel.Setup(_levelStats);

            _grid = new GridState(_levelData.gridConfigurationData);
            _grid.OnStartedUpdatingGrid += HandleOnStartedUpdatingGrid;
            StartCoroutine(WaitFrameAndContinueSetup());
        }

        private IEnumerator WaitFrameAndContinueSetup()
        {
            yield return new WaitForEndOfFrame();
            gridView.Setup(_grid, _levelData.gridViewSettings, statsPanel.ActiveGatheredCellsPanels);
            _gridInputConversor = new GridInputConversor(_inputHandler, gridView, Camera.main);
            _gridInputConversor.Enable(true);
            RegisterGridInputActions();
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
            _levelStats.ProcessAction((CELL_TYPES)removedCells[0].CellType.id, removedCells.Count);
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