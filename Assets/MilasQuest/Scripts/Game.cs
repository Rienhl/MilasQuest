using MilasQuest.Grids;
using MilasQuest.Grids.LinkableRules;
using MilasQuest.InputManagement;
using System;
using UnityEngine;

namespace MilasQuest
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private GridView gridView;

        private InputHandler _inputHandler;
        private GridInputConversor _gridInputConversor;
        private CellLinker _cellLinker;
        private GridState _grid;

        private const int CHAIN_MIN_CELL_COUNT = 3;

        private void Start()
        {
            _grid = new GridState(new GridConfig() { dimension = new PointInt2D() { X = 3, Y = 3 } });
            gridView.Init(_grid, new GridViewConfig() { cellSize = 1f, validInputRatio = 0.5f });
            _inputHandler = SolveInputHandler();
            _gridInputConversor = new GridInputConversor(_inputHandler, gridView, Camera.main);
            _gridInputConversor.Enable(true);
            _cellLinker = new CellLinker(_grid.Dimension, new CELL_LINKING_RULE[3] { CELL_LINKING_RULE.IS_SAME_TYPE, CELL_LINKING_RULE.IS_UNSELECTED, CELL_LINKING_RULE.IS_NEIGHBOUR_TO_LAST});
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
            _cellLinker.AddCell(gridView.Grid.Cells[newPoint.X][newPoint.Y]);
        }

        private void HandleOnGridInputUpdated(PointInt2D newPoint)
        {
            if (GridUtils.IsPointOutOfGridBounds(newPoint, _grid.Dimension.X, _grid.Dimension.Y))
                return;
            _cellLinker.AddCell(gridView.Grid.Cells[newPoint.X][newPoint.Y]);
        }

        private void HandleOnGridInputEnded(PointInt2D newPoint)
        {
            if (_cellLinker.LinkedCells.Count >= CHAIN_MIN_CELL_COUNT) //this should be managed by a chainender condition
            {
                UnregisterGridInputActions();
                gridView.OnAllCellsUpdated += ResumeInput;
                _grid.RemoveCells(_cellLinker.LinkedCells);
            }
            _cellLinker.LinkFinalized();
        }

        private void ResumeInput()
        {
            gridView.OnAllCellsUpdated -= ResumeInput;
            gridView.SetViewResponsiveness(false);
            _grid.IsDeadlocked(_cellLinker);
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
}