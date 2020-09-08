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
        [SerializeField]
        private GridView gridView;

        private InputHandler _inputHandler;
        private GridInputConversor _gridInputConversor;
        private CellChainer _cellChainer;
        private GridState _grid;

        private const int CHAIN_MIN_CELL_COUNT = 3;

        private void Start()
        {
            _grid = new GridState(new GridConfig() { dimension = new PointInt2D() { X = 3, Y = 3 } });
            gridView.Init(_grid, new GridViewConfig() { cellSize = 1f, validInputRatio = 0.5f });
            _inputHandler = SolveInputHandler();
            _gridInputConversor = new GridInputConversor(_inputHandler, gridView, Camera.main);
            _gridInputConversor.Enable(true);
            _cellChainer = new CellChainer(_grid.Dimension, new CELL_CHAIN_CONDITION[3] { CELL_CHAIN_CONDITION.IS_SAME_TYPE, CELL_CHAIN_CONDITION.IS_UNSELECTED, CELL_CHAIN_CONDITION.IS_NEIGHBOUR_TO_LAST});
            RegisterGridInputActions();
        }

        private void RegisterGridInputActions()
        {
            _gridInputConversor.OnGridInputStarted += HandleOnGridInputStarted;
            _gridInputConversor.OnGridInputUpdated += HandleOnGridInputUpdated;
            _gridInputConversor.OnGridInputEnded += HandleOnGridInputEnded;
            _gridInputConversor.OnGridInputCancelled += HandleOnGridInputCancelled;
        }

        private void UnregisterGridInputActions()
        {
            _gridInputConversor.OnGridInputStarted -= HandleOnGridInputStarted;
            _gridInputConversor.OnGridInputUpdated -= HandleOnGridInputUpdated;
            _gridInputConversor.OnGridInputEnded -= HandleOnGridInputEnded;
            _gridInputConversor.OnGridInputCancelled -= HandleOnGridInputCancelled;
        }

        private void HandleOnGridInputStarted(PointInt2D newPoint)
        {
            if (GridUtils.IsPointOutOfGridBounds(newPoint, _grid.Dimension.X, _grid.Dimension.Y))
                return;
            _cellChainer.ChainNewCell(gridView.Grid.Cells[newPoint.X][newPoint.Y]);
        }

        private void HandleOnGridInputUpdated(PointInt2D newPoint)
        {
            if (GridUtils.IsPointOutOfGridBounds(newPoint, _grid.Dimension.X, _grid.Dimension.Y))
                return;
            _cellChainer.ChainNewCell(gridView.Grid.Cells[newPoint.X][newPoint.Y]);
        }

        private void HandleOnGridInputEnded(PointInt2D newPoint)
        {
            if (_cellChainer.ChainedCells.Count >= CHAIN_MIN_CELL_COUNT) //this should be managed by a chainender condition
            {
                UnregisterGridInputActions();
                gridView.OnAllCellsUpdated += ResumeInput;
                _grid.RemoveCells(_cellChainer.ChainedCells);
            }
            _cellChainer.ChainEnded();
        }

        private void ResumeInput()
        {
            gridView.OnAllCellsUpdated -= ResumeInput;
            gridView.SetViewResponsiveness(false);
            _grid.IsDeadlocked(_cellChainer);
            gridView.SetViewResponsiveness(true);
            RegisterGridInputActions();
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

    public interface ICellChainCondition
    {
        CELL_EVALUATION_OUTPUT CheckCondition(List<Cell> chainedCells, Cell newCell);

        CELL_CHAIN_CONDITION GetConditionType();
    }

    public class IsCellSameType : ICellChainCondition
    {
        public IsCellSameType() { }

        public CELL_EVALUATION_OUTPUT CheckCondition(List<Cell> chainedCells, Cell newCell)
        {
            if (chainedCells.Count == 0)
                return CELL_EVALUATION_OUTPUT.ADD;

            if (chainedCells[chainedCells.Count - 1].CellType == newCell.CellType)
                return CELL_EVALUATION_OUTPUT.ADD;
            else
                return CELL_EVALUATION_OUTPUT.DONT_ADD;
        }

        public CELL_CHAIN_CONDITION GetConditionType()
        {
            return CELL_CHAIN_CONDITION.IS_SAME_TYPE;
        }
    }

    public class IsCellUnselected : ICellChainCondition
    {
        public IsCellUnselected() { }

        public CELL_EVALUATION_OUTPUT CheckCondition(List<Cell> chainedCells, Cell newCell)
        {
            if (!newCell.IsSelected)
                return CELL_EVALUATION_OUTPUT.ADD;
            else
            {
                if (chainedCells.Count == 1)
                    return CELL_EVALUATION_OUTPUT.DONT_ADD;

                if (chainedCells[chainedCells.Count - 2] == newCell)
                    return CELL_EVALUATION_OUTPUT.REMOVE_PREVIOUS;
                else
                    return CELL_EVALUATION_OUTPUT.DONT_ADD;
            }
        }

        public CELL_CHAIN_CONDITION GetConditionType()
        {
            return CELL_CHAIN_CONDITION.IS_UNSELECTED;
        }
    }

    public class IsCellNeighbourToLast : ICellChainCondition
    {
        public IsCellNeighbourToLast() { }

        public CELL_EVALUATION_OUTPUT CheckCondition(List<Cell> chainedCells, Cell newCell)
        {
            if (chainedCells.Count == 0)
                return CELL_EVALUATION_OUTPUT.ADD;

            if (IsNeighbour(chainedCells[chainedCells.Count - 1].Index, newCell.Index))
                return CELL_EVALUATION_OUTPUT.ADD;
            else
                return CELL_EVALUATION_OUTPUT.DONT_ADD;
        }

        public CELL_CHAIN_CONDITION GetConditionType()
        {
            return CELL_CHAIN_CONDITION.IS_NEIGHBOUR_TO_LAST;
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

    public enum CELL_EVALUATION_OUTPUT
    {
        ADD,
        DONT_ADD,
        REMOVE_PREVIOUS
    }

    public enum CELL_CHAIN_CONDITION
    {
        IS_SAME_TYPE,
        IS_UNSELECTED,
        IS_NEIGHBOUR_TO_LAST
    }

    public static class ChainConditionFactory
    {
        public static ICellChainCondition GetCondition(CELL_CHAIN_CONDITION condition)
        {
            ICellChainCondition newCondition;
            switch (condition)
            {
                case CELL_CHAIN_CONDITION.IS_SAME_TYPE:
                    newCondition =  new IsCellSameType();
                    break;
                case CELL_CHAIN_CONDITION.IS_UNSELECTED:
                    newCondition = new IsCellUnselected();
                    break;
                case CELL_CHAIN_CONDITION.IS_NEIGHBOUR_TO_LAST:
                    newCondition = new IsCellNeighbourToLast();
                    break;
                default:
                    newCondition = null;
                    break;
            }
            return newCondition;
        }
    }

    public class CellChainer
    {
        public List<Cell> ChainedCells { get; private set; }
        private List<ICellChainCondition> _chainConditions;


        public CellChainer(PointInt2D gridDimensions, CELL_CHAIN_CONDITION[] conditions)
        {
            ChainedCells = new List<Cell>();
            _chainConditions = new List<ICellChainCondition>();

            for (int i = 0; i < conditions.Length; i++)
            {
                _chainConditions.Add(ChainConditionFactory.GetCondition(conditions[i]));
            }
        }

        public void AddCondition(CELL_CHAIN_CONDITION newCondition)
        {
            for (int i = 0; i < _chainConditions.Count; i++)
            {
                if (_chainConditions[i].GetConditionType() == newCondition)
                    return;
            }
            _chainConditions.Add(ChainConditionFactory.GetCondition(newCondition));
        }

        public void RemoveCondition(CELL_CHAIN_CONDITION newCondition)
        {
            for (int i = _chainConditions.Count - 1; i >= 0; i--)
            {
                if (_chainConditions[i].GetConditionType() == newCondition)
                {
                    _chainConditions.RemoveAt(i);
                }
            }
        }

        public bool ChainNewCell(Cell newCell)
        {
            CELL_EVALUATION_OUTPUT currentOutput = EvaluateCell(newCell);
            switch (currentOutput)
            {
                case CELL_EVALUATION_OUTPUT.ADD:
                    newCell.SetAsSelected();
                    ChainedCells.Add(newCell);
                    return true;
                case CELL_EVALUATION_OUTPUT.DONT_ADD:
                    break;
                case CELL_EVALUATION_OUTPUT.REMOVE_PREVIOUS:
                    ChainedCells[ChainedCells.Count - 1].SetAsSelected(false);
                    ChainedCells.RemoveAt(ChainedCells.Count - 1);
                    break;
                default:
                    break;
            }
            return false;
        }

        public void ChainEnded()
        {
            Debug.Log("Chain count " + ChainedCells.Count);
            for (int i = 0; i < ChainedCells.Count; i++)
            {
                ChainedCells[i].SetAsSelected(false);
            }
            ChainedCells.Clear();
        }

        private CELL_EVALUATION_OUTPUT EvaluateCell(Cell newCell)
        {
            CELL_EVALUATION_OUTPUT currentOutput = CELL_EVALUATION_OUTPUT.ADD;
            for (int i = 0; i < _chainConditions.Count; i++)
            {
                currentOutput = _chainConditions[i].CheckCondition(ChainedCells, newCell);
                if (currentOutput != CELL_EVALUATION_OUTPUT.ADD)
                    break;
            }
            return currentOutput;
        }
    }

}