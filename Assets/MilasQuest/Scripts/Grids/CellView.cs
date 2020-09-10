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
        [SerializeField] private SpriteRenderer glow;

        public Cell Cell { get; private set; }
        public bool IsCuedForMovement { get; private set; }

        private Color originColor;
        private Vector3 _targetPos;
        private bool _eventsRegistered;
        private bool _isVisible;
        private Vector3 _originScale;

        public Action<CellView> OnCellIndexUpdated;

        public void Init(Cell cell)
        {
            this.Cell = cell;
            _originScale = transform.localScale;
            if (cellSprite == null)
                cellSprite = GetComponent<SpriteRenderer>();
            cellSprite.sprite = cell.CellType.sprite;
            originColor = cellSprite.color;
            cellSprite.color = new Color(originColor.r, originColor.g, originColor.b, 0);
            _eventsRegistered = false;
            _isVisible = false;
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
            transform.DOBlendableScaleBy(Vector3.one * 0.3f, 0.3f).SetEase(Ease.OutElastic);
            transform.DOBlendablePunchRotation(Vector3.forward * 15f, 1, 8);
            glow.gameObject.SetActive(true);
            glow.DOFade(1, 0.2f);
            glow.transform.DOLocalRotate(new Vector3(0, 0, UnityEngine.Random.Range(-60, -90)), 1).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear).SetRelative();
        }

        private void HandleOnCellUnselected()
        {
            glow.transform.DOKill();
            glow.DOFade(0, 0.2f).OnComplete(() => { glow.DOKill(); glow.gameObject.SetActive(false); });
            transform.DOKill();
            transform.localRotation = Quaternion.identity;
            transform.localScale = _originScale;
            cellSprite.DOFade(originColor.a, 0.3f);
        }

        public void DestroyCell(float delay = 0, Action OnDestroyed = null)
        {
            this.transform.DOKill();
            UnregisterViewListeners();
            this.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).SetDelay(delay).OnComplete(() => { HandleOnCellUnselected();  OnDestroyed?.Invoke(); GetComponent<PoolObject>().Despawn(); });
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
            if (!_isVisible)
            {
                _isVisible = true;
                cellSprite.DOFade(originColor.a, 0.2f).OnComplete(() => PlayMovement(onCompleteCallback));
                return;
            }
            if (IsCuedForMovement)
            {
                this.gameObject.transform.DOMove(_targetPos, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => { IsCuedForMovement = false; onCompleteCallback?.Invoke(); });
            }
        }
    }
}