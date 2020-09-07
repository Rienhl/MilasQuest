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

        public Action<CellView> OnCellIndexUpdated;

        public void Init(Cell cell, CellViewProperties properties)
        {
            this.Cell = cell;
            this.cellViewProperties = properties;

            if (cellSprite == null)
                cellSprite = GetComponent<SpriteRenderer>();
            cellSprite.sprite = properties.sprite;
            cellSprite.color = cellViewProperties.rndmColors[UnityEngine.Random.Range(0, cellViewProperties.rndmColors.Length)];
            originColor = cellSprite.color;

            Cell.OnIndexUpdated += HandleOnIndexUpdated;
            Cell.OnSelected += HandleOnCellSelected;
            Cell.OnUnselected += HandleOnCellUnselected;
            Cell.OnRemoved += HandleOnRemoved;
        }

        private void HandleOnCellUnselected()
        {
            cellSprite.DOFade(originColor.a, 0.3f);
        }

        private void HandleOnCellSelected()
        {
            cellSprite.DOFade(1, 0.3f);
            transform.DOShakeScale(0.5f, 0.7f);
        }

        public void DestroyCell()
        {
            Cell.OnIndexUpdated -= HandleOnIndexUpdated;
            Cell.OnSelected -= HandleOnCellSelected;
            Cell.OnUnselected -= HandleOnCellUnselected;
            Cell.OnRemoved -= HandleOnRemoved;
        }

        private void HandleOnRemoved()
        {
            DestroyCell();
            Destroy(this.gameObject);
        }

        private void HandleOnIndexUpdated()
        {
            OnCellIndexUpdated?.Invoke(this);
        }
    }
}