using System;
using System.Data;
using System.Threading.Tasks;

namespace EasyDapper.Core.Abstractions
{
    public interface ISqlCommand : IDisposable
    {
        int CommandTimeout { get; set; }

        CommandType CommandType { get; set; }

        string CommandText { get; set; }

        ISqlParameterCollection Parameters { get; }

        int ExecuteNonQuery();

        Task<int> ExecuteNonQueryAsync();

        IDataReader ExecuteReader(CommandBehavior closeConnection);

        IDataReader ExecuteReader();

        Task<IDataReader> ExecuteReaderAsync(CommandBehavior closeConnection);
    }
}