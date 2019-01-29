using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
  public abstract class FromClauseBaseBuilder : ClauseBuilder, IFromClauseBuilder, IClauseBuilder
  {
    protected readonly IList<TableSpecificationBase> tableSpecifications = new List<TableSpecificationBase>();
    protected TableSpecificationBase currentTableSpecification;

    public IList<TableDefinition> TableDefinitions { get; } = (IList<TableDefinition>) new List<TableDefinition>();

    public IFromClauseBuilder From<TEntity>(string tableAlias = null, string tableName = null, string tableSchema = null)
    {
      AddTableDefinition<TEntity>(tableAlias, tableName, tableSchema);
      AddTableSpecification<TEntity>("FROM", tableName, tableSchema, tableAlias, null, null);
      IsClean = false;
      return this;
    }

    protected void AddTableDefinition<TEntity>(string tableAlias, string tableName, string tableSchema)
    {
      Type type = typeof (TEntity);
      TableDefinitions.Add(new TableDefinition
      {
        TableType = type,
        Name = string.IsNullOrEmpty(tableName) ? type.Name : tableName,
        Schema = string.IsNullOrEmpty(tableSchema) ? "dbo" : tableSchema,
        Alias = tableAlias
      });
    }

    public IFromClauseBuilder InnerJoin<TLeft, TRight>(string leftTableAlias = null, string rightTableAlias = null, string rightTableName = null, string rightTableSchema = null)
    {
      ThrowIfAliasingInvalid<TLeft, TRight>(leftTableAlias, rightTableAlias);
      AddTableDefinition<TRight>(rightTableAlias, rightTableName, rightTableSchema);
      AddTableSpecification<TRight>("INNER JOIN", rightTableName, rightTableSchema, rightTableAlias, typeof (TLeft), leftTableAlias);
      return this;
    }

    protected void ThrowIfAliasingInvalid<TLeft, TRight>(string leftTableAlias, string rightTableAlias)
    {
      if (typeof (TLeft) == typeof (TRight) && string.IsNullOrEmpty(rightTableAlias))
        throw new AliasRequiredException();
      if (TableDefinitions.Any(d =>
      {
          if (!string.IsNullOrEmpty(d.Alias))
              return d.Alias == rightTableAlias;
          return false;
      }))
        throw new DuplicateAliasException();
    }

    public IFromClauseBuilder LeftOuterJoin<TLeft, TRight>(string leftTableAlias = null, string rightTableAlias = null, string rightTableName = null, string rightTableSchema = null)
    {
      ThrowIfAliasingInvalid<TLeft, TRight>(leftTableAlias, rightTableAlias);
      AddTableDefinition<TRight>(rightTableAlias, rightTableName, rightTableSchema);
      AddTableSpecification<TRight>("LEFT OUTER JOIN", rightTableName, rightTableSchema, rightTableAlias, typeof (TLeft), leftTableAlias);
      return this;
    }

    public IFromClauseBuilder On<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expression, string leftTableAlias = null, string rightTableAlias = null)
    {
      currentTableSpecification.Conditions.Add(GetCondition(LogicalOperator.NotSet, expression));
      return this;
    }

    public IFromClauseBuilder RightOuterJoin<TLeft, TRight>(string leftTableAlias = null, string rightTableAlias = null, string rightTableName = null, string rightTableSchema = null)
    {
      ThrowIfAliasingInvalid<TLeft, TRight>(leftTableAlias, rightTableAlias);
      AddTableDefinition<TRight>(rightTableAlias, rightTableName, rightTableSchema);
      AddTableSpecification<TRight>("RIGHT OUTER JOIN", rightTableName, rightTableSchema, rightTableAlias, typeof (TLeft), leftTableAlias);
      return this;
    }

    public override string Sql()
    {
      return string.Join("\n", tableSpecifications);
    }

    public TableDefinition TableDefinition<T>(string alias = null)
    {
      return TableDefinitions.FirstOrDefault(d =>
      {
          if (!(d.TableType == typeof (T)))
              return false;
          if (!string.IsNullOrEmpty(alias))
              return d.Alias == alias;
          return true;
      });
    }

    protected abstract void AddTableSpecification<TEntity>(string specificationType, string rightTableName, string rightTableSchema, string rightTableAlias, Type leftTableType = null, string leftTableAlias = null);

    protected abstract JoinConditionBase GetCondition<TLeft, TRight>(LogicalOperator logicalOperator, Expression<Func<TLeft, TRight, bool>> expression);
  }
}
