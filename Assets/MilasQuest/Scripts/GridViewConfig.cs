namespace MilasQuest.Grids
{
    /// <summary>
    /// Contains any visual configuration for our current grid state
    /// This allows for different grid views applied to the same grid state
    /// </summary>
    [System.Serializable]
    public class GridViewConfig
    {
        public float cellSize;
        public float validInputRatio;
    }
}