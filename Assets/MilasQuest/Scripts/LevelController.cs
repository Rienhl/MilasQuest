using MilasQuest.GameData;
using MilasQuest.Grids;
using MilasQuest.InputManagement;
using MilasQuest.Stats;
using MilasQuest.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest
{
    public class LevelController : MonoBehaviour
    {
        public Action OnSuccess;
        public Action OnFailure;

        [SerializeField] private StatsPanel statsPanel;
        [SerializeField] private GridView gridView;

        private GridInputConversor _gridInputConversor;
        private GridState _grid;
        private LevelStats _levelStats;

        public void SetupLevel(InputHandler inputHandler, LevelData levelData)
        {
            _levelStats = new LevelStats(levelData.endLevelData, levelData.scoreValuesData);
            _levelStats.OnSuccess += HandleOnSuccess;
            _levelStats.OnFailure += HandleOnFailure;

            statsPanel.Setup(_levelStats);

            _grid = new GridState(levelData.gridConfigurationData);
            _grid.OnStartedUpdatingGrid += HandleOnStartedUpdatingGrid;

            gridView.Setup(_grid, levelData.gridViewSettings, statsPanel.ActiveGatheredCellsPanels);

            _gridInputConversor = new GridInputConversor(inputHandler, Camera.main);
            _gridInputConversor.SetGridView(gridView);
            _gridInputConversor.Enable(true);

            RegisterGridInputActions();
        }

        public void ResetLevel()
        {
            UnregisterGridInputActions();
            gridView.OnGridViewUpdated -= HandleGridViewUpdated;
            _levelStats.OnSuccess -= HandleOnSuccess;
            _levelStats.OnFailure -= HandleOnFailure;
            statsPanel.Unsetup();
            gridView.Unsetup();
        }

        private void HandleOnSuccess()
        {
            _gridInputConversor.Enable(false);
            UnregisterGridInputActions();
            OnSuccess?.Invoke();
        }

        private void HandleOnFailure()
        {
            _gridInputConversor.Enable(false);
            UnregisterGridInputActions();
            OnFailure?.Invoke();
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
            //This should handle what should happen with the input if the game gets sent to background while input is active (locking the phone, getting a call, etc...)
            throw new NotImplementedException();
        }
    }
}