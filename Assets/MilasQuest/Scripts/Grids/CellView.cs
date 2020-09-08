using System;
using UnityEngine;
using DG.Tweening;

namespace MilasQuest.Grids
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer cellSprite;

        public Cell Cell { get; private set; }

        private CellViewProperties cellViewProperties;
        private Color originColor;
        private Vector3 _targetPos;
        private bool _eventsRegistered;
        public bool IsCuedForMovement { get; private set; }

        public Action<CellView> OnCellIndexUpdated;

        public void Init(Cell cell, CellViewProperties properties)
        {
            this.Cell = cell;
            this.cellViewProperties = properties;

            if (cellSprite == null)
                cellSprite = GetComponent<SpriteRenderer>();
            cellSprite.sprite = properties.sprite;
            cellSprite.color = cellViewProperties.rndmColors[cell.CellType];
            originColor = cellSprite.color;
            _eventsRegistered = false;

            RegisterViewListeners();
        }

        public void RegisterViewListeners()
        {
            if (_eventsRegistered)
                return;
            _eventsRegistered = true;
            Cell.OnIndexUpdated += HandleOnIndexUpdated;
            Cell.OnSelected += HandleOnCellSelected;
            Cell.OnUnselected += HandleOnCellUnselected;
        }

        public void UnregisterViewListeners()
        {
            if (!_eventsRegistered)
                return;
            _eventsRegistered = false;
            Cell.OnIndexUpdated -= HandleOnIndexUpdated;
            Cell.OnSelected -= HandleOnCellSelected;
            Cell.OnUnselected -= HandleOnCellUnselected;
        }

        private void HandleOnCellSelected()
        {
            Debug.Log("selected");
            cellSprite.DOFade(1, 0.3f);
            transform.DOShakeScale(0.5f, 0.7f);
        }

        private void HandleOnCellUnselected()
        {
            cellSprite.DOFade(originColor.a, 0.3f);
        }

        public void DestroyCell()
        {
            this.transform.DOKill();
            Cell.OnIndexUpdated -= HandleOnIndexUpdated;
            Cell.OnSelected -= HandleOnCellSelected;
            Cell.OnUnselected -= HandleOnCellUnselected;
            this.transform.DOScale(0, 0.3f).SetEase(Ease.OutCirc);
            //repool
        }

        private void HandleOnIndexUpdated()
        {
            OnCellIndexUpdated?.Invoke(this);
        }

        public void CueMovement(Vector3 newPos)
        {
            _targetPos = newPos;
            IsCuedForMovement = true;
        }

        public void PlayMovement(Action onCompleteCallback = null)
        {
            if (IsCuedForMovement)
            {
                this.gameObject.transform.DOMove(_targetPos, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => { IsCuedForMovement = false; onCompleteCallback?.Invoke(); });

            }
        }
    }
}