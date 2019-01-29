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
    private readonly SqlConnection _connection;

    public MsSqlConnectionAdapter(string connectionString)
    {
      _connection = new SqlConnection(connectionString);
    }

    public MsSqlConnectionAdapter()
    {
      _connection = new SqlConnection();
    }

    public ISqlCommand CreateCommand(IDbTransaction dbTransaction = null)
    {
      var command = _connection.CreateCommand();
      if (dbTransaction != null)
        command.Transaction = (SqlTransaction) dbTransaction;
      return new MsSqlCommandAdapter(command);
    }

    public void Dispose()
    {
      _connection.Dispose();
    }

    public void Open()
    {
      if (_connection.State != ConnectionState.Closed)
        return;
      _connection.Open();
    }

    public Task OpenAsync()
    {
      return _connection.OpenAsync();
    }

    public DbConnection GetDbConnection()
    {
      if (_connection.State == ConnectionState.Closed)
        Open();
      return _connection;
    }
  }
}
