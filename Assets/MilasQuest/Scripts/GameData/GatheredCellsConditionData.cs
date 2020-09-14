using MilasQuest.Stats;

namespace MilasQuest.GameData
{
    [System.Serializable]
    public class GatheredCellsConditionData : GeneralStatConditionData
    {
        public CELL_TYPES cellType;

        public GatheredCellsConditionData() : base()
        {
            statType = STAT_TYPE.GATHERED_CELLS_BY_TYPE;
        }
    }
}