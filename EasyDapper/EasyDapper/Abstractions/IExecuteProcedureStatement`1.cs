using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDapper.Abstractions
{
  public interface IExecuteProcedureStatement<TReturn>
  {
    IList<ParameterDefinition> ParameterDefinitions { get; }

    TReturn Go();

    Task<TReturn> GoAsync();

    IExecuteProcedureStatement<TReturn> WithName(string procedureName);

    IExecuteProcedureStatement<TReturn> WithParameter(string name, object value);

    IExecuteProcedureStatement<TReturn> WithParameter(ParameterDefinition parameter);

    IExecuteProcedureStatement<TReturn> WithParameters(ParameterDefinition[] parameters);

    IExecuteProcedureStatement<TReturn> UseConnectionProvider(IConnectionProvider connectionProvider);
  }
}
