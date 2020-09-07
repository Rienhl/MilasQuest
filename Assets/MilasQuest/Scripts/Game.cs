using MilasQuest.Grids;
using MilasQuest.InputManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace MilasQuest
{
    public class Game : MonoBehaviour
    {
        public GridView gridView;

        private InputHandler _inputHandler;
        private GridInputConversor _gridInputConversor;
        private CellChainer _cellChainer;

        private void Start()
        {
            gridView.Init(new GridState(new PointInt2D() { X = 10, Y = 15 }), new GridViewConfig() { cellSize = 1f });
            _gridInputConversor = new GridInputConversor();
            _inputHandler = SolveInputHandler();
            _gridInputConversor.Setup(_inputHandler, gridView, Camera.main);
            _gridInputConversor.Enable(true);
            _cellChainer = new CellChainer(gridView.Grid.CellCount);

            _gridInputConversor.OnGridInputStarted += HandleOnGridInputStarted;
            _gridInputConversor.OnGridInputUpdated += HandleOnGridInputUpdated;
            _gridInputConversor.OnGridInputEnded += HandleOnGridInputEnded;
            _gridInputConversor.OnGridInputCancelled += HandleOnGridInputCancelled;
        }

        private void HandleOnGridInputStarted(PointInt2D newPoint)
        {
            if (GridUtils.IsPointOutOfGridBounds(newPoint, gridView.Grid.CellCount.X, gridView.Grid.CellCount.Y))
                return;
            _cellChainer.ChainNewCell(gridView.Grid.Cells[newPoint.X][newPoint.Y]);
        }

        private void HandleOnGridInputUpdated(PointInt2D newPoint)
        {
            if (GridUtils.IsPointOutOfGridBounds(newPoint, gridView.Grid.CellCount.X, gridView.Grid.CellCount.Y))
                return;
            _cellChainer.ChainNewCell(gridView.Grid.Cells[newPoint.X][newPoint.Y]);
        }

        private void HandleOnGridInputEnded(PointInt2D newPoint)
        {
            _cellChainer.ChainEnded();
        }

        private void HandleOnGridInputCancelled()
        {
            throw new NotImplementedException();
        }

        public InputHandler SolveInputHandler()
        {
#if UNITY_EDITOR
            return gameObject.AddComponent<MouseInputHandler>();
#else
            return gameObject.AddComponent<TouchInputHandler>()
#endif
        }
    }

    public enum GRID_INPUT_RULES
    {
        DIAGONALS_ALLOWED,
        OUT_OF_BOUNDS_ALLOWED,
        COUNT
    }

    public enum GRID_CELL_RULES
    {
        CELLS_FALL,
        ALWAYS_SOLVABLE,
        AUTO_SHUFFLE,
        COUNT
    }

    public interface ICellCondition
    {
        CELL_EVALUATION_OUTPUT CheckCondition(Cell newCell);
    }

    public abstract class CellCondition : ICellCondition
    {
        protected CellChainer _cellChainer;
        public CellCondition(CellChainer cellChainer)
        {
            _cellChainer = cellChainer;
        }

        public abstract CELL_EVALUATION_OUTPUT CheckCondition(Cell newCell);
    }

    public class IsCellFree : CellCondition
    {
        public IsCellFree(CellChainer cellChainer) : base(cellChainer) { }

        public override CELL_EVALUATION_OUTPUT CheckCondition(Cell newCell)
        {
            if (!newCell.IsSelected)
                return CELL_EVALUATION_OUTPUT.ADD;
            else
            {
                if (_cellChainer.ChainedCells.Count == 1)
                    return CELL_EVALUATION_OUTPUT.DONT_ADD;

                if (_cellChainer.ChainedCells[_cellChainer.ChainedCells.Count - 2] == newCell)
                    return CELL_EVALUATION_OUTPUT.REMOVE_PREVIOUS;
                else
                    return CELL_EVALUATION_OUTPUT.DONT_ADD;
            }
        }
    }

    public class IsCellNeighbourToLastCondition : CellCondition
    {
        public IsCellNeighbourToLastCondition(CellChainer cellChainer) : base(cellChainer) { }

        public override CELL_EVALUATION_OUTPUT CheckCondition(Cell newCell)
        {
            if (_cellChainer.ChainedCells.Count == 0)
                return CELL_EVALUATION_OUTPUT.ADD;

            if (IsNeighbour(_cellChainer.ChainedCells[_cellChainer.ChainedCells.Count - 1].Index, newCell.Index))
                return CELL_EVALUATION_OUTPUT.ADD;
            else
                return CELL_EVALUATION_OUTPUT.DONT_ADD;
        }

        private bool IsNeighbour(PointInt2D previousPoint, PointInt2D newPoint)
        {
            PointInt2D[] neighbours = GridUtils.GetSurroundingPoints(previousPoint);
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] == newPoint)
                    return true;
            }
            return false;
        }
    }

    public class CellConditionEvaluator
    {
        private List<ICellCondition> _conditions;

        private CELL_EVALUATION_OUTPUT currentOutput;

        public CellConditionEvaluator()
        {
            _conditions = new List<ICellCondition>();
        }

        public void AddNewCondition(ICellCondition cellcondition)
        {
            _conditions.Add(cellcondition);
        }

        public CELL_EVALUATION_OUTPUT EvaluateCell(Cell newCell)
        {
            for (int i = 0; i < _conditions.Count; i++)
            {
                currentOutput = _conditions[i].CheckCondition(newCell);
                if (currentOutput != CELL_EVALUATION_OUTPUT.ADD)
                    break;
            }
            return currentOutput;
        }
    }

    public enum CELL_EVALUATION_OUTPUT
    {
        ADD,
        DONT_ADD,
        REMOVE_PREVIOUS
    }

    public class CellChainer
    {
        public List<Cell> ChainedCells;
        private CellConditionEvaluator _cellEvaluator;

        public CellChainer(PointInt2D gridDimensions)
        {
            ChainedCells = new List<Cell>();
            _cellEvaluator = new CellConditionEvaluator();
            _cellEvaluator.AddNewCondition(new IsCellFree(this));
            _cellEvaluator.AddNewCondition(new IsCellNeighbourToLastCondition(this));
        }

        public void ChainNewCell(Cell newCell)
        {
            switch (_cellEvaluator.EvaluateCell(newCell))
            {
                case CELL_EVALUATION_OUTPUT.ADD:
                    newCell.SetAsSelected();
                    ChainedCells.Add(newCell);
                    break;
                case CELL_EVALUATION_OUTPUT.DONT_ADD:
                    break;
                case CELL_EVALUATION_OUTPUT.REMOVE_PREVIOUS:
                    ChainedCells[ChainedCells.Count - 1].SetAsSelected(false);
                    ChainedCells.RemoveAt(ChainedCells.Count - 1);
                    break;
                default:
                    break;
            }
            Debug.Log($"Added Cell {newCell.Index}");
        }

        public void ChainEnded()
        {
            Debug.Log("Chain count " + ChainedCells.Count);
            ChainedCells.Clear();
        }
    }

}