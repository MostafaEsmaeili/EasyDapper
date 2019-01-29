using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
  public abstract class DeleteStatementBase<TEntity> : SqlStatement<TEntity, int>, IDeleteStatement<TEntity>, ISqlStatement<int>, IClauseBuilder where TEntity : class, new()
  {
    protected readonly IWhereClauseBuilder whereClauseBuilder;
    protected TEntity entity;

    public DeleteStatementBase(IStatementExecutor statementExecutor, IEntityMapper entityMapper, IWhereClauseBuilder whereClauseBuilder, IWritablePropertyMatcher writablePropertyMatcher)
      : base(statementExecutor, entityMapper, writablePropertyMatcher)
    {
      this.whereClauseBuilder = whereClauseBuilder;
    }

    public IDeleteStatement<TEntity> And(Expression<Func<TEntity, bool>> expression)
    {
      whereClauseBuilder.And(expression, null, null, null);
      return this;
    }

    public IDeleteStatement<TEntity> For(TEntity entity)
    {
      if (!whereClauseBuilder.IsClean)
        throw new InvalidOperationException("For cannot be used once Where has been used, please use FromScratch to reset the statement before using Where.");
      IsClean = false;
      this.entity = entity;
      return this;
    }

    public override int Go()
    {
      return StatementExecutor.ExecuteNonQuery(Sql());
    }

    public override async Task<int> GoAsync()
    {
      int num = await StatementExecutor.ExecuteNonQueryAsync(Sql());
      return num;
    }

    public IDeleteStatement<TEntity> NestedAnd(Expression<Func<TEntity, bool>> expression)
    {
      whereClauseBuilder.NestedAnd(expression, null, null, null);
      return this;
    }

    public IDeleteStatement<TEntity> NestedOr(Expression<Func<TEntity, bool>> expression)
    {
      whereClauseBuilder.NestedOr(expression, null, null, null);
      return this;
    }

    public IDeleteStatement<TEntity> Or(Expression<Func<TEntity, bool>> expression)
    {
      whereClauseBuilder.Or(expression, null, null, null);
      return this;
    }

    public IDeleteStatement<TEntity> UsingTableName(string tableName)
    {
      TableName = tableName;
      return this;
    }

    public IDeleteStatement<TEntity> UsingTableSchema(string tableSchema)
    {
      TableSchema = tableSchema;
      return this;
    }

    public IDeleteStatement<TEntity> Where(Expression<Func<TEntity, bool>> expression)
    {
      if (entity != null)
        throw new InvalidOperationException("Where cannot be used once For has been used, please use FromScratch to reset the statement before using Where.");
      IsClean = false;
      whereClauseBuilder.Where(expression, null, TableName, TableSchema);
      return this;
    }

    public IDeleteStatement<TEntity> WhereIn<T, TMember>(Expression<Func<T, TMember>> selector, TMember[] values)
    {
      whereClauseBuilder.WhereIn(selector, values, null, TableName, TableSchema);
      return this;
    }

    protected string FormatColumnValuePairs(IEnumerable<string> columnValuePairs)
    {
      return string.Join(", ", columnValuePairs);
    }
  }
}
