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
      using (ISqlConnection sqlConnection = connectionProvider.Provide<ISqlConnection>())
      {
        sqlConnection.Open();
        using (ISqlCommand command = sqlConnection.CreateCommand(null))
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
      int num1;
      using (ISqlConnection connection = connectionProvider.Provide<ISqlConnection>())
      {
        await connection.OpenAsync();
        using (ISqlCommand command = connection.CreateCommand(null))
        {
          command.CommandTimeout = 300000;
          command.CommandType = CommandType.Text;
          command.CommandText = sql;
          int num = await command.ExecuteNonQueryAsync();
          num1 = num;
        }
      }
      return num1;
    }

    public int ExecuteNonQueryStoredProcedure(string name, params ParameterDefinition[] parameterDefinitions)
    {
      LogExecuteProc(name);
      ISqlConnection sqlConnection = connectionProvider.Provide<ISqlConnection>();
      sqlConnection.Open();
      using (ISqlCommand command = sqlConnection.CreateCommand(null))
      {
        command.CommandTimeout = 300000;
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = name;
        foreach (ParameterDefinition parameterDefinition in parameterDefinitions)
          command.Parameters.AddWithValue(parameterDefinition.Name, parameterDefinition.Value, parameterDefinition.IsNullable, parameterDefinition.DbType, parameterDefinition.Size, parameterDefinition.Direction);
        int num = command.ExecuteNonQuery();
        GetParameterCollection(command.Parameters.GetParameter(), parameterDefinitions);
        return num;
      }
    }

    public async Task<int> ExecuteNonQueryStoredProcedureAsync(string name, params ParameterDefinition[] parameterDefinitions)
    {
      LogExecuteProc(name);
      ISqlConnection connection = connectionProvider.Provide<ISqlConnection>();
      await connection.OpenAsync();
      int num1;
      using (ISqlCommand command = connection.CreateCommand(null))
      {
        command.CommandTimeout = 300000;
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = name;
        ParameterDefinition[] parameterDefinitionArray = parameterDefinitions;
        for (int index = 0; index < parameterDefinitionArray.Length; ++index)
        {
          ParameterDefinition parameterDefinition = parameterDefinitionArray[index];
          command.Parameters.AddWithValue(parameterDefinition.Name, parameterDefinition.Value, parameterDefinition.IsNullable, parameterDefinition.DbType, parameterDefinition.Size, parameterDefinition.Direction);
          parameterDefinition = null;
        }
        parameterDefinitionArray = null;
        int num = await command.ExecuteNonQueryAsync();
        int result = num;
        GetParameterCollection(command.Parameters.GetParameter(), parameterDefinitions);
        num1 = result;
      }
      return num1;
    }

    public IDataReader ExecuteReader(string sql)
    {
      LogQuery(sql);
      ISqlConnection sqlConnection = connectionProvider.Provide<ISqlConnection>();
      sqlConnection.Open();
      using (ISqlCommand command = sqlConnection.CreateCommand(null))
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
      ISqlConnection connection = connectionProvider.Provide<ISqlConnection>();
      await connection.OpenAsync();
      IDataReader dataReader1;
      using (ISqlCommand command = connection.CreateCommand(null))
      {
        command.CommandTimeout = 300000;
        command.CommandType = CommandType.Text;
        command.CommandText = sql;
        IDataReader dataReader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        dataReader1 = dataReader;
      }
      return dataReader1;
    }

    public IDataReader ExecuteStoredProcedure(string name, params ParameterDefinition[] parametersDefinitions)
    {
      LogExecuteProc(name);
      ISqlConnection sqlConnection = connectionProvider.Provide<ISqlConnection>();
      sqlConnection.Open();
      using (ISqlCommand command = sqlConnection.CreateCommand(null))
      {
        command.CommandTimeout = 300000;
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = name;
        foreach (ParameterDefinition parametersDefinition in parametersDefinitions)
          command.Parameters.AddWithValue(parametersDefinition.Name, parametersDefinition.Value, parametersDefinition.IsNullable, parametersDefinition.DbType, parametersDefinition.Size, parametersDefinition.Direction);
        if (parametersDefinitions.Where(m => m.Direction > ParameterDirection.Input).Count() > 0)
        {
          command.ExecuteNonQuery();
          GetParameterCollection(command.Parameters.GetParameter(), parametersDefinitions);
        }
        return command.ExecuteReader(CommandBehavior.CloseConnection);
      }
    }

    public async Task<IDataReader> ExecuteStoredProcedureAsync(string name, params ParameterDefinition[] parametersDefinitions)
    {
      LogExecuteProc(name);
      ISqlConnection connection = connectionProvider.Provide<ISqlConnection>();
      await connection.OpenAsync();
      IDataReader dataReader1;
      using (ISqlCommand command = connection.CreateCommand(null))
      {
        command.CommandTimeout = 300000;
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = name;
        ParameterDefinition[] parameterDefinitionArray = parametersDefinitions;
        for (int index = 0; index < parameterDefinitionArray.Length; ++index)
        {
          ParameterDefinition parameterDefinition = parameterDefinitionArray[index];
          command.Parameters.AddWithValue(parameterDefinition.Name, parameterDefinition.Value, parameterDefinition.IsNullable, parameterDefinition.DbType, parameterDefinition.Size, parameterDefinition.Direction);
          parameterDefinition = null;
        }
        parameterDefinitionArray = null;
        if (parametersDefinitions.Where(m => m.Direction > ParameterDirection.Input).Count() > 0)
        {
          command.ExecuteNonQuery();
          GetParameterCollection(command.Parameters.GetParameter(), parametersDefinitions);
        }
        IDataReader dataReader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        dataReader1 = dataReader;
      }
      return dataReader1;
    }

    public IStatementExecutor UseConnectionProvider(IConnectionProvider connectionProvider)
    {
      this.connectionProvider = connectionProvider;
      return this;
    }

    protected abstract void GetParameterCollection(IDataParameterCollection dataParameters, ParameterDefinition[] parameters);

    public abstract void GetParameterCollection(IDataReader dataReader, ParameterDefinition[] parameters);

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
