// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.Abstractions.MsSqlCommandAdapter
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer.Abstractions
{
    public class MsSqlCommandAdapter : ISqlCommand, IDisposable
    {
        private readonly SqlCommand command;

        public MsSqlCommandAdapter(SqlCommand command, ISqlParameterCollection parameters)
        {
            this.command = command;
            Parameters = parameters;
        }

        public MsSqlCommandAdapter(SqlCommand command)
        {
            this.command = command;
            Parameters = new MsSqlParameterCollectionAdapter(this.command.Parameters);
        }

        public string CommandText
        {
            get => command.CommandText;
            set => command.CommandText = value;
        }

        public int CommandTimeout
        {
            get => command.CommandTimeout;
            set => command.CommandTimeout = value;
        }

        public CommandType CommandType
        {
            get => command.CommandType;
            set => command.CommandType = value;
        }

        public void Dispose()
        {
        }

        public int ExecuteNonQuery()
        {
            return command.ExecuteNonQuery();
        }

        public Task<int> ExecuteNonQueryAsync()
        {
            return command.ExecuteNonQueryAsync();
        }

        public IDataReader ExecuteReader(CommandBehavior commandBehavior)
        {
            return command.ExecuteReader(commandBehavior);
        }

        public async Task<IDataReader> ExecuteReaderAsync(CommandBehavior commandBehavior)
        {
            var sqlDataReader = await command.ExecuteReaderAsync(commandBehavior);
            return sqlDataReader;
        }

        public IDataReader ExecuteReader()
        {
            return command.ExecuteReader();
        }

        public ISqlParameterCollection Parameters { get; }
    }
}