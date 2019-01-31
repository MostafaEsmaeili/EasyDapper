using System;

namespace EasyDapper.Core
{
    public abstract class ColumnSpecificationBase
    {
        public Aggregation Aggregation { get; set; }

        public string Alias { get; set; }

        public Type EntityType { get; set; }

        public string Identifier { get; set; }

        public string Table { get; set; }

        public string Schema { get; set; }

        public string ColumnName { get; set; }

        public string AggregationColumnName { get; set; }
    }
}