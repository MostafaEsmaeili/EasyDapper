// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.Abstractions.MsSqlConnectionAdapter
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer.Abstractions
{
    public class MsSqlConnectionAdapter : ISqlConnection, IConnection, IDisposable
    {
        private readonly SqlConnection connection;

        public MsSqlConnectionAdapter(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public MsSqlConnectionAdapter()
        {
            connection = new SqlConnection();
        }

        public ISqlCommand CreateCommand(IDbTransaction dbTransaction = null)
        {
            var command = connection.CreateCommand();
            if (dbTransaction != null)
                command.Transaction = (SqlTransaction) dbTransaction;
            return new MsSqlCommandAdapter(command);
        }

        public void Dispose()
        {
            connection?.Dispose();
        }

        public void Open()
        {
            if (connection.State != ConnectionState.Closed)
                return;
            connection.Open();
        }

        public Task OpenAsync()
        {
            return connection.OpenAsync();
        }

        public DbConnection GetDbConnection()
        {
            if (connection.State == ConnectionState.Closed)
                Open();
            return connection;
        }
    }
}