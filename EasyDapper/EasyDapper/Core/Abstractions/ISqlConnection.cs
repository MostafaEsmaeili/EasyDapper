using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using EasyDapper.Abstractions;

namespace EasyDapper.Core.Abstractions
{
  public interface ISqlConnection : IConnection, IDisposable
  {
    void Open();

    ISqlCommand CreateCommand(IDbTransaction dbTransaction = null);

    Task OpenAsync();

    DbConnection GetDbConnection();
  }
}
