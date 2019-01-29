using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer.Abstractions
{
  public class MsSqlCommandAdapter : ISqlCommand, IDisposable
  {
    private readonly SqlCommand _command;

    public MsSqlCommandAdapter(SqlCommand command, ISqlParameterCollection parameters)
    {
      _command = command;
      Parameters = parameters;
    }

    public MsSqlCommandAdapter(SqlCommand command)
    {
      _command = command;
      Parameters = new MsSqlParameterCollectionAdapter(_command.Parameters);
    }

    public string CommandText
    {
      get => _command.CommandText;
      set => _command.CommandText = value;
    }

    public int CommandTimeout
    {
      get => _command.CommandTimeout;
      set => _command.CommandTimeout = value;
    }

    public CommandType CommandType
    {
      get => _command.CommandType;
      set => _command.CommandType = value;
    }

    public void Dispose()
    {
    }

    public int ExecuteNonQuery()
    {
      return _command.ExecuteNonQuery();
    }

    public Task<int> ExecuteNonQueryAsync()
    {
      return _command.ExecuteNonQueryAsync();
    }

    public IDataReader ExecuteReader(CommandBehavior commandBehavior)
    {
      return _command.ExecuteReader(commandBehavior);
    }

    public async Task<IDataReader> ExecuteReaderAsync(CommandBehavior commandBehavior)
    {
      var sqlDataReader = await _command.ExecuteReaderAsync(commandBehavior);
      return sqlDataReader;
    }

    public IDataReader ExecuteReader()
    {
      return _command.ExecuteReader();
    }

    public ISqlParameterCollection Parameters { get; }
  }
}
