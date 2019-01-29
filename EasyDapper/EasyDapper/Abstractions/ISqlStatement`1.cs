using System.Threading.Tasks;

namespace EasyDapper.Abstractions
{
  public interface ISqlStatement<TResult> : IClauseBuilder
  {
    string TableName { get; }

    TResult Go();

    Task<TResult> GoAsync();

    ISqlStatement<TResult> UseConnectionProvider(IConnectionProvider connectionProvider);
  }
}
