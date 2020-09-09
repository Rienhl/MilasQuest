using MilasQuest.GameData;
using MilasQuest.Grids;
using MilasQuest.InputManagement;
using MilasQuest.Pools;
using MilasQuest.Stats;
using System;
using UnityEngine;

namespace MilasQuest
{
    public class ScoreTracker
    {

    }

    public class Game : MonoBehaviour
    {
        [SerializeField]
        private GridView gridView;

        [SerializeField]
        private PoolData[] _pools;
        [SerializeField]
        private LevelData _levelData;

        private InputHandler _inputHandler;
        private GridInputConversor _gridInputConversor;
        private GridState _grid;
        private LevelStats _levelStats;

        private void Start()
        {
            for (int i = 0; i < _pools.Length; i++)
            {
                Pool.CreatePool(_pools[i]);
            }
            _grid = new GridState(_levelData.gridConfigurationData);
            _grid.OnStartedUpdatingGrid += HandleOnStartedUpdatingGrid;
            gridView.Init(_grid, _levelData.gridViewConfigurationData);
            _inputHandler = SolveInputHandler();
            _gridInputConversor = new GridInputConversor(_inputHandler, gridView, Camera.main);
            _gridInputConversor.Enable(true);
            RegisterGridInputActions();
            _levelStats = new LevelStats();
            _levelStats.OnStatUpdatedListeners[STAT_TYPE.TOTAL_SCORE].Add((Stat s) => Debug.Log(s.GetCurrentValue()));
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
            _levelStats.ProcessStatModifier(new StatModifier() { operation = ARITHMETIC_OPERATOR.ADD, targetStat = STAT_TYPE.TOTAL_SCORE, value = 5 });
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