using MilasQuest.Grids;
using MilasQuest.Grids.LinkableRules;
using MilasQuest.InputManagement;
using MilasQuest.Pools;
using System;
using UnityEngine;

namespace MilasQuest
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private GridView gridView;

        [SerializeField]
        private PoolData[] pools;

        private InputHandler _inputHandler;
        private GridInputConversor _gridInputConversor;
        private GridState _grid;


        private void Start()
        {
            for (int i = 0; i < pools.Length; i++)
            {
                Pool.CreatePool(pools[i]);
            }
            _grid = new GridState(new GridConfig() { dimension = new PointInt2D() { X = 3, Y = 3 } });
            _grid.OnStartedUpdatingGrid += HandleOnStartedUpdatingGrid;
            gridView.Init(_grid, new GridViewConfig() { cellSize = 1f, validInputRatio = 0.5f });
            _inputHandler = SolveInputHandler();
            _gridInputConversor = new GridInputConversor(_inputHandler, gridView, Camera.main);
            _gridInputConversor.Enable(true);
            RegisterGridInputActions();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _grid.CheckForDeadlock();
            }
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
            _grid.AddCellAtPoint(newPoint);
        }

        private void HandleOnGridInputUpdated(PointInt2D newPoint)
        {
            _grid.AddCellAtPoint(newPoint);
        }

        private void HandleOnGridInputEnded(PointInt2D newPoint)
        {
            _grid.ProcessCurrentLink();
        }

        private void HandleOnStartedUpdatingGrid()
        {
            UnregisterGridInputActions();
            gridView.OnGridViewUpdated += HandleGridViewUpdated;
        }

        private void HandleGridViewUpdated()
        {
            gridView.OnGridViewUpdated -= HandleGridViewUpdated;
            //gridView.SetViewResponsiveness(false);
            _grid.CheckForDeadlock();
            //gridView.SetViewResponsiveness(true);
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