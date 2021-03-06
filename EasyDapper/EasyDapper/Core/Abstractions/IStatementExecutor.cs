﻿using System.Data;
using System.Threading.Tasks;
using EasyDapper.Abstractions;

namespace EasyDapper.Core.Abstractions
{
    public interface IStatementExecutor
    {
        int ExecuteNonQuery(string sql);
        int ExecuteNonQuery<T>(string sql,T parameter);

        Task<int> ExecuteNonQueryAsync(string sql);
        Task<int> ExecuteNonQueryAsync<T>(string sql,T parameter);


        int ExecuteNonQueryStoredProcedure(string name, params ParameterDefinition[] parameterDefinitions);

        Task<int> ExecuteNonQueryStoredProcedureAsync(string name, params ParameterDefinition[] parameterDefinitions);

        IDataReader ExecuteReader(string sql);

        Task<IDataReader> ExecuteReaderAsync(string sql);

        IDataReader ExecuteStoredProcedure(string name, params ParameterDefinition[] parametersDefinitions);

        Task<IDataReader> ExecuteStoredProcedureAsync(string name, params ParameterDefinition[] parametersDefinitions);

        IStatementExecutor UseConnectionProvider(IConnectionProvider connectionProvider);
    }
}