using System.Threading.Tasks;
using EasyDapper.Abstractions;

namespace EasyDapper.Core.Abstractions
{
    public abstract class ExecuteSqlStatement<TResult> : IExecuteSqlStatement<TResult>
    {
        protected ExecuteSqlStatement(IStatementExecutor statementExecutor)
        {
            StatementExecutor = statementExecutor;
        }

        protected string Sql { get; private set; }

        protected IStatementExecutor StatementExecutor { get; }

        public abstract TResult Go();

        public abstract Task<TResult> GoAsync();

        public IExecuteSqlStatement<TResult> UseConnectionProvider(IConnectionProvider connectionProvider)
        {
            StatementExecutor.UseConnectionProvider(connectionProvider);
            return this;
        }

        public IExecuteSqlStatement<TResult> WithSql(string sql)
        {
            Sql = sql;
            return this;
        }
    }
}