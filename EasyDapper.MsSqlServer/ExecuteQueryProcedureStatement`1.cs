using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.MsSqlServer
{
    public class ExecuteQueryProcedureStatement<TEntity> : ExecuteProcedureStatement<IEnumerable<TEntity>>,
        IExecuteQueryProcedureStatement<TEntity>, IExecuteProcedureStatement<IEnumerable<TEntity>>
        where TEntity : class, new()
    {
        private readonly IEntityMapper _entityMapper;

        protected string SchemaName { get; private set; } = "dbo";

        public ExecuteQueryProcedureStatement(
            IStatementExecutor commandExecutor,
            IEntityMapper entityMapper)
            : base(commandExecutor)
        {
            _entityMapper = entityMapper;
        }

        public override IEnumerable<TEntity> Go()
        {
            if (string.IsNullOrWhiteSpace(ProcedureName))
                ProcedureName = CustomAttributeHandle.DbTableName<TEntity>();
            if (string.IsNullOrWhiteSpace(ProcedureName))
                throw new MissingProcedureNameException();
            var name = "[" + SchemaName + "].[" + ProcedureName + "]";
            using (var reader = ParameterDefinitions.Any()
                ? StatementExecutor.ExecuteStoredProcedure(name, ParameterDefinitions.ToArray())
                : StatementExecutor.ExecuteStoredProcedure(name))
                return _entityMapper.Map<TEntity>(reader);
        }

        public override async Task<IEnumerable<TEntity>> GoAsync()
        {
            if (string.IsNullOrWhiteSpace(ProcedureName))
                ProcedureName = CustomAttributeHandle.DbTableName<TEntity>();
            if (string.IsNullOrWhiteSpace(ProcedureName))
                throw new MissingProcedureNameException();
            var procedureName = "[" + SchemaName + "].[" + ProcedureName + "]";
            IDataReader dataReader;
            if (ParameterDefinitions.Any())
                dataReader = await StatementExecutor.ExecuteStoredProcedureAsync(procedureName,
                    ParameterDefinitions.ToArray());
            else
                dataReader = await StatementExecutor.ExecuteStoredProcedureAsync(procedureName);
            var reader = dataReader;
            IEnumerable<TEntity> entities;
            try
            {
                entities = _entityMapper.Map<TEntity>(reader);
            }
            finally
            {
                reader?.Dispose();
            }

            return entities;
        }

        public IExecuteQueryProcedureStatement<TEntity> WithSchema(
            string schemaName)
        {
            SchemaName = schemaName;
            return this;
        }
    }
}
