using System.Collections.Generic;

namespace EasyDapper.Abstractions
{
    public interface IRepository<TEntity> : IRepository where TEntity : class, new()
    {
        IDeleteStatement<TEntity> Delete();

        int Delete(TEntity entity);

        IExecuteQueryProcedureStatement<TEntity> ExecuteQueryProcedure();

        IExecuteQuerySqlStatement<TEntity> ExecuteQuerySql();

        IInsertStatement<TEntity> Insert();

        TEntity Insert(TEntity entity);

        ISelectStatement<TEntity> Query();

        IEnumerable<TEntity> ResultsFrom(ISelectStatement<TEntity> query);

        IUpdateStatement<TEntity> Update();

        int Update(TEntity entity);
    }
}