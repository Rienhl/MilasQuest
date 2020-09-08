using DG.Tweening;
using System;
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
        
        private CellView[] _cellViews;
        private GridViewConfig _gridConfig;

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
        }


        private void PlaceCells()
        {
            _cellViews = new CellView[Grid.Dimension.X * Grid.Dimension.Y]; //this currently only supports rectangle grids, for jagged grids we need another system
            for (int x = 0; x < Grid.Dimension.X; x++)
            {
                for (int y = 0; y < Grid.Dimension.Y; y++)
                {
                    SpawnNewCell(Grid.Cells[x][y]);
                }
            }
        }
        private void HandleOnNewCellAdded(Cell cell)
        {
            SpawnNewCell(cell);
        }

        private void SpawnNewCell(Cell cell)
        {
            CellView cellView = new GameObject().AddComponent<CellView>();
            cellView.gameObject.AddComponent<SpriteRenderer>();
            cellView.Init(cell, cellProps);
            cellView.gameObject.name = "Cell " + cellView.Cell.Index.ToString();
            cellView.transform.localPosition = GetLocalPositionFromIndex(new PointInt2D() { X = cell.Index.X, Y = Grid.Dimension.Y + 1 });
            cellView.OnCellIndexUpdated += HandleOnCellIndexUpdated;
            _cellViews[cell.Index.X + cell.Index.Y * Grid.Dimension.X] = cellView;
            HandleOnCellIndexUpdated(cellView);
        }

        private void HandleOnCellIndexUpdated(CellView cellView)
        {
            cellView.transform.DOMove(GetLocalPositionFromIndex(cellView.Cell.Index), 0.5f).SetEase(Ease.OutBounce);
        }

        private Vector3 GetLocalPositionFromIndex(PointInt2D index)
        {
            return new Vector3(index.X * CellSize + CellHalfSize + GridBounds.min.x, index.Y * CellSize + CellHalfSize + GridBounds.min.y, GridBounds.center.z);
        }
    }
}