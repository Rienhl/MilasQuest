using System;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest.Grids
{
    /// <summary>
    /// Think MVC, this class contains the view part of the grid. It handles each cells visual representation, animations
    /// and is used as information source for input processing
    /// </summary>
    public class GridView : MonoBehaviour
    {
        public CellViewProperties cellProps;

        public GridState Grid { get; private set; }

        public Bounds GridBounds { get; private set; }
        public float CellSize { get; private set; }
        public float CellHalfSize { get; private set; }
        public float ActiveInputRadius { get; private set; }
        
        private List<CellView> _cellViews;
        private GridViewConfig _gridConfig;

        private int _movingCellsCount = 0;
        public Action OnGridViewUpdated;

        public void Init(GridState grid, GridViewConfig gridConfig)
        {
            _gridConfig = gridConfig;

            Grid = grid;
            CellSize = _gridConfig.cellSize;
            CellHalfSize = _gridConfig.cellSize * 0.5f;
            ActiveInputRadius = CellHalfSize * _gridConfig.validInputRatio;
            GridBounds = new Bounds(Vector3.zero, new Vector3(grid.Dimension.X * gridConfig.cellSize, grid.Dimension.Y * gridConfig.cellSize));
            PlaceCells();
            Grid.OnNewCellSpawned += HandleOnNewCellAdded;
            Grid.OnCellRemoved += HandleOnCellRemoved;
            Grid.OnFinishedUpdatingGrid += HandleOnGridUpdated;
        }

        public void SetViewResponsiveness(bool enable)
        {
            if (enable)
            {
                for (int i = 0; i < _cellViews.Count; i++)
                {
                    _cellViews[i].RegisterViewListeners();
                }
            }
            else
            {
                for (int i = 0; i < _cellViews.Count; i++)
                {
                    _cellViews[i].UnregisterViewListeners();
                }
            }
        }

        private void PlaceCells()
        {
            _cellViews = new List<CellView>();
            for (int x = 0; x < Grid.Dimension.X; x++)
            {
                for (int y = 0; y < Grid.Dimension.Y; y++)
                {
                    SpawnNewCellView(Grid.Cells[x][y]).PlayMovement();
                }
            }
        }

        private CellView SpawnNewCellView(Cell cell)
        {
            CellView cellView = new GameObject().AddComponent<CellView>();
            cellView.gameObject.AddComponent<SpriteRenderer>();
            cellView.Init(cell, cellProps);
            cellView.gameObject.name = "Cell " + cellView.Cell.Index.ToString();
            cellView.transform.localPosition = GetLocalPositionFromIndex(new PointInt2D() { X = cell.Index.X, Y = Grid.Dimension.Y + 1 });
            cellView.OnCellIndexUpdated += HandleOnCellIndexUpdated;
            _cellViews.Add(cellView);
            HandleOnCellIndexUpdated(cellView);
            return cellView;
        }

        private void HandleOnNewCellAdded(Cell cell)
        {
            SpawnNewCellView(cell);
        }

        private void HandleOnCellRemoved(Cell cell)
        {
            for (int i = _cellViews.Count - 1; i >= 0; i--)
            {
                if (_cellViews[i].Cell.Index == cell.Index)
                {
                    _cellViews[i].DestroyCell();
                    _cellViews.RemoveAt(i);
                }
            }
        }

        private void HandleOnGridUpdated()
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

        private void HandleOnCellIndexUpdated(CellView cellView)
        {
            cellView.CueMovement(GetLocalPositionFromIndex(cellView.Cell.Index));
        }

        private Vector3 GetLocalPositionFromIndex(PointInt2D index)
        {
            return new Vector3(index.X * CellSize + CellHalfSize + GridBounds.min.x, index.Y * CellSize + CellHalfSize + GridBounds.min.y, GridBounds.center.z);
        }
    }
}