using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
    public class GroupByClauseBuilder : GroupByClauseBaseBuilder
    {
        protected override void AddGroupBySpecification<TEntity>(
            string alias,
            string tableName,
            string tableSchema,
            string name)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = TableNameFromType<TEntity>();
            if (string.IsNullOrWhiteSpace(tableSchema))
                tableSchema = "dbo";
            var bySpecifications = GroupBySpecifications;
            var groupBySpecification = new GroupBySpecification
            {
                Alias = alias, Table = tableName, Schema = tableSchema, Name = name
            };
            bySpecifications.Add(groupBySpecification);
        }

        protected override void AddHavingSpecification<TEntity>(
            string alias,
            string tableName,
            string tableSchema,
            string name,
            Aggregation aggregation,
            Comparison comparison,
            object value)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = TableNameFromType<TEntity>();
            if (string.IsNullOrWhiteSpace(tableSchema))
                tableSchema = "dbo";
            var havingSpecifications = HavingSpecifications;
            var havingSpecification = new HavingSpecification
            {
                Aggregation = aggregation,
                Alias = alias,
                Table = tableName,
                Schema = tableSchema,
                Name = name,
                Comparison = comparison,
                Value = value
            };
            havingSpecifications.Add(havingSpecification);
        }
    }
}
