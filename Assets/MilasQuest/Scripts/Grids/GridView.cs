using DG.Tweening;
using MilasQuest.Grids.GameData;
using MilasQuest.Pools;
using MilasQuest.UI;
using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

namespace MilasQuest.Grids
{
    /// <summary>
    /// this class contains the view part of the grid. It handles each cells visual representation, animations
    /// and is used as information source for input processing.
    /// Before creating other types of views, this should be converted into an abstract class
    /// </summary>
    public class GridView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        public Bounds GridBounds { get; private set; }
        public float CellSize { get; private set; }
        public float ActiveInputRadius { get; private set; }
        
        private float _cellHalfSize;
        private GridState _gridState;
        private List<CellView> _cellViews;
        private GridViewSettings _gridViewSettings;
        private Queue<CellView> _removalQueue;
        private GameObject[] _backgroundTiles;
        private Dictionary<int, GatheredCellsStatPanel> _removedCellsTargetPositions;

        private int _movingCellsCount = 0;
        private float accumDelay = 0;
        private int _cellsToRemove = 0;

        public Action OnGridViewUpdated;
        public Action<int> OnCellReachedPanel;

        public void Setup(GridState grid, GridViewSettings gridViewSettings, Dictionary<int, GatheredCellsStatPanel> gatheredCellsPanels)
        {
            if (_removalQueue == null)
                _removalQueue = new Queue<CellView>();
            else
                _removalQueue.Clear();

            _removedCellsTargetPositions = gatheredCellsPanels;

            _gridViewSettings = gridViewSettings;
            _gridState = grid;

            CellSize = _gridViewSettings.cellSize;
            _cellHalfSize = _gridViewSettings.cellSize * 0.5f;
            ActiveInputRadius = _gridViewSettings.validInputRatio;
            GridBounds = new Bounds(Vector3.zero, new Vector3(grid.Dimension.X * gridViewSettings.cellSize, grid.Dimension.Y * gridViewSettings.cellSize));

            _gridState.OnCellAdded += HandleOnCellAdded;
            _gridState.OnCellRemoved += HandleOnCellRemoved;
            _gridState.OnGridUpdated += HandleOnGridUpdated;

            _gridState.OnCellLinked += HandleOnCellLinked;
            _gridState.OnCellUnlinked += HandleOnCellUnlinked;
            _gridState.OnLinkCleared += HandleOnLinkCleared;


            SpawnGridCells();
            SpawnBackground();
        }

        public void Unsetup()
        {
            _gridState.OnCellAdded -= HandleOnCellAdded;
            _gridState.OnCellRemoved -= HandleOnCellRemoved;
            _gridState.OnGridUpdated -= HandleOnGridUpdated;

            _gridState.OnCellLinked -= HandleOnCellLinked;
            _gridState.OnCellUnlinked -= HandleOnCellUnlinked;
            _gridState.OnLinkCleared -= HandleOnLinkCleared;

            DespawnGridCells();
        }

        private void SpawnBackground()
        {
            _backgroundTiles = new GameObject[_cellViews.Count];
            Vector3 scale = Vector3.one * _gridViewSettings.cellSize;
            for (int i = 0; i < _backgroundTiles.Length; i++)
            {
                _backgroundTiles[i] = Pool.GetPool(_gridViewSettings.backgroundTilesPoolDatas[i % 2]).Spawn(this.transform);
                _backgroundTiles[i].transform.localScale = Vector3.zero;
                _backgroundTiles[i].transform.localPosition = GetLocalPositionFromIndex(_cellViews[i].Cell.Index);
                if (i == _backgroundTiles.Length - 1)
                    _backgroundTiles[i].transform.DOScale(scale, _gridViewSettings.backgroundConstructionDelay).SetDelay(_gridViewSettings.backgroundConstructionDelay * i * 0.1f).OnComplete(DropAllCellsIntoGrid);
                else
                    _backgroundTiles[i].transform.DOScale(scale, _gridViewSettings.backgroundConstructionDelay).SetDelay(_gridViewSettings.backgroundConstructionDelay * i * 0.1f);
            }
        }

        private void SpawnGridCells()
        {
            _cellViews = new List<CellView>();
            for (int x = 0; x < _gridState.Cells.Length; x++)
            {
                for (int y = 0; y < _gridState.Cells[x].Length; y++)
                {
                    SpawnNewCellView(_gridState.Cells[x][y]);
                }
            }
        }

        private void DespawnGridCells()
        {
            for (int i = _cellViews.Count - 1; i >= 0; i--)
            {
                _cellViews[i].Unsetup();
            }
            for (int i = _backgroundTiles.Length - 1; i >= 0; i--)
            {
                _backgroundTiles[i].GetComponent<PoolObject>().Despawn();
            }
        }

        private void DropAllCellsIntoGrid()
        {
            for (int i = 0; i < _cellViews.Count - 1; i++)
            {
                _cellViews[i].PlayMovement();
            }
            _cellViews[_cellViews.Count - 1].PlayMovement(() => OnGridViewUpdated?.Invoke());
        }

        private CellView SpawnNewCellView(Cell cell)
        {
            CellView cellView = Pool.GetPool(_gridViewSettings.cellPoolData).Spawn(this.transform).GetComponent<CellView>();
            cellView.transform.localPosition = GetLocalPositionFromIndex(new PointInt2D() { X = cell.Index.X, Y = _gridState.Cells[cell.Index.X].Length + 1 });
            cellView.transform.localScale = Vector3.one * _gridViewSettings.cellSize * _gridViewSettings.cellContentScale;
            cellView.Setup(cell);
            cellView.gameObject.name = "Cell " + cellView.Cell.Index.ToString();
            cellView.OnCellIndexUpdated += HandleOnCellIndexUpdated;
            _cellViews.Add(cellView);
            HandleOnCellIndexUpdated(cellView);
            return cellView;
        }

        private void HandleOnCellAdded(Cell cell)
        {
            SpawnNewCellView(cell);
        }

        private void HandleOnCellRemoved(Cell cell)
        {
            for (int i = _cellViews.Count - 1; i >= 0; i--)
            {
                if (_cellViews[i].Cell.Index == cell.Index)
                {
                    _removalQueue.Enqueue(_cellViews[i]);
                    _cellViews.RemoveAt(i);
                    break;
                }
            }
        }

        private void HandleOnGridUpdated()
        {
            RemoveLinkView();
            if (_removalQueue.Count == 0)
            {
                MoveCells();
                return;
            }
            _cellsToRemove = _removalQueue.Count;
            while (_removalQueue.Count > 0)
            {
                CellView cell = _removalQueue.Dequeue();
                Vector3 targetPosition = default;
                if (_removedCellsTargetPositions.TryGetValue(cell.Cell.CellType.id, out GatheredCellsStatPanel targetPanel))
                {
                    targetPosition = targetPanel.GetImagePosition();
                }
                cell.OnDestroyed += HandleOnCellViewDestroyed;
                cell.DestroyCell(targetPosition, accumDelay);
                accumDelay += _gridViewSettings.cellDestructionDelay * 0.5f;
            }
            _removalQueue.Clear();
            accumDelay = 0;
        }

        private void HandleOnCellViewDestroyed(CellView cellView)
        {
            if(_removedCellsTargetPositions.TryGetValue(cellView.Cell.CellType.id, out GatheredCellsStatPanel targetPanel))
            {
                targetPanel.DoAnim();
            }
            cellView.OnDestroyed -= HandleOnCellViewDestroyed;
            CheckIfRemovalIsDone();
        }

        private void HandleOnCellIndexUpdated(CellView cellView)
        {
            cellView.CueMovement(GetLocalPositionFromIndex(cellView.Cell.Index));
        }

        private void CheckIfRemovalIsDone()
        {
            _cellsToRemove--;
            if (_cellsToRemove <= 0)
            {
                _cellsToRemove = 0;
                MoveCells();
            }
        }

        private void MoveCells()
        {
            _movingCellsCount = 0;
            for (int i = 0; i < _cellViews.Count; i++)
            {
                if (_cellViews[i].IsCuedForMovement)
                {
                    _movingCellsCount++;
                    _cellViews[i].PlayMovement(CheckForMovementDone);
                }
            }
        }

        private void CheckForMovementDone()
        {
            _movingCellsCount--;
            if (_movingCellsCount <= 0)
                OnGridViewUpdated?.Invoke();
        }

        private void HandleOnCellLinked(Cell cell)
        {
            _lineRenderer.positionCount = _gridState.GetCurrentLink().Count;
            for (int i = 0; i < _gridState.GetCurrentLink().Count; i++)
            {
                _lineRenderer.SetPosition(i, GetLocalPositionFromIndex(_gridState.GetCurrentLink()[i].Index)); 
            }
        }

        private void HandleOnCellUnlinked(Cell cell)
        {
            UpdateLinkView();
        }

        private void HandleOnLinkCleared()
        {
            RemoveLinkView();
        }

        private void UpdateLinkView()
        {
            _lineRenderer.positionCount = _gridState.GetCurrentLink().Count;
            for (int i = 0; i < _gridState.GetCurrentLink().Count; i++)
            {
                _lineRenderer.SetPosition(i, GetLocalPositionFromIndex(_gridState.GetCurrentLink()[i].Index));
            }
        }

        private void RemoveLinkView()
        {
            _lineRenderer.positionCount = 0;
        }

        private Vector3 GetLocalPositionFromIndex(PointInt2D index)
        {
            return new Vector3(index.X * CellSize + _cellHalfSize + GridBounds.min.x, index.Y * CellSize + _cellHalfSize + GridBounds.min.y, GridBounds.center.z);
        }
    }
}