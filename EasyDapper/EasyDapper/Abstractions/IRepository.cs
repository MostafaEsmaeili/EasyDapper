// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.Abstractions.IRepository
// Assembly: SqlRepoEx.Core, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: A7B6EBA2-BEAE-4CC1-8497-DDC4E1B4A9B4
// Assembly location: C:\Users\Mostafa\.nuget\packages\sqlrepoex.core\2.2.4\lib\netstandard2.0\SqlRepoEx.Core.dll

using System.Data;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Abstractions
{
  public interface IRepository
  {
    IExecuteNonQueryProcedureStatement ExecuteNonQueryProcedure();

    IExecuteNonQuerySqlStatement ExecuteNonQuerySql();

    IRepository UseConnectionProvider(IConnectionProvider connectionProvider);

    IConnectionProvider GetConnectionProvider { get; }

    IDbConnection DbConnection { get; }

    IStatementExecutor StatementExecutor { get; }
  }
}
