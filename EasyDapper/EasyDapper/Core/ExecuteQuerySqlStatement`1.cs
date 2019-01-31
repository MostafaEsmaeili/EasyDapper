using System.Collections.Generic;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
    public class ExecuteQuerySqlStatement<TEntity> : ExecuteSqlStatement<IEnumerable<TEntity>>,
        IExecuteQuerySqlStatement<TEntity>, IExecuteSqlStatement<IEnumerable<TEntity>> where TEntity : class, new()
    {
        private readonly IEntityMapper entityMapper;

        public ExecuteQuerySqlStatement(IStatementExecutor commandExecutor, IEntityMapper entityMapper)
            : base(commandExecutor)
        {
            this.entityMapper = entityMapper;
        }

        public override IEnumerable<TEntity> Go()
        {
            if (string.IsNullOrWhiteSpace(Sql))
                throw new MissingSqlException();
            using (var reader = StatementExecutor.ExecuteReader(Sql))
            {
                return entityMapper.Map<TEntity>(reader);
            }
        }

        public override async Task<IEnumerable<TEntity>> GoAsync()
        {
            if (string.IsNullOrWhiteSpace(Sql))
                throw new MissingSqlException();
            IEnumerable<TEntity> entities;
            using (var reader = await StatementExecutor.ExecuteReaderAsync(Sql))
            {
                entities = entityMapper.Map<TEntity>(reader);
            }

            return entities;
        }
    }
}