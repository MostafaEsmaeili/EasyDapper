using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using EasyDapper.Core.Abstractions;

namespace SqlRepoEx.Adapter.Dapper
{
    public class DapperEntityMapper : IEntityMapper
    {
        public IEnumerable<TEntity> Map<TEntity>(IDataReader reader) where TEntity : class, new()
        {
            return reader.Parse<TEntity>().ToList();
        }

        public TLEntity MapEntityList<TLEntity, T>(IDataReader reader)
            where TLEntity : List<T>, new()
            where T : class, new()
        {
            var lentity = new TLEntity();
            lentity.AddRange(reader.Parse<T>());
            return lentity;
        }

        public List<TEntity> MapList<TEntity>(IDataReader reader) where TEntity : class, new()
        {
            return reader.Parse<TEntity>().ToList();
        }
    }
}