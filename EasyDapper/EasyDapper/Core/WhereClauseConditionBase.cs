namespace EasyDapper.Core
{
    public abstract class WhereClauseConditionBase
    {
        public string Alias { get; set; }

        public string LeftSchema { get; set; }

        public string LeftTable { get; set; }

        public string Left { get; set; }

        public LogicalOperator LocigalOperator { get; set; }

        public string Operator { get; set; }

        public string Right { get; set; }
    }
}