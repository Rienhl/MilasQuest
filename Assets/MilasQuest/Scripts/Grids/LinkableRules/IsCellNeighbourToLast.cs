using System.Collections.Generic;

namespace MilasQuest.Grids.LinkableRules
{
    public class IsCellNeighbourToLast : ILinkableRule
    {
        public IsCellNeighbourToLast() { }

        public CELL_EVALUATION_OUTPUT CheckRule(List<Cell> linkedCells, Cell newCell)
        {
            if (linkedCells.Count == 0)
                return CELL_EVALUATION_OUTPUT.ADD;

            if (IsNeighbour(linkedCells[linkedCells.Count - 1].Index, newCell.Index))
                return CELL_EVALUATION_OUTPUT.ADD;
            else
                return CELL_EVALUATION_OUTPUT.DONT_ADD;
        }

        public CELL_LINKING_RULE GetRuleType()
        {
            return CELL_LINKING_RULE.IS_NEIGHBOUR_TO_LAST;
        }

        private bool IsNeighbour(PointInt2D previousPoint, PointInt2D newPoint)
        {
            PointInt2D[] neighbours = GridUtils.GetSurroundingPoints(previousPoint);
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] == newPoint)
                    return true;
            }
            return false;
        }
    }

}