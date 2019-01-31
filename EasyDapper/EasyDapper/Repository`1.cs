using System.Collections.Generic;
using EasyDapper.Abstractions;

namespace EasyDapper
{
    public class Repository<TEntity> : Repository, IRepository<TEntity>, IRepository where TEntity : class, new()
    {
        public Repository(IStatementFactory statementFactory)
            : base(statementFactory)
        {
        }

        public IDeleteStatement<TEntity> Delete()
        {
            return statementFactory.CreateDelete<TEntity>();
        }

        public int Delete(TEntity entity)
        {
            return statementFactory.CreateDelete<TEntity>().For(entity).Go();
        }

        public IExecuteQueryProcedureStatement<TEntity> ExecuteQueryProcedure()
        {
            return statementFactory.CreateExecuteQueryProcedure<TEntity>();
        }

        public IExecuteQuerySqlStatement<TEntity> ExecuteQuerySql()
        {
            return statementFactory.CreateExecuteQuerySql<TEntity>();
        }

        public IInsertStatement<TEntity> Insert()
        {
            return statementFactory.CreateInsert<TEntity>();
        }

        public TEntity Insert(TEntity entity)
        {
            return statementFactory.CreateInsert<TEntity>().For(entity).Go();
        }

        public ISelectStatement<TEntity> Query()
        {
            return statementFactory.CreateSelect<TEntity>();
        }

        public IEnumerable<TEntity> ResultsFrom(ISelectStatement<TEntity> query)
        {
            return query.Go();
        }

        public IUpdateStatement<TEntity> Update()
        {
            return statementFactory.CreateUpdate<TEntity>();
        }

        public int Update(TEntity entity)
        {
            return statementFactory.CreateUpdate<TEntity>().For(entity).Go();
        }
    }
}