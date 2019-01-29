using System.Data;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper
{
  public class Repository : IRepository
  {
    protected readonly IStatementFactory statementFactory;

    public Repository(IStatementFactory statementFactory)
    {
      this.statementFactory = statementFactory;
    }

    public IConnectionProvider GetConnectionProvider
    {
      get
      {
        return statementFactory.GetConnectionProvider;
      }
    }

    public IDbConnection DbConnection
    {
      get
      {
        return GetConnectionProvider.GetDbConnection;
      }
    }

    public IStatementExecutor StatementExecutor
    {
      get
      {
        return statementFactory.StatementExecutor;
      }
    }

    public IExecuteNonQueryProcedureStatement ExecuteNonQueryProcedure()
    {
      return statementFactory.CreateExecuteNonQueryProcedure();
    }

    public IExecuteNonQuerySqlStatement ExecuteNonQuerySql()
    {
      return statementFactory.CreateExecuteNonQuerySql();
    }

    public IRepository UseConnectionProvider(IConnectionProvider connectionProvider)
    {
      statementFactory.UseConnectionProvider(connectionProvider);
      return this;
    }
  }
}
