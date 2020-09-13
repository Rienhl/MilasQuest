namespace MilasQuest.Stats
{
    public class GatheredCellsStat : Stat
    {
        public CELL_TYPES CellType { get; protected set; }

        public GatheredCellsStat(CELL_TYPES cellType, float startingValue = 0) : base(STAT_TYPE.GATHERED_CELLS_BY_TYPE, startingValue)
        {
            CellType = cellType;
        }
    }
}