using System;

namespace MilasQuest.Grid
{
    public class Cell
    {
        public PointInt2D Index { get; protected set; }

        public Action OnIndexUpdated;

        public Cell(int x, int y)
        {
            UpdateIndex(x, y);
        }

        public void UpdateIndex(int x, int y)
        {
            Index = new PointInt2D() { X = x, Y = y };
            OnIndexUpdated?.Invoke();
        }
    }
}