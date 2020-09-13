namespace MilasQuest.Stats
{
    public static class StatUtils
    {
        public static bool DoRelationalOperation(float a, float b, RELATIONAL_OPERATOR op)
        {
            bool result = false;
            switch (op)
            {
                case RELATIONAL_OPERATOR.EQUAL:
                    result = a == b;
                    break;
                case RELATIONAL_OPERATOR.NOT_EQUAL:
                    result = a != b;
                    break;
                case RELATIONAL_OPERATOR.GREATER_THAN:
                    result = a > b;
                    break;
                case RELATIONAL_OPERATOR.LESS_THAN:
                    result = a < b;
                    break;
                case RELATIONAL_OPERATOR.GREATER_OR_EQUAL_TO:
                    result = a >= b;
                    break;
                case RELATIONAL_OPERATOR.LESS_OR_EQUAL_TO:
                    result = a <= b;
                    break;
                default:
                    break;
            }
            return result;
        }
    }

}