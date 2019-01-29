using System;
using System.Linq.Expressions;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
  public abstract class WhereClauseBaseBuilder : ClauseBuilder, IWhereClauseBuilder, IClauseBuilder
  {
    protected WhereClauseGroupBase currentGroup;
    protected WhereClauseGroupBase rootGroup;

    public IWhereClauseBuilder And<TEntity>(Expression<Func<TEntity, bool>> expression, string alias = null, string tableName = null, string tableSchema = null)
    {
      ThrowIfNotInitialised();
      return AddConditionToCurrentGroup(expression, LogicalOperator.And, alias, tableName, tableSchema);
    }

    public IWhereClauseBuilder AndBetween<TEntity, TMember>(Expression<Func<TEntity, TMember>> selector, TMember start, TMember end, string alias = null, string tableName = null, string tableSchema = null)
    {
      AddBetweenConditionToCurrentGroup(selector, start, end, LogicalOperator.And, alias, tableName, tableSchema);
      return this;
    }

    public IWhereClauseBuilder AndIn<TEntity, TMember>(Expression<Func<TEntity, TMember>> selector, TMember[] values, string alias = null, string tableName = null, string tableSchema = null)
    {
      ThrowIfNotInitialised();
      AddInConditionToCurrentGroup(selector, values, LogicalOperator.And, alias, tableName, tableSchema);
      return this;
    }

    public IWhereClauseBuilder EndNesting()
    {
      if (currentGroup != null && currentGroup.Parent != null)
        currentGroup = currentGroup.Parent;
      return this;
    }

    public IWhereClauseBuilder NestedAnd<TEntity>(Expression<Func<TEntity, bool>> expression, string alias = null, string tableName = null, string tableSchema = null)
    {
      return AddNestedGroupToCurrentGroup(expression, WhereClauseGroupType.And, alias, tableName, tableSchema);
    }

    public IWhereClauseBuilder NestedOr<TEntity>(Expression<Func<TEntity, bool>> expression, string alias = null, string tableName = null, string tableSchema = null)
    {
      return AddNestedGroupToCurrentGroup(expression, WhereClauseGroupType.Or, alias, tableName, tableSchema);
    }

    public IWhereClauseBuilder Or<TEntity>(Expression<Func<TEntity, bool>> expression, string alias = null, string tableName = null, string tableSchema = null)
    {
      return AddConditionToCurrentGroup(expression, LogicalOperator.Or, alias, tableName, tableSchema);
    }

    public IWhereClauseBuilder OrBetween<TEntity, TMember>(Expression<Func<TEntity, TMember>> selector, TMember start, TMember end, string alias = null, string tableName = null, string tableSchema = null)
    {
      AddBetweenConditionToCurrentGroup(selector, start, end, LogicalOperator.Or, alias, tableName, tableSchema);
      return this;
    }

    public IWhereClauseBuilder OrIn<TEntity, TMember>(Expression<Func<TEntity, TMember>> selector, TMember[] values, string alias = null, string tableName = null, string tableSchema = null)
    {
      ThrowIfNotInitialised();
      AddInConditionToCurrentGroup(selector, values, LogicalOperator.Or, alias, tableName, tableSchema);
      return this;
    }

    public override string Sql()
    {
      return rootGroup?.ToString() ?? string.Empty;
    }

    public IWhereClauseBuilder Where<TEntity>(Expression<Func<TEntity, bool>> expression, string alias = null, string tableName = null, string tableSchema = null)
    {
      Initialise();
      AddConditionToCurrentGroup(expression, LogicalOperator.NotSet, alias, tableName, tableSchema);
      IsClean = false;
      return this;
    }

    public IWhereClauseBuilder WhereBetween<TEntity, TMember>(Expression<Func<TEntity, TMember>> selector, TMember start, TMember end, string alias = null, string tableName = null, string tableSchema = null)
    {
      Initialise();
      AddBetweenConditionToCurrentGroup(selector, start, end, LogicalOperator.NotSet, alias, tableName, tableSchema);
      return this;
    }

    public IWhereClauseBuilder WhereIn<TEntity, TMember>(Expression<Func<TEntity, TMember>> selector, TMember[] values, string alias = null, string tableName = null, string tableSchema = null)
    {
      Initialise();
      AddInConditionToCurrentGroup(selector, values, LogicalOperator.NotSet, alias, tableName, tableSchema);
      return this;
    }

    protected abstract void AddBetweenConditionToCurrentGroup<TEntity, TMember>(Expression<Func<TEntity, TMember>> selector, TMember start, TMember end, LogicalOperator locigalOperator, string alias, string tableName, string tableSchema);

    protected abstract IWhereClauseBuilder AddConditionToCurrentGroup<TEntity>(Expression<Func<TEntity, bool>> expression, LogicalOperator locigalOperator, string alias = null, string tableName = null, string tableSchema = null);

    protected abstract void AddInConditionToCurrentGroup<TEntity, TMember>(Expression<Func<TEntity, TMember>> selector, TMember[] values, LogicalOperator locigalOperator, string alias, string tableName, string tableSchema);

    protected abstract IWhereClauseBuilder AddNestedGroupToCurrentGroup<TEntity>(Expression<Func<TEntity, bool>> expression, WhereClauseGroupType groupType, string alias = null, string tableName = null, string tableSchema = null);

    protected string GetActualOperator(string operatorString, string value)
    {
      return value != "NULL" ? operatorString : (operatorString == "=" ? "IS" : "IS NOT");
    }

    protected abstract void Initialise();

    protected void ThrowIfNotInitialised()
    {
      if (rootGroup == null)
        throw new InvalidOperationException("Where must be used before any additional conditions can be applied.");
    }
  }
}
