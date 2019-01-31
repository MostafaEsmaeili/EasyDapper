using System.Threading.Tasks;

namespace EasyDapper.Abstractions
{
    public interface IExecuteSqlStatement<TReturn>
    {
        TReturn Go();

        Task<TReturn> GoAsync();

        IExecuteSqlStatement<TReturn> WithSql(string sql);

        IExecuteSqlStatement<TReturn> UseConnectionProvider(
            IConnectionProvider connectionProvider);
    }
}