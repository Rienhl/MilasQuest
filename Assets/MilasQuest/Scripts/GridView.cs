using System;
using UnityEngine;

namespace MilasQuest.Grid
{
    public class GridView : MonoBehaviour
    {
        public CellViewProperties cellProps;

        private Grid grid;
        private GridViewProperties properties;
        private Bounds gridBounds;

        public CellView[] CellViews { get; private set; }

        public void Init(Grid grid, GridViewProperties properties)
        {
            this.grid = grid;
            this.properties = properties;

            gridBounds = new Bounds(Vector3.zero, new Vector3(grid.Dimensions.X * properties.cellSize, grid.Dimensions.Y * properties.cellSize));
        }

        private void PlaceCells()
        {
            CellViews = new CellView[grid.Dimensions.X * grid.Dimensions.Y]; //this currently only supports rectangle grids, for jagged grids we need another system
            for (int x = 0; x < grid.Dimensions.X; x++)
            {
                for (int y = 0; y < grid.Dimensions.Y; y++)
                {
                    CellView cellView = new GameObject().AddComponent<CellView>();
                    cellView.gameObject.AddComponent<SpriteRenderer>();
                    cellView.Init(grid.Cells[x][y], cellProps);
                    cellView.gameObject.name = "Cell " + cellView.Cell.Index.ToString();
                    cellView.transform.localPosition = GetLocalPositionFromIndex(grid.Cells[x][y].Index);
                    cellView.OnCellIndexUpdated += HandleOnCellIndexUpdated;
                    CellViews[x + y * grid.Dimensions.X] = cellView;
                }
            }
        }

        private void HandleOnCellIndexUpdated(CellView cellView)
        {
            cellView.transform.localEulerAngles = GetLocalPositionFromIndex(cellView.Cell.Index);
        }

        private Vector3 GetLocalPositionFromIndex(PointInt2D index)
        {
            return new Vector3(index.X * properties.cellSize + properties.GetHalfCellSize() + gridBounds.min.x, index.Y * properties.cellSize + properties.GetHalfCellSize() + gridBounds.min.y, gridBounds.center.z);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                Init(new Grid(new PointInt2D() { X = 10, Y = 15 }), new GridViewProperties() { cellSize = 1f});
                PlaceCells();
            }
        }
    }
}