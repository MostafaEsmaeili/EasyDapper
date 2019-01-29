// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.MsSqlStatementFactoryProvider
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;
using EasyDapper.MsSqlServer.Abstractions;

namespace EasyDapper.MsSqlServer
{
  public class MsSqlStatementFactoryProvider : IStatementFactoryProvider
  {
    private readonly IConnectionProvider connectionProvider;
    private readonly IEntityMapper entityMapper;
    private readonly ISqlLogger sqlLogger;
    private readonly IWritablePropertyMatcher writablePropertyMatcher;
    private readonly IStatementExecutor statementExecutor;

    public MsSqlStatementFactoryProvider(
      IEntityMapper entityMapper,
      IWritablePropertyMatcher writablePropertyMatcher,
      IMsSqlConnectionProvider connectionProvider,
      IStatementExecutor statementExecutor,
      ISqlLogger sqlLogger)
    {
      this.entityMapper = entityMapper;
      this.writablePropertyMatcher = writablePropertyMatcher;
      this.connectionProvider = connectionProvider;
      this.sqlLogger = sqlLogger;
      this.statementExecutor = statementExecutor;
    }

    public IConnectionProvider GetConnectionProvider => connectionProvider;

    public IStatementFactory Provide()
    {
      return new StatementFactory(sqlLogger, connectionProvider, entityMapper, statementExecutor, writablePropertyMatcher);
    }
  }
}
