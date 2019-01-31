using System;

namespace EasyDapper.Core
{
    public abstract class FilterConditionBase
    {
        public string Alias { get; set; }

        public Type EntityType { get; set; }

        public string Left { get; set; }

        public LogicalOperator LocigalOperator { get; set; }

        public string Operator { get; set; }

        public string Right { get; set; }

        public string Schema { get; set; }

        public string TableName { get; set; }

        public string LambdaTree { get; set; }
    }
}