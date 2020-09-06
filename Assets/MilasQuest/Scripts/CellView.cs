using System;
using UnityEngine;

namespace MilasQuest.Grid
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer cellSprite;

        public Cell Cell { get; private set; }

        private CellViewProperties cellViewProperties;

        public Action<CellView> OnCellIndexUpdated;

        public void Init(Cell cell, CellViewProperties properties)
        {
            this.Cell = cell;
            this.cellViewProperties = properties;

            if (cellSprite == null)
                cellSprite = GetComponent<SpriteRenderer>();
            cellSprite.sprite = properties.sprite;
            cellSprite.color = UnityEngine.Random.ColorHSV();

            Cell.OnIndexUpdated += HandleOnIndexUpdated;
        }

        public void DestroyCell()
        {
            Cell.OnIndexUpdated -= HandleOnIndexUpdated;
        }

        private void HandleOnIndexUpdated()
        {
            OnCellIndexUpdated?.Invoke(this);
        }
    }
}