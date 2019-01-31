// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.StatementFactory
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer
{
    public class StatementFactory : IStatementFactory
    {
        private readonly IEntityMapper entityMapper;
        private readonly ISqlLogger sqlLogger;
        private readonly IWritablePropertyMatcher writablePropertyMatcher;

        public StatementFactory(
            ISqlLogger sqlLogger,
            IConnectionProvider connectionProvider,
            IEntityMapper entityMapper,
            IStatementExecutor statementExecutor,
            IWritablePropertyMatcher writablePropertyMatcher)
        {
            this.sqlLogger = sqlLogger;
            GetConnectionProvider = connectionProvider;
            this.entityMapper = entityMapper;
            this.writablePropertyMatcher = writablePropertyMatcher;
            StatementExecutor = statementExecutor;
        }

        public IConnectionProvider GetConnectionProvider { get; private set; }

        public IStatementExecutor StatementExecutor { get; }

        public IDeleteStatement<TEntity> CreateDelete<TEntity>() where TEntity : class, new()
        {
            return new DeleteStatement<TEntity>(StatementExecutor, entityMapper, new WhereClauseBuilder(),
                writablePropertyMatcher);
        }

        public IExecuteNonQueryProcedureStatement CreateExecuteNonQueryProcedure()
        {
            return new ExecuteNonQueryProcedureStatement(StatementExecutor);
        }

        public IExecuteNonQuerySqlStatement CreateExecuteNonQuerySql()
        {
            return new ExecuteNonQuerySqlStatement(StatementExecutor);
        }

        public IExecuteQueryProcedureStatement<TEntity> CreateExecuteQueryProcedure<TEntity>()
            where TEntity : class, new()
        {
            return new ExecuteQueryProcedureStatement<TEntity>(StatementExecutor, entityMapper);
        }

        public IExecuteQuerySqlStatement<TEntity> CreateExecuteQuerySql<TEntity>() where TEntity : class, new()
        {
            return new ExecuteQuerySqlStatement<TEntity>(StatementExecutor, entityMapper);
        }

        public IInsertStatement<TEntity> CreateInsert<TEntity>() where TEntity : class, new()
        {
            return new InsertStatement<TEntity>(StatementExecutor, entityMapper, writablePropertyMatcher);
        }

        public ISelectStatement<TEntity> CreateSelect<TEntity>() where TEntity : class, new()
        {
            return new SelectStatement<TEntity>(StatementExecutor, entityMapper, writablePropertyMatcher);
        }

        public IUpdateStatement<TEntity> CreateUpdate<TEntity>() where TEntity : class, new()
        {
            return new UpdateStatement<TEntity>(StatementExecutor, entityMapper, writablePropertyMatcher,
                new WhereClauseBuilder());
        }

        public IStatementFactory UseConnectionProvider(
            IConnectionProvider connectionProvider)
        {
            GetConnectionProvider = connectionProvider;
            return this;
        }
    }
}