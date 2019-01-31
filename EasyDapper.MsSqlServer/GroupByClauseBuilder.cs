// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.GroupByClauseBuilder
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

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
            var bySpecifications = groupBySpecifications;
            var groupBySpecification = new GroupBySpecification();
            groupBySpecification.Alias = alias;
            groupBySpecification.Table = tableName;
            groupBySpecification.Schema = tableSchema;
            groupBySpecification.Name = name;
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
            var havingSpecifications = this.havingSpecifications;
            var havingSpecification = new HavingSpecification();
            havingSpecification.Aggregation = aggregation;
            havingSpecification.Alias = alias;
            havingSpecification.Table = tableName;
            havingSpecification.Schema = tableSchema;
            havingSpecification.Name = name;
            havingSpecification.Comparison = comparison;
            havingSpecification.Value = value;
            havingSpecifications.Add(havingSpecification);
        }
    }
}