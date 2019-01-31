using System.Collections.Generic;

namespace EasyDapper.Abstractions
{
    public interface IExecuteQuerySqlStatement<TEntity> : IExecuteSqlStatement<IEnumerable<TEntity>>
        where TEntity : class, new()
    {
    }
}