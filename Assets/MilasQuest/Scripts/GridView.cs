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
        
        private CellView[] _cellViews;
        private GridViewConfig _gridConfig;

        public void Init(GridState grid, GridViewConfig gridConfig)
        {
            _gridConfig = gridConfig;

            Grid = grid;
            CellSize = _gridConfig.cellSize;
            CellHalfSize = _gridConfig.cellSize * 0.5f;
            GridBounds = new Bounds(Vector3.zero, new Vector3(grid.Dimension.X * gridConfig.cellSize, grid.Dimension.Y * gridConfig.cellSize));
            PlaceCells();
        }

        private void PlaceCells()
        {
            _cellViews = new CellView[Grid.Dimension.X * Grid.Dimension.Y]; //this currently only supports rectangle grids, for jagged grids we need another system
            for (int x = 0; x < Grid.Dimension.X; x++)
            {
                for (int y = 0; y < Grid.Dimension.Y; y++)
                {
                    CellView cellView = new GameObject().AddComponent<CellView>();
                    cellView.gameObject.AddComponent<SpriteRenderer>();
                    cellView.Init(Grid.Cells[x][y], cellProps);
                    cellView.gameObject.name = "Cell " + cellView.Cell.Index.ToString();
                    cellView.transform.localPosition = GetLocalPositionFromIndex(Grid.Cells[x][y].Index);
                    cellView.OnCellIndexUpdated += HandleOnCellIndexUpdated;
                    _cellViews[x + y * Grid.Dimension.X] = cellView;
                }
            }
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