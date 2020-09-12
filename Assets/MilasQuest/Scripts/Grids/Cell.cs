using MilasQuest.GameData;
using System;

namespace MilasQuest.Grids
{
    public class Cell
    {
        public CellTypeData CellType { get; private set; }
        public PointInt2D Index { get; protected set; }
        public bool IsSelected { get; protected set; }

        public Action OnIndexUpdated;
        public Action OnSelected;
        public Action OnUnselected;

        public Cell(int x, int y, CellTypeData cellTypeData)
        {
            CellType = cellTypeData;
            UpdateIndex(x, y);
        }

        public void UpdateIndex(int x, int y)
        {
            Index = new PointInt2D() { X = x, Y = y };
            OnIndexUpdated?.Invoke();
        }

        public void SetAsSelected(bool selected = true, bool updateView = true)
        {
            if (IsSelected == selected)
                return;
            IsSelected = selected;
            if (!updateView)
                return;
            if (IsSelected)
                OnSelected?.Invoke();
            else
                OnUnselected?.Invoke();
        }
    }
}