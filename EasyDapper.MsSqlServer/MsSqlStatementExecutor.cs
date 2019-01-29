// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.MsSqlStatementExecutor
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.MsSqlServer.Abstractions;

namespace EasyDapper.MsSqlServer
{
  public class MsSqlStatementExecutor : StatementExecutorBase
  {
    public MsSqlStatementExecutor(ISqlLogger logger, IMsSqlConnectionProvider connectionProvider)
      : base(logger, connectionProvider)
    {
    }

    protected override void GetParameterCollection(
      IDataParameterCollection dataParameters,
      ParameterDefinition[] parameters)
    {
      foreach (SqlParameter dataParameter in dataParameters)
      {
        var p = dataParameter;
        if (p.Direction > ParameterDirection.Input)
        {
          var parameterDefinition = parameters.Where(m => m.Name == p.ParameterName).FirstOrDefault();
          if (parameterDefinition != null)
            parameterDefinition.Value = p.Value;
        }
      }
    }

    public override void GetParameterCollection(
      IDataReader dataReader,
      ParameterDefinition[] parameters)
    {
      dataReader.GetParameterCollection(parameters);
    }
  }
}
