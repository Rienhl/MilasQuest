using MilasQuest.InputManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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


        public bool IsDeadlocked(CellChainer chainer)
        {
            List<Cell> openList = new List<Cell>();
            List<Cell> closedList = new List<Cell>();

            chainer.RemoveCondition(CELL_CHAIN_CONDITION.IS_NEIGHBOUR_TO_LAST);

            for (int x = 0; x < Dimension.X; x++)
            {
                for (int y = 0; y < Dimension.Y; y++)
                {
                    Cell cellToCheck = Cells[x][y];
                    if (closedList.Contains(cellToCheck))
                        continue;
                    if (chainer.ChainedCells.Contains(cellToCheck))
                        continue;
                    openList.Clear();
                    chainer.ChainNewCell(cellToCheck);
                    if (AreNeighboursDeadlocked(cellToCheck, chainer, openList))
                    {
                        closedList.AddRange(chainer.ChainedCells);
                        chainer.ChainEnded();
                    }
                    else
                    {
                        chainer.AddCondition(CELL_CHAIN_CONDITION.IS_NEIGHBOUR_TO_LAST);
                        chainer.ChainEnded();
                        return false;
                    }
                }
            }
            chainer.AddCondition(CELL_CHAIN_CONDITION.IS_NEIGHBOUR_TO_LAST);
            chainer.ChainEnded();
            Debug.Log("DEADLOCK!");
            return true;
        }

        private bool AreNeighboursDeadlocked(Cell cell, CellChainer chainer, List<Cell> openList)
        {
            PointInt2D[] neighbours = GridUtils.GetSurroundingPoints(cell.Index);
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (GridUtils.IsPointOutOfGridBounds(neighbours[i], Dimension.X, Dimension.Y))
                    continue;
                bool r = chainer.ChainNewCell(Cells[neighbours[i].X][neighbours[i].Y]);
                if (r)
                {
                    if (chainer.ChainedCells.Count >= 3)
                        return false; //NO DEADLOCK!
                    openList.Add(Cells[neighbours[i].X][neighbours[i].Y]);
                }
            }
            if (chainer.ChainedCells.Count == 1)
                return true; //No good chain, but no deadlock yet
            else
            {
                for (int i = openList.Count - 1; i >= 0; i--)
                {
                    if (AreNeighboursDeadlocked(openList[i], chainer, openList))
                    {
                        openList.RemoveAt(i);
                    }
                    else
                    {
                        return false; //NO DEADLOCK!!
                    }
                }
                return true; //No good chain, but no deadlock yet

            }


        }
    }
}