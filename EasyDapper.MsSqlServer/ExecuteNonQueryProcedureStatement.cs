// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.ExecuteNonQueryProcedureStatement
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System.Linq;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer
{
    public class ExecuteNonQueryProcedureStatement : ExecuteProcedureStatement<int>, IExecuteNonQueryProcedureStatement,
        IExecuteProcedureStatement<int>
    {
        public ExecuteNonQueryProcedureStatement(IStatementExecutor statementExecutor)
            : base(statementExecutor)
        {
        }

        protected string SchemaName { get; private set; } = "dbo";

        public override int Go()
        {
            if (string.IsNullOrWhiteSpace(ProcedureName))
                throw new MissingProcedureNameException();
            var name = "[" + SchemaName + "].[" + ProcedureName + "]";
            return ParameterDefinitions.Any()
                ? StatementExecutor.ExecuteNonQueryStoredProcedure(name, ParameterDefinitions.ToArray())
                : StatementExecutor.ExecuteNonQueryStoredProcedure(name);
        }

        public override async Task<int> GoAsync()
        {
            if (string.IsNullOrWhiteSpace(ProcedureName))
                throw new MissingProcedureNameException();
            var procedureName = "[" + SchemaName + "].[" + ProcedureName + "]";
            int num;
            if (ParameterDefinitions.Any())
                num = await StatementExecutor.ExecuteNonQueryStoredProcedureAsync(procedureName,
                    ParameterDefinitions.ToArray());
            else
                num = await StatementExecutor.ExecuteNonQueryStoredProcedureAsync(procedureName);
            return num;
        }

        public IExecuteProcedureStatement<int> WithSchema(string schemaName)
        {
            SchemaName = schemaName;
            return this;
        }
    }
}