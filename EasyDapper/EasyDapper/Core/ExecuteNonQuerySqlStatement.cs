using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
    public class ExecuteNonQuerySqlStatement : ExecuteSqlStatement<int>, IExecuteNonQuerySqlStatement,
        IExecuteSqlStatement<int>
    {
        public ExecuteNonQuerySqlStatement(IStatementExecutor statementExecutor)
            : base(statementExecutor)
        {
        }

        public override int Go()
        {
            if (string.IsNullOrWhiteSpace(Sql))
                throw new MissingSqlException();
            return StatementExecutor.ExecuteNonQuery(Sql);
        }

        public override async Task<int> GoAsync()
        {
            if (string.IsNullOrWhiteSpace(Sql))
                throw new MissingSqlException();
            var num = await StatementExecutor.ExecuteNonQueryAsync(Sql);
            return num;
        }
    }
}