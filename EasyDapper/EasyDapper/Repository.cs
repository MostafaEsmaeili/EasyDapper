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

        public IConnectionProvider GetConnectionProvider => statementFactory.GetConnectionProvider;

        public IDbConnection DbConnection => GetConnectionProvider.GetDbConnection;

        public IStatementExecutor StatementExecutor => statementFactory.StatementExecutor;

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