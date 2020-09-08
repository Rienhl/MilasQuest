using MilasQuest.InputManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
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

        private GridConfig _config;

        public Action<Cell> OnNewCellSpawned;
        public Action<Cell> OnCellRemoved;
        public Action OnGridFinishedUpdating;

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
            OnGridFinishedUpdating?.Invoke();
        }

        private void RemoveCell(Cell cell)
        {
            OnCellRemoved?.Invoke(cell);
            for (int y = cell.Index.Y + 1; y < Dimension.Y; y++)
            {
                Cells[cell.Index.X][y - 1] = Cells[cell.Index.X][y];
                Cells[cell.Index.X][y - 1].UpdateIndex(cell.Index.X, y - 1);
            }
            Cells[cell.Index.X][Dimension.Y-1] = new Cell(cell.Index.X, Dimension.Y - 1);
            OnNewCellSpawned?.Invoke(Cells[cell.Index.X][Dimension.Y-1]);
        }


        private bool IsDeadlocked()
        {
            List<Cell> openList = new List<Cell>();
            List<Cell> closedList = new List<Cell>();
            PointInt2D[] auxSurroundingPoints = new PointInt2D[8];

            for (int x = 0; x < Dimension.X; x++)
            {
                for (int y = 0; y < Dimension.Y; y++)
                {
                    Cell cellToCheck = Cells[x][y];
                    if (closedList.Contains(cellToCheck)) //have I checked this cell before?
                        continue;
                    if (openList.Contains(cellToCheck)) //is cell already part of my current solution?
                        continue;

                    openList.Add(cellToCheck);
                    auxSurroundingPoints = GridUtils.GetSurroundingPoints(cellToCheck.Index);

                    for (int i = 0; i < auxSurroundingPoints.Length; i++)
                    {
                        if (GridUtils.IsPointOutOfGridBounds(auxSurroundingPoints[i], Dimension.X, Dimension.Y))
                            continue;
                        if (closedList.Contains(Cells[auxSurroundingPoints[i].X][auxSurroundingPoints[i].Y]))
                            continue;
                        if (openList.Contains(Cells[auxSurroundingPoints[i].X][auxSurroundingPoints[i].Y]))
                            continue;
                        if (Cells[auxSurroundingPoints[i].X][auxSurroundingPoints[i].Y].CellType != Cells[x][y].CellType)
                            continue;
                        openList.Add(Cells[auxSurroundingPoints[i].X][auxSurroundingPoints[i].Y]);
                        if (openList.Count >= 3)
                            return true;
                    }

                    if (openList.Count > 1)
                    {

                    }
                    else
                    {
                        closedList.Add(cellToCheck);
                        openList.Clear();
                    }
                }
            }
            return false;
        }
    }
}