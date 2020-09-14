using System.Collections.Generic;

namespace MilasQuest.Grids.LinkableRules
{
    public interface ILinkableRule
    {
        CELL_EVALUATION_OUTPUT CheckRule(List<Cell> linkedCells, Cell newCell);

        CELL_LINKING_RULE GetRuleType();
    }

}