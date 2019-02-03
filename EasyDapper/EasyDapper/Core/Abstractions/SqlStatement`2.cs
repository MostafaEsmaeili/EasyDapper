using System;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.Core.Abstractions
{
    public abstract class SqlStatement<TEntity, TResult> : ClauseBuilder, ISqlStatement<TResult>, IClauseBuilder
        where TEntity : class, new()
    {
        protected SqlStatement(IStatementExecutor statementExecutor, IEntityMapper entityMapper,
            IWritablePropertyMatcher writablePropertyMatcher)
        {
           ;
            StatementExecutor = statementExecutor ?? throw new ArgumentNullException(nameof(statementExecutor));
            var entityMapper1 = entityMapper;
            EntityMapper = entityMapper1 ?? throw new ArgumentNullException(nameof(entityMapper));
            TableSchema = CustomAttributeHandle.DbTableSchema<TEntity>();
            TableName = CustomAttributeHandle.DbTableName<TEntity>();
            var writablePropertyMatcher1 = writablePropertyMatcher;
            WritablePropertyMatcher = writablePropertyMatcher1 ?? throw new ArgumentNullException(nameof(writablePropertyMatcher));
        }

        protected IStatementExecutor StatementExecutor { get; }

        protected IWritablePropertyMatcher WritablePropertyMatcher { get; }

        protected IEntityMapper EntityMapper { get; }

        public string TableSchema { get; protected set; }

        public string TableName { get; protected set; }

        public abstract TResult Go();

        public abstract Task<TResult> GoAsync();

        public ISqlStatement<TResult> UseConnectionProvider(IConnectionProvider connectionProvider)
        {
            StatementExecutor.UseConnectionProvider(connectionProvider);
            return this;
        }
    }
}