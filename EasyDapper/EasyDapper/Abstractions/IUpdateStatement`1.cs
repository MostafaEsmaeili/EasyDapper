using System;
using System.Linq.Expressions;

namespace EasyDapper.Abstractions
{
    public interface IUpdateStatement<TEntity> : ISqlStatement<int>, IClauseBuilder where TEntity : class, new()
    {
        string ParamSql();

        ValueTuple<string, TEntity> ParamSqlWithEntity();

        IUpdateStatement<TEntity> UsingTableName(string tableName);

        IUpdateStatement<TEntity> And(Expression<Func<TEntity, bool>> expression);

        IUpdateStatement<TEntity> For(TEntity entity);

        IUpdateStatement<TEntity> NestedAnd(Expression<Func<TEntity, bool>> expression);

        IUpdateStatement<TEntity> NestedOr(Expression<Func<TEntity, bool>> expression);

        IUpdateStatement<TEntity> Or(Expression<Func<TEntity, bool>> expression);

        IUpdateStatement<TEntity> Set<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember value,
            string tableSchema = null,
            string tableName = null);

        IUpdateStatement<TEntity> ParamSet(
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors);

        IUpdateStatement<TEntity> Where(Expression<Func<TEntity, bool>> expression);

        IUpdateStatement<TEntity> WhereIn<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember[] values);
    }
}