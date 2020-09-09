using MilasQuest.Grids.GameData;
using MilasQuest.Pools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest.Grids
{
    /// <summary>
    /// this class contains the view part of the grid. It handles each cells visual representation, animations
    /// and is used as information source for input processing.
    /// Before creating other types of views, this should be converted into an abstract class
    /// </summary>
    public class GridView : MonoBehaviour
    {
        public Bounds GridBounds { get; private set; }
        public float CellSize { get; private set; }
        public float ActiveInputRadius { get; private set; }
        
        private float _cellHalfSize;
        private GridState _gridState;
        private List<CellView> _cellViews;
        private GridViewConfigurationData _gridViewConfigurationData;
        private Queue<CellView> removalQueue;
        private Queue<CellView> movementQueue;

        private int _movingCellsCount = 0;
        private float accumDelay = 0;
        private int _cellsToRemove = 0;
        public Action OnGridViewUpdated;

        public void Init(GridState grid, GridViewConfigurationData gridConfig)
        {
            removalQueue = new Queue<CellView>();
            movementQueue = new Queue<CellView>();
            _gridViewConfigurationData = gridConfig;
            _gridState = grid;
            CellSize = _gridViewConfigurationData.cellSize;
            _cellHalfSize = _gridViewConfigurationData.cellSize * 0.5f;
            ActiveInputRadius = _cellHalfSize * _gridViewConfigurationData.validInputRatio;
            GridBounds = new Bounds(Vector3.zero, new Vector3(grid.Dimension.X * gridConfig.cellSize, grid.Dimension.Y * gridConfig.cellSize));

            _gridState.OnCellAdded += HandleOnCellAdded;
            _gridState.OnCellRemoved += HandleOnCellRemoved;
            _gridState.OnGridUpdated += HandleOnGridUpdated;

            SpawnGridCells();
        }

        private void SpawnGridCells()
        {
            _cellViews = new List<CellView>();
            for (int x = 0; x < _gridState.Cells.Length; x++)
            {
                for (int y = 0; y < _gridState.Cells[x].Length; y++)
                {
                    SpawnNewCellView(_gridState.Cells[x][y]).PlayMovement();
                }
            }
        }

        private CellView SpawnNewCellView(Cell cell)
        {
            CellView cellView = Pool.GetPool(_gridViewConfigurationData.cellPoolData).Spawn(this.transform).GetComponent<CellView>();
            cellView.Init(cell);
            cellView.gameObject.name = "Cell " + cellView.Cell.Index.ToString();
            cellView.transform.localPosition = GetLocalPositionFromIndex(new PointInt2D() { X = cell.Index.X, Y = _gridState.Cells[cell.Index.X].Length + 1 });
            cellView.OnCellIndexUpdated += HandleOnCellIndexUpdated;
            _cellViews.Add(cellView);
            HandleOnCellIndexUpdated(cellView);
            return cellView;
        }

        private void HandleOnCellAdded(Cell cell)
        {
            SpawnNewCellView(cell);
        }

        private void HandleOnCellRemoved(Cell cell)
        {
            for (int i = _cellViews.Count - 1; i >= 0; i--)
            {
                if (_cellViews[i].Cell.Index == cell.Index)
                {
                    removalQueue.Enqueue(_cellViews[i]);
                    _cellViews.RemoveAt(i);
                    break;
                }
            }
        }

        private void HandleOnGridUpdated()
        {
            _cellsToRemove = removalQueue.Count;
            while (removalQueue.Count > 0)
            {
                removalQueue.Dequeue().DestroyCell(accumDelay, CheckIfRemovalIsDone);
                accumDelay += _gridViewConfigurationData.cellDestructionDelay * 0.5f;
            }
            removalQueue.Clear();
            accumDelay = 0;
        }

        private void HandleOnCellIndexUpdated(CellView cellView)
        {
            cellView.CueMovement(GetLocalPositionFromIndex(cellView.Cell.Index));
        }

        private void CheckIfRemovalIsDone()
        {
            _cellsToRemove--;
            if (_cellsToRemove <= 0)
            {
                _cellsToRemove = 0;
                MoveCells();
            }
        }

        private void MoveCells()
        {
            _movingCellsCount = 0;
            for (int i = 0; i < _cellViews.Count; i++)
            {
                if (_cellViews[i].IsCuedForMovement)
                {
                    _movingCellsCount++;
                    _cellViews[i].PlayMovement(CheckForMovementDone);
                }
            }
        }

        private void CheckForMovementDone()
        {
            _movingCellsCount--;
            if (_movingCellsCount <= 0)
                OnGridViewUpdated?.Invoke();
        }


        private Vector3 GetLocalPositionFromIndex(PointInt2D index)
        {
            return new Vector3(index.X * CellSize + _cellHalfSize + GridBounds.min.x, index.Y * CellSize + _cellHalfSize + GridBounds.min.y, GridBounds.center.z);
        }
    }
}