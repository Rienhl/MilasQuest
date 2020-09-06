namespace MilasQuest.Grid
{
    [System.Serializable]
    public class GridViewProperties
    {
        public float cellSize;

        private float halfCellSize;

        public float GetHalfCellSize()
        {
            if (halfCellSize == 0)
                halfCellSize /= 0.5f;
            return halfCellSize;
        }
    }
}