using System;

namespace EasyDapper.Core
{
    public abstract class SelectStatementHavingSpecificationBase
    {
        public Aggregation Aggregation { get; set; }

        public string Alias { get; set; }

        public Type EntityType { get; set; }

        public string Identifier { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }

        public string Schema { get; set; }

        public string TableName { get; set; }

        protected string ApplyAggregation(string columnExpression)
        {
            if (Aggregation == Aggregation.Count && Identifier == "*")
                return "COUNT(*)";
            return Aggregation.ToString().ToUpperInvariant() + "(" + columnExpression + ")";
        }
    }
}