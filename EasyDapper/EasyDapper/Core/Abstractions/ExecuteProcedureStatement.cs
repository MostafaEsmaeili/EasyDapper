using System.Collections.Generic;
using System.Threading.Tasks;
using EasyDapper.Abstractions;

namespace EasyDapper.Core.Abstractions
{
  public abstract class ExecuteProcedureStatement<TResult> : IExecuteProcedureStatement<TResult>
  {
    protected ExecuteProcedureStatement(IStatementExecutor statementExecutor)
    {
      StatementExecutor = statementExecutor;
      ParameterDefinitions = new List<ParameterDefinition>();
    }

    protected string ProcedureName { get; set; }

    protected IStatementExecutor StatementExecutor { get; }

    public abstract TResult Go();

    public abstract Task<TResult> GoAsync();

    public IList<ParameterDefinition> ParameterDefinitions { get; }

    public IExecuteProcedureStatement<TResult> UseConnectionProvider(IConnectionProvider connectionProvider)
    {
      StatementExecutor.UseConnectionProvider(connectionProvider);
      return this;
    }

    public IExecuteProcedureStatement<TResult> WithName(string procedureName)
    {
      ProcedureName = procedureName;
      return this;
    }

    public IExecuteProcedureStatement<TResult> WithParameter(string name, object value)
    {
      ParameterDefinitions.Add(new ParameterDefinition
      {
        Name = name,
        Value = value
      });
      return this;
    }

    public IExecuteProcedureStatement<TResult> WithParameter(ParameterDefinition parameter)
    {
      ParameterDefinitions.Add(parameter);
      return this;
    }

    public IExecuteProcedureStatement<TResult> WithParameters(ParameterDefinition[] parameters)
    {
      foreach (ParameterDefinition parameter in parameters)
        ParameterDefinitions.Add(parameter);
      return this;
    }
  }
}
