// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.ConnectionProviders.MsSqlConnectionProvider
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Data;
using System.Data.Common;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;
using EasyDapper.MsSqlServer.Abstractions;

namespace EasyDapper.MsSqlServer.ConnectionProviders
{
    public abstract class MsSqlConnectionProvider : IMsSqlConnectionProvider, IConnectionProvider, IDisposable
    {
        protected string connectionString;
        protected DbConnection dbConnection;
        protected ISqlConnection sqlConnection;

        public void Dispose()
        {
            dbConnection?.Dispose();
        }

        public string GetConnectionString => connectionString;

        public DbConnection GetDbConnection
        {
            get
            {
                if (dbConnection == null)
                    dbConnection = new MsSqlConnectionAdapter(connectionString).GetDbConnection();
                return dbConnection;
            }
        }

        public TConnection Provide<TConnection>() where TConnection : class, IConnection
        {
            if (sqlConnection == null)
                sqlConnection = new MsSqlConnectionAdapter(connectionString);
            dbConnection = sqlConnection.GetDbConnection();
            return sqlConnection as TConnection;
        }

        public IDbTransaction GetDbTransaction { get; private set; }

        public IDbTransaction BeginTransaction()
        {
            GetDbTransaction = dbConnection.BeginTransaction();
            return GetDbTransaction;
        }
    }
}