namespace MilasQuest.Grids.LinkableRules
{
    public static class LinkableRulesFactory
    {
        public static ILinkableRule GetRule(CELL_LINKING_RULE rule)
        {
            ILinkableRule newRule;
            switch (rule)
            {
                case CELL_LINKING_RULE.IS_SAME_TYPE:
                    newRule = new IsCellSameType();
                    break;
                case CELL_LINKING_RULE.IS_UNSELECTED:
                    newRule = new IsCellUnselected();
                    break;
                case CELL_LINKING_RULE.IS_NEIGHBOUR_TO_LAST:
                    newRule = new IsCellNeighbourToLast();
                    break;
                default:
                    newRule = null;
                    break;
            }
            return newRule;
        }
    }

}