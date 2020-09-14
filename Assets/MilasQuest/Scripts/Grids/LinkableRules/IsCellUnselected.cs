using System.Collections.Generic;

namespace MilasQuest.Grids.LinkableRules
{
    public class IsCellUnselected : ILinkableRule
    {
        public IsCellUnselected() { }

        public CELL_EVALUATION_OUTPUT CheckRule(List<Cell> linkedCells, Cell newCell)
        {
            if (!newCell.IsSelected)
                return CELL_EVALUATION_OUTPUT.ADD;
            else
            {
                if (linkedCells.Count == 1)
                    return CELL_EVALUATION_OUTPUT.DONT_ADD;

                if (linkedCells[linkedCells.Count - 2] == newCell)
                    return CELL_EVALUATION_OUTPUT.REMOVE_PREVIOUS;
                else
                    return CELL_EVALUATION_OUTPUT.DONT_ADD;
            }
        }

        public CELL_LINKING_RULE GetRuleType()
        {
            return CELL_LINKING_RULE.IS_UNSELECTED;
        }
    }

}