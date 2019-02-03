using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using EasyDapper;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace SqlRepoEx.Adapter.Dapper
{
    public class DapperStatementExecutor : IStatementExecutor
    {
        private IConnectionProvider connectionProvider;
        private readonly DbConnection dbConnection;

        public DapperStatementExecutor(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
            dbConnection = connectionProvider.GetDbConnection;
        }

        public int ExecuteNonQuery(string sql)
        {
            return dbConnection.Execute(sql, null, null, new int?(), new CommandType?());
        }

        public int ExecuteNonQuery<T>(string sql, T parameter)
        {
            var dynamicParameters = new DynamicParameters(parameter);
            return dbConnection.Execute(sql, dynamicParameters, null, new int?(), new CommandType?());

        }

        public Task<int> ExecuteNonQueryAsync(string sql)
        {
            return dbConnection.ExecuteAsync(sql, null, null, new int?(), new CommandType?());
        }

        public Task<int> ExecuteNonQueryAsync<T>(string sql, T parameter)
        {
            var dynamicParameters = new DynamicParameters(parameter);
            return dbConnection.ExecuteAsync(sql, dynamicParameters, null, new int?(), new CommandType?());
        }

        public int ExecuteNonQueryStoredProcedure(
            string name,
            params ParameterDefinition[] parameterDefinitions)
        {
            var dynamicParameters = TurnParameters(parameterDefinitions);
            return dbConnection.Execute(name, dynamicParameters, null, new int?(), CommandType.StoredProcedure);
        }

        public Task<int> ExecuteNonQueryStoredProcedureAsync(
            string name,
            params ParameterDefinition[] parameterDefinitions)
        {
            var dynamicParameters = TurnParameters(parameterDefinitions);
            return dbConnection.ExecuteAsync(name, dynamicParameters, null, new int?(), CommandType.StoredProcedure);
        }

        public IDataReader ExecuteReader(string sql)
        {
            return dbConnection.ExecuteReader(sql, null, null, new int?(), new CommandType?());
        }

        public Task<IDataReader> ExecuteReaderAsync(string sql)
        {
            return dbConnection.ExecuteReaderAsync(sql, null, null, new int?(), new CommandType?());
        }

        public IDataReader ExecuteStoredProcedure(
            string name,
            params ParameterDefinition[] parametersDefinitions)
        {
            var dynamicParameters = TurnParameters(parametersDefinitions);
            return dbConnection.ExecuteReader(name, dynamicParameters, null, new int?(), CommandType.StoredProcedure);
        }

        public Task<IDataReader> ExecuteStoredProcedureAsync(
            string name,
            params ParameterDefinition[] parametersDefinitions)
        {
            var dynamicParameters = TurnParameters(parametersDefinitions);
            return dbConnection.ExecuteReaderAsync(name, dynamicParameters, null, new int?(),
                CommandType.StoredProcedure);
        }

        public IStatementExecutor UseConnectionProvider(
            IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
            return this;
        }

        private DynamicParameters TurnParameters(
            ParameterDefinition[] parameterDefinitions)
        {
            if (parameterDefinitions == null || parameterDefinitions.Length == 0)
                return null;
            var dynamicParameters = new DynamicParameters();
            foreach (var parameterDefinition in parameterDefinitions)
                dynamicParameters.Add(parameterDefinition.Name, parameterDefinition.Value, parameterDefinition.DbType,
                    parameterDefinition.Direction, parameterDefinition.Size);
            return dynamicParameters;
        }
    }
}