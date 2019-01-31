// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.ExecuteQueryProcedureStatement`1
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;
using EesyDapper.Core.CustomAttribute;

namespace EasyDapper.MsSqlServer
{
    public class ExecuteQueryProcedureStatement<TEntity> : ExecuteProcedureStatement<IEnumerable<TEntity>>,
        IExecuteQueryProcedureStatement<TEntity>, IExecuteProcedureStatement<IEnumerable<TEntity>>
        where TEntity : class, new()
    {
        private readonly IEntityMapper entityMapper;

        public ExecuteQueryProcedureStatement(
            IStatementExecutor commandExecutor,
            IEntityMapper entityMapper)
            : base(commandExecutor)
        {
            this.entityMapper = entityMapper;
        }

        protected string SchemaName { get; private set; } = "dbo";

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
            {
                return entityMapper.Map<TEntity>(reader);
            }
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
                dataReader =
                    await StatementExecutor.ExecuteStoredProcedureAsync(procedureName, ParameterDefinitions.ToArray());
            else
                dataReader = await StatementExecutor.ExecuteStoredProcedureAsync(procedureName);
            var reader = dataReader;
            dataReader = null;
            IEnumerable<TEntity> entities;
            try
            {
                entities = entityMapper.Map<TEntity>(reader);
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