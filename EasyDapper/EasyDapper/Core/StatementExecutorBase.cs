using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
    public abstract class StatementExecutorBase : IStatementExecutor
    {
        protected const int CommandTimeout = 300000;
        protected readonly ISqlLogger logger;
        protected IConnectionProvider connectionProvider;

        public StatementExecutorBase(ISqlLogger logger, IConnectionProvider connectionProvider)
        {
            this.logger = logger;
            this.connectionProvider = connectionProvider;
        }

        public int ExecuteNonQuery(string sql)
        {
            LogQuery(sql);
            using (var sqlConnection = connectionProvider.Provide<ISqlConnection>())
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand(null))
                {
                    command.CommandTimeout = 300000;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    return command.ExecuteNonQuery();
                }
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            LogQuery(sql);
            int num;
            using (var connection = connectionProvider.Provide<ISqlConnection>())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand(null))
                {
                    command.CommandTimeout = 300000;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    num = await command.ExecuteNonQueryAsync();
                }
            }

            return num;
        }

        public int ExecuteNonQueryStoredProcedure(
            string name,
            params ParameterDefinition[] parameterDefinitions)
        {
            LogExecuteProc(name);
            var sqlConnection = connectionProvider.Provide<ISqlConnection>();
            sqlConnection.Open();
            using (var command = sqlConnection.CreateCommand(null))
            {
                command.CommandTimeout = 300000;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = name;
                foreach (var parameterDefinition in parameterDefinitions)
                    command.Parameters.AddWithValue(parameterDefinition.Name, parameterDefinition.Value,
                        parameterDefinition.IsNullable, parameterDefinition.DbType, parameterDefinition.Size,
                        parameterDefinition.Direction);
                var num = command.ExecuteNonQuery();
                GetParameterCollection(command.Parameters.GetParameter(), parameterDefinitions);
                return num;
            }
        }

        public async Task<int> ExecuteNonQueryStoredProcedureAsync(
            string name,
            params ParameterDefinition[] parameterDefinitions)
        {
            LogExecuteProc(name);
            var connection = connectionProvider.Provide<ISqlConnection>();
            await connection.OpenAsync();
            int num;
            using (var command = connection.CreateCommand(null))
            {
                command.CommandTimeout = 300000;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = name;
                var parameterDefinitionArray = parameterDefinitions;
                for (var index = 0; index < parameterDefinitionArray.Length; ++index)
                {
                    var parameterDefinition = parameterDefinitionArray[index];
                    command.Parameters.AddWithValue(parameterDefinition.Name, parameterDefinition.Value,
                        parameterDefinition.IsNullable, parameterDefinition.DbType, parameterDefinition.Size,
                        parameterDefinition.Direction);
                    parameterDefinition = null;
                }

                parameterDefinitionArray = null;
                var result = await command.ExecuteNonQueryAsync();
                GetParameterCollection(command.Parameters.GetParameter(), parameterDefinitions);
                num = result;
            }

            return num;
        }

        public IDataReader ExecuteReader(string sql)
        {
            LogQuery(sql);
            var sqlConnection = connectionProvider.Provide<ISqlConnection>();
            sqlConnection.Open();
            using (var command = sqlConnection.CreateCommand(null))
            {
                command.CommandTimeout = 300000;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public async Task<IDataReader> ExecuteReaderAsync(string sql)
        {
            LogQuery(sql);
            var connection = connectionProvider.Provide<ISqlConnection>();
            await connection.OpenAsync();
            IDataReader dataReader;
            using (var command = connection.CreateCommand(null))
            {
                command.CommandTimeout = 300000;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                dataReader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }

            return dataReader;
        }

        public IDataReader ExecuteStoredProcedure(
            string name,
            params ParameterDefinition[] parametersDefinitions)
        {
            LogExecuteProc(name);
            var sqlConnection = connectionProvider.Provide<ISqlConnection>();
            sqlConnection.Open();
            using (var command = sqlConnection.CreateCommand(null))
            {
                command.CommandTimeout = 300000;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = name;
                foreach (var parametersDefinition in parametersDefinitions)
                    command.Parameters.AddWithValue(parametersDefinition.Name, parametersDefinition.Value,
                        parametersDefinition.IsNullable, parametersDefinition.DbType, parametersDefinition.Size,
                        parametersDefinition.Direction);
                if (parametersDefinitions.Where(m => m.Direction > ParameterDirection.Input).Count() > 0)
                {
                    command.ExecuteNonQuery();
                    GetParameterCollection(command.Parameters.GetParameter(), parametersDefinitions);
                }

                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public async Task<IDataReader> ExecuteStoredProcedureAsync(
            string name,
            params ParameterDefinition[] parametersDefinitions)
        {
            LogExecuteProc(name);
            var connection = connectionProvider.Provide<ISqlConnection>();
            await connection.OpenAsync();
            IDataReader dataReader;
            using (var command = connection.CreateCommand(null))
            {
                command.CommandTimeout = 300000;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = name;
                var parameterDefinitionArray = parametersDefinitions;
                for (var index = 0; index < parameterDefinitionArray.Length; ++index)
                {
                    var parameterDefinition = parameterDefinitionArray[index];
                    command.Parameters.AddWithValue(parameterDefinition.Name, parameterDefinition.Value,
                        parameterDefinition.IsNullable, parameterDefinition.DbType, parameterDefinition.Size,
                        parameterDefinition.Direction);
                    parameterDefinition = null;
                }

                parameterDefinitionArray = null;
                if (parametersDefinitions.Where(m => m.Direction > ParameterDirection.Input).Count() > 0)
                {
                    command.ExecuteNonQuery();
                    GetParameterCollection(command.Parameters.GetParameter(), parametersDefinitions);
                }

                dataReader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }

            return dataReader;
        }

        public IStatementExecutor UseConnectionProvider(
            IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
            return this;
        }

        protected abstract void GetParameterCollection(
            IDataParameterCollection dataParameters,
            ParameterDefinition[] parameters);

        public abstract void GetParameterCollection(
            IDataReader dataReader,
            ParameterDefinition[] parameters);

        protected void LogExecuteProc(string name)
        {
            logger.Log("Executing SP: " + name);
        }

        protected void LogQuery(string sql)
        {
            logger.Log("Executing SQL:" + Environment.NewLine + sql);
        }
    }
}