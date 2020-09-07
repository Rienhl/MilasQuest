using System;

namespace MilasQuest.Grids
{
    public class Cell
    {
        public int CellType { get; private set; }
        public PointInt2D Index { get; protected set; }
        public bool IsSelected { get; protected set; }

        public Action OnIndexUpdated;
        public Action OnSelected;
        public Action OnUnselected;

        public Cell(int x, int y)
        {
            UpdateIndex(x, y);
        }

        public void UpdateIndex(int x, int y)
        {
            Index = new PointInt2D() { X = x, Y = y };
            OnIndexUpdated?.Invoke();
        }

        public static bool operator ==(Cell a, Cell b)
        {
            return (a.Index == b.Index) && (a.CellType == b.CellType);
        }

        public static bool operator !=(Cell a, Cell b)
        {
            return (a.Index != b.Index) && (a.CellType != b.CellType);
        }

        internal void SetAsSelected(bool selected = true)
        {
            IsSelected = selected;
            if (IsSelected)
                OnSelected?.Invoke();
            else
                OnUnselected?.Invoke();
        }
    }
}