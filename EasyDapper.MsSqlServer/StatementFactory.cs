// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.StatementFactory
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

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
    private IConnectionProvider connectionProvider;
    private readonly IStatementExecutor statementExecutor;

    public IConnectionProvider GetConnectionProvider => connectionProvider;

    public IStatementExecutor StatementExecutor => statementExecutor;

    public StatementFactory(
      ISqlLogger sqlLogger,
      IConnectionProvider connectionProvider,
      IEntityMapper entityMapper,
      IStatementExecutor statementExecutor,
      IWritablePropertyMatcher writablePropertyMatcher)
    {
      this.sqlLogger = sqlLogger;
      this.connectionProvider = connectionProvider;
      this.entityMapper = entityMapper;
      this.writablePropertyMatcher = writablePropertyMatcher;
      this.statementExecutor = statementExecutor;
    }

    public IDeleteStatement<TEntity> CreateDelete<TEntity>() where TEntity : class, new()
    {
      return new DeleteStatement<TEntity>(statementExecutor, entityMapper, new WhereClauseBuilder(), writablePropertyMatcher);
    }

    public IExecuteNonQueryProcedureStatement CreateExecuteNonQueryProcedure()
    {
      return new ExecuteNonQueryProcedureStatement(statementExecutor);
    }

    public IExecuteNonQuerySqlStatement CreateExecuteNonQuerySql()
    {
      return new ExecuteNonQuerySqlStatement(statementExecutor);
    }

    public IExecuteQueryProcedureStatement<TEntity> CreateExecuteQueryProcedure<TEntity>() where TEntity : class, new()
    {
      return new ExecuteQueryProcedureStatement<TEntity>(statementExecutor, entityMapper);
    }

    public IExecuteQuerySqlStatement<TEntity> CreateExecuteQuerySql<TEntity>() where TEntity : class, new()
    {
      return new ExecuteQuerySqlStatement<TEntity>(statementExecutor, entityMapper);
    }

    public IInsertStatement<TEntity> CreateInsert<TEntity>() where TEntity : class, new()
    {
      return new InsertStatement<TEntity>(statementExecutor, entityMapper, writablePropertyMatcher);
    }

    public ISelectStatement<TEntity> CreateSelect<TEntity>() where TEntity : class, new()
    {
      return new SelectStatement<TEntity>(statementExecutor, entityMapper, writablePropertyMatcher);
    }

    public IUpdateStatement<TEntity> CreateUpdate<TEntity>() where TEntity : class, new()
    {
      return new UpdateStatement<TEntity>(statementExecutor, entityMapper, writablePropertyMatcher, new WhereClauseBuilder());
    }

    public IStatementFactory UseConnectionProvider(
      IConnectionProvider connectionProvider)
    {
      this.connectionProvider = connectionProvider;
      return this;
    }
  }
}
