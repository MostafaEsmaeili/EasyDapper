// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.MsSqlStatementFactoryProvider
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;
using EasyDapper.MsSqlServer.Abstractions;

namespace EasyDapper.MsSqlServer
{
    public class MsSqlStatementFactoryProvider : IStatementFactoryProvider
    {
        private readonly IEntityMapper entityMapper;
        private readonly ISqlLogger sqlLogger;
        private readonly IStatementExecutor statementExecutor;
        private readonly IWritablePropertyMatcher writablePropertyMatcher;

        public MsSqlStatementFactoryProvider(
            IEntityMapper entityMapper,
            IWritablePropertyMatcher writablePropertyMatcher,
            IMsSqlConnectionProvider connectionProvider,
            IStatementExecutor statementExecutor,
            ISqlLogger sqlLogger)
        {
            this.entityMapper = entityMapper;
            this.writablePropertyMatcher = writablePropertyMatcher;
            GetConnectionProvider = connectionProvider;
            this.sqlLogger = sqlLogger;
            this.statementExecutor = statementExecutor;
        }

        public IConnectionProvider GetConnectionProvider { get; }

        public IStatementFactory Provide()
        {
            return new StatementFactory(sqlLogger, GetConnectionProvider, entityMapper, statementExecutor,
                writablePropertyMatcher);
        }
    }
}