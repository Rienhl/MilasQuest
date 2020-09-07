using Cinemachine;
using System;
using UnityEngine;

namespace MilasQuest.Grids
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
            Cell.OnSelected += HandleOnCellSelected;
            Cell.OnUnselected += HandleOnCellUnselected;
        }

        private void HandleOnCellUnselected()
        {
            cellSprite.color = Color.red;
            transform.localScale = Vector3.one;
        }

        private void HandleOnCellSelected()
        {
            cellSprite.color = Color.green;
            transform.localScale *= 1.1f;
        }

        public void DestroyCell()
        {
            Cell.OnIndexUpdated -= HandleOnIndexUpdated;
            Cell.OnSelected -= HandleOnCellSelected;
        }

        private void HandleOnIndexUpdated()
        {
            OnCellIndexUpdated?.Invoke(this);
        }
    }
}