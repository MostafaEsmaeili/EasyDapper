using System;

namespace EasyDapper.Core
{
    public abstract class JoinSpecificationBase
    {
        public Type LeftEntityType { get; set; }

        public string LeftIdentifier { get; set; }

        public string LeftTableAlias { get; set; }

        public LogicalOperator LogicalOperator { get; set; }

        public string Operator { get; set; }

        public Type RightEntityType { get; set; }

        public string RightIdentifier { get; set; }

        public string RightTableAlias { get; set; }

        public string LeftSchema { get; set; }

        public string LeftTableName { get; set; }

        public string RightSchema { get; set; }

        public string RightTableName { get; set; }

        protected string GetPrefix()
        {
            switch (LogicalOperator)
            {
                case LogicalOperator.And:
                    return "AND";
                case LogicalOperator.Or:
                    return "OR";
                default:
                    return "ON";
            }
        }
    }
}