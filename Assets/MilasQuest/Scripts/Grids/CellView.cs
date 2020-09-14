using System;
using UnityEngine;
using DG.Tweening;
using MilasQuest.Pools;
using MilasQuest.Grids.GameData;
using System.ComponentModel;

namespace MilasQuest.Grids
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer cellSprite;
        [SerializeField] private SpriteRenderer glow;

        public Cell Cell { get; private set; }
        public bool IsCuedForMovement { get; private set; }

        private Vector3 _targetPos;
        private bool _eventsRegistered;
        private bool _isVisible;
        private Color _originColor;
        private Color _originGlowColor;
        private Vector3 _originScale;
        private Vector3 _originGlowScale;
        private PoolObject _poolObject;

        public Action<CellView> OnCellIndexUpdated;
        public Action<CellView> OnDestroyed;

        public void Setup(Cell cell)
        {
            this.Cell = cell;
            _originScale = transform.localScale;
            _originGlowScale = glow.transform.localScale;
            if (cellSprite == null)
                cellSprite = GetComponent<SpriteRenderer>();
            cellSprite.sprite = cell.CellType.sprite;
            _originColor = cellSprite.color;
            _originGlowColor = glow.color;
            cellSprite.color = new Color(_originColor.r, _originColor.g, _originColor.b, 0);
            _eventsRegistered = false;
            _isVisible = false;
            _poolObject = GetComponent<PoolObject>();
            RegisterViewListeners();
        }

        public void Unsetup()
        {
            UnregisterViewListeners();
            glow.transform.DOKill();
            transform.DOKill();
            transform.localScale = _originScale;
            transform.localRotation = Quaternion.identity;
            cellSprite.color = _originColor;
            glow.transform.localScale = _originGlowScale;
            glow.color = _originGlowColor;
            _poolObject.Despawn();
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
            glow.transform.localScale = _originGlowScale;
            transform.DOKill();
            transform.localRotation = Quaternion.identity;
            transform.localScale = _originScale;
            cellSprite.DOFade(_originColor.a, 0.3f);
        }

        public void DestroyCell(Vector3 targetPosition, float delay = 0)
        {
            this.transform.DOKill();
            UnregisterViewListeners();
            if (targetPosition != default)
            {
                cellSprite.sortingOrder++;
                glow.transform.DOScale(2, 0.3f);
                glow.DOFade(0, 0.3f);
                transform.DOMove(new Vector3(targetPosition.x, targetPosition.y, transform.position.z), 0.7f).SetEase(Ease.InQuad).SetDelay(delay).OnComplete(() => { cellSprite.sortingOrder--; HandleOnCellUnselected(); OnDestroyed?.Invoke(this); _poolObject.Despawn(); });
            }
            else
                this.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).SetDelay(delay).OnComplete(() => { HandleOnCellUnselected(); OnDestroyed?.Invoke(this);_poolObject.Despawn(); });
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
                cellSprite.DOFade(_originColor.a, 0.2f).OnComplete(() => PlayMovement(onCompleteCallback));
                return;
            }
            if (IsCuedForMovement)
            {
                this.gameObject.transform.DOMove(_targetPos, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => { IsCuedForMovement = false; onCompleteCallback?.Invoke(); });
            }
        }
    }
}