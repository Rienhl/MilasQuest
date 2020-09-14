using System.Collections.Generic;

namespace MilasQuest.Grids.LinkableRules
{
    public class IsCellSameType : ILinkableRule
    {
        public IsCellSameType() { }

        public CELL_EVALUATION_OUTPUT CheckRule(List<Cell> linkedCells, Cell newCell)
        {
            if (linkedCells.Count == 0)
                return CELL_EVALUATION_OUTPUT.ADD;

            if (linkedCells[linkedCells.Count - 1].CellType == newCell.CellType)
                return CELL_EVALUATION_OUTPUT.ADD;
            else
                return CELL_EVALUATION_OUTPUT.DONT_ADD;
        }

        public CELL_LINKING_RULE GetRuleType()
        {
            return CELL_LINKING_RULE.IS_SAME_TYPE;
        }
    }

}