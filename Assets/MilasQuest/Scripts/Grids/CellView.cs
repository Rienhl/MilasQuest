using System;
using UnityEngine;
using DG.Tweening;
using MilasQuest.Pools;
using MilasQuest.Grids.GameData;

namespace MilasQuest.Grids
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer cellSprite;

        public Cell Cell { get; private set; }

        private Color originColor;
        private Vector3 _targetPos;
        private bool _eventsRegistered;
        public bool IsCuedForMovement { get; private set; }

        public Action<CellView> OnCellIndexUpdated;

        public void Init(Cell cell)
        {
            this.Cell = cell;

            if (cellSprite == null)
                cellSprite = GetComponent<SpriteRenderer>();
            cellSprite.sprite = cell.CellType.sprite;
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
            transform.localScale = Vector3.one;
            cellSprite.DOFade(1, 0.3f);
            transform.DOShakeScale(0.5f, 0.5f);
        }

        private void HandleOnCellUnselected()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            cellSprite.DOFade(originColor.a, 0.3f);
        }

        public void DestroyCell(float delay = 0, Action OnDestroyed = null)
        {
            this.transform.DOKill();
            UnregisterViewListeners();
            this.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).SetDelay(delay).OnComplete(() => { OnDestroyed?.Invoke(); GetComponent<PoolObject>().Despawn(); });
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