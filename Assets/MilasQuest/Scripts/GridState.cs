using System;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;

namespace MilasQuest.Grids
{
    /// <summary>
    /// Contains all the necessary information for setting up a grid, independent from it's visuals
    /// </summary>
    [System.Serializable]
    public class GridConfig
    {
        public PointInt2D dimension;
    }


    /// <summary>
    /// Think similar to MVC, this class contains the model part of the grid, where it's current state is stored
    /// independent from cell size, sprites, grid position in the world, etc...
    /// </summary>
    public class GridState
    {
        public PointInt2D Dimension { get; private set; }

        //Jagged Arrays performance in tight loops outweigh the readability provided by Multidimensional Arrays.
        //This is recommended both by Unity and Microsoft. (More info: https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity8.html)
        public Cell[][] Cells { get; private set; }

        private PointInt2D aux;
        private GridConfig _config;

        public Action<Cell> OnNewCellSpawned;

        public GridState(GridConfig config)
        {
            _config = config;
            this.Dimension = config.dimension;
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            Cells = new Cell[Dimension.X][];
            for (int x = 0; x < Dimension.X; x++)
            {
                Cells[x] = new Cell[Dimension.Y];
                for (int y = 0; y < Dimension.Y; y++)
                {
                    Cells[x][y] = new Cell(x, y);
                }
            }
        }

        internal void RemoveCells(List<Cell> chainedCells)
        {
            for (int i = 0; i < chainedCells.Count; i++)
            {
                RemoveCell(chainedCells[i]);
            }
        }

        private void RemoveCell(Cell cell)
        {
            cell.Remove();
            for (int y = cell.Index.Y; y < Dimension.Y; y++)
            {
                if (y < Dimension.Y - 1)
                {
                    Cells[cell.Index.X][y] = Cells[cell.Index.X][y + 1];
                    Cells[cell.Index.X][y].UpdateIndex(cell.Index.X, y);
                }
                else
                {
                    Cells[cell.Index.X][y] = new Cell(cell.Index.X, y);
                    OnNewCellSpawned?.Invoke(Cells[cell.Index.X][y]);
                }
            }
        }
    }
}