using System;
using System.Linq.Expressions;

namespace EasyDapper.Abstractions
{
    public interface IInsertStatement<TEntity> : ISqlStatement<TEntity>, IClauseBuilder where TEntity : class, new()
    {
        string ParamSql(bool IsOne = true);

        ValueTuple<string, TEntity> ParamSqlWithEntity();

        IInsertStatement<TEntity> For(TEntity entity);

        IInsertStatement<TEntity> FromScratch();

        IInsertStatement<TEntity> UsingTableName(string tableName);

        IInsertStatement<TEntity> UsingIdField<TMember>(
            Expression<Func<TEntity, TMember>> idField,
            bool IsAutoInc = true);

        IInsertStatement<TEntity> With<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember value);

        IInsertStatement<TEntity> ParamWith(
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors);
    }
}