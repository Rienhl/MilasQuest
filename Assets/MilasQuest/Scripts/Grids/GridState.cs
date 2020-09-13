using MilasQuest.GameData;
using MilasQuest.Grids.GameData;
using MilasQuest.Grids.LinkableRules;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest.Grids
{
    /// <summary>
    /// Logical Layer of the Grid, contains its model and state independent from cell size, sprites, grid position in the world, etc...
    /// </summary>
    public class GridState //this class should be split into model/controller classes 
    {
        public PointInt2D Dimension { get; private set; }

        //Jagged Arrays performance in tight loops outweigh the readability provided by Multidimensional Arrays.
        //This is recommended both by Unity and Microsoft. (More info: https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity8.html)
        //This increased performance is trivial in match3 type games, and the whole grid system should be abstracted.
        //Anyways, going with jagged arrays was decided as a way of getting the developers familiarized with them. 
        public Cell[][] Cells { get; private set; }

        private GridSettings _gridConfigurationData;
        private CellLinker _cellLinker;
        private CellTypeData[] _cellsInLevel;

        private const int _CHAIN_MIN_CELL_COUNT = 3;

        public Action<Cell> OnCellAdded;
        public Action<Cell> OnCellRemoved;
        public Action<Cell> OnCellLinked;
        public Action<Cell> OnCellUnlinked;
        public Action OnLinkCleared;
        public Action<List<Cell>> OnStartedUpdatingGrid;
        public Action OnGridUpdated;

        public GridState(GridSettings gridconfigurationData)
        {
            _gridConfigurationData = gridconfigurationData;
            Dimension = _gridConfigurationData.gridDimension;
            GetCellsInLevelDatas();
            GenerateGrid();

            _cellLinker = new CellLinker(Dimension, _gridConfigurationData.linkingRules);
            _cellLinker.OnCellLinked += HandleOnCellLinked;
            _cellLinker.OnCellUnlinked += HandleOnCellUnlinked;
        }

        public bool AddCellAtPoint(PointInt2D point)
        {
            if (GridUtils.IsPointOutOfGridBounds(point, Dimension.X, Dimension.Y))
                return false;
            return _cellLinker.AddCell(Cells[point.X][point.Y]);
        }

        public List<Cell> GetCurrentLink()
        {
            return _cellLinker.LinkedCells;
        }

        public void ProcessCurrentLink()
        {
            if (_cellLinker.LinkedCells.Count >= _CHAIN_MIN_CELL_COUNT) //this should be managed by a chainender condition
            {
                RemoveCells(_cellLinker.LinkedCells);
                _cellLinker.ClearLink();
            }
            _cellLinker.ClearLink();
            OnLinkCleared?.Invoke();
        }

        public void CheckForDeadlock()
        {
            while(IsGridDeadlocked())
            {
                ShuffleBoard();
            }
            OnGridUpdated?.Invoke();
        }

        private void GenerateGrid()
        {
            Cells = new Cell[Dimension.X][];
            for (int x = 0; x < Dimension.X; x++)
            {
                Cells[x] = new Cell[Dimension.Y];
                for (int y = 0; y < Dimension.Y; y++)
                {
                    Cells[x][y] = new Cell(x, y, GetRandomCell());
                }
            }
        }

        private void GetCellsInLevelDatas()
        {
            _cellsInLevel = new CellTypeData[_gridConfigurationData.cellsInLevel.Length];
            for (int i = 0; i < _cellsInLevel.Length; i++)
            {
                _cellsInLevel[i] = CellDataProvider.Instance.GetCellTypeData(_gridConfigurationData.cellsInLevel[i]);
            }
        }

        private CellTypeData GetRandomCell()
        {
            return _cellsInLevel[UnityEngine.Random.Range(0, _cellsInLevel.Length)];
        }

        private void RemoveCells(List<Cell> chainedCells)
        {
            OnStartedUpdatingGrid?.Invoke(chainedCells);
            for (int i = 0; i < chainedCells.Count; i++)
            {
                RemoveCell(chainedCells[i]);
            }
            OnGridUpdated?.Invoke();
        }

        private void RemoveCell(Cell cell)
        {
            OnCellRemoved?.Invoke(cell);
            for (int y = cell.Index.Y + 1; y < Dimension.Y; y++)
            {
                Cells[cell.Index.X][y - 1] = Cells[cell.Index.X][y];
                Cells[cell.Index.X][y - 1].UpdateIndex(cell.Index.X, y - 1);
            }
            Cells[cell.Index.X][Dimension.Y - 1] = new Cell(cell.Index.X, Dimension.Y - 1, GetRandomCell());
            OnCellAdded?.Invoke(Cells[cell.Index.X][Dimension.Y - 1]);
        }
        
        private void HandleOnCellLinked(Cell cell)
        {
            OnCellLinked?.Invoke(cell);
        }

        private void HandleOnCellUnlinked(Cell cell)
        {
            OnCellUnlinked?.Invoke(cell);
        }

        #region Deadlock
        private bool IsGridDeadlocked()
        {
            List<Cell> openList = new List<Cell>();
            List<Cell> closedList = new List<Cell>();

            _cellLinker.OnCellLinked-= HandleOnCellLinked;
            _cellLinker.OnCellUnlinked -= HandleOnCellUnlinked;

            _cellLinker.RemoveRule(CELL_LINKING_RULE.IS_NEIGHBOUR_TO_LAST);

            for (int x = 0; x < Dimension.X; x++)
            {
                for (int y = 0; y < Dimension.Y; y++)
                {
                    Cell cellToCheck = Cells[x][y];
                    if (closedList.Contains(cellToCheck))
                        continue;
                    if (_cellLinker.LinkedCells.Contains(cellToCheck))
                        continue;
                    openList.Clear();
                    _cellLinker.AddCell(cellToCheck, false);
                    if (AreNeighboursDeadlocked(cellToCheck, openList))
                    {
                        closedList.AddRange(_cellLinker.LinkedCells);
                        _cellLinker.ClearLink();
                    }
                    else
                    {
                        _cellLinker.AddRule(CELL_LINKING_RULE.IS_NEIGHBOUR_TO_LAST);
                        _cellLinker.ClearLink();
                        _cellLinker.OnCellLinked += HandleOnCellLinked;
                        _cellLinker.OnCellUnlinked += HandleOnCellUnlinked;
                        return false;
                    }
                }
            }
            _cellLinker.AddRule(CELL_LINKING_RULE.IS_NEIGHBOUR_TO_LAST);
            _cellLinker.ClearLink();
            return true;
        }

        private bool AreNeighboursDeadlocked(Cell cell, List<Cell> openList)
        {
            PointInt2D[] neighbours = GridUtils.GetSurroundingPoints(cell.Index);
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (GridUtils.IsPointOutOfGridBounds(neighbours[i], Dimension.X, Dimension.Y))
                    continue;
                bool r = _cellLinker.AddCell(Cells[neighbours[i].X][neighbours[i].Y], false);
                if (r)
                {
                    if (_cellLinker.LinkedCells.Count >= 3)
                        return false; //NO DEADLOCK!
                    openList.Add(Cells[neighbours[i].X][neighbours[i].Y]);
                }
            }
            if (_cellLinker.LinkedCells.Count == 1)
                return true; //No good chain, but no deadlock yet
            else
            {
                for (int i = openList.Count - 1; i >= 0; i--)
                {
                    if (AreNeighboursDeadlocked(openList[i], openList))
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
        #endregion

        private void ShuffleBoard()
        {
            List<PointInt2D> allIndices = new List<PointInt2D>();
            for (int x = 0; x < Dimension.X; x++)
            {
                for (int y = 0; y < Dimension.Y; y++)
                {
                    allIndices.Add(new PointInt2D() { X = x, Y = y });
                }
            }
            allIndices.Shuffle();
            for (int i = allIndices.Count - 1; i >= 1; i-=2)
            {
                SwapCells(allIndices[i], allIndices[i - 1]);
            }
        }

        private void SwapCells(PointInt2D a, PointInt2D b)
        {
            Cells[a.X][a.Y].UpdateIndex(b.X, b.Y);
            Cells[b.X][b.Y].UpdateIndex(a.X, a.Y);

            Cell temp = Cells[a.X][a.Y];
            Cells[a.X][a.Y] = Cells[b.X][b.Y];
            Cells[b.X][b.Y] = temp;
        }
    }
}