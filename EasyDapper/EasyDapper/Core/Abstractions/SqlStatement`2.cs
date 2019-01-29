using System;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.Core.Abstractions
{
  public abstract class SqlStatement<TEntity, TResult> : ClauseBuilder, ISqlStatement<TResult>, IClauseBuilder where TEntity : class, new()
  {
    protected SqlStatement(IStatementExecutor statementExecutor, IEntityMapper entityMapper, IWritablePropertyMatcher writablePropertyMatcher)
    {
      IStatementExecutor statementExecutor1 = statementExecutor;
      if (statementExecutor1 == null)
        throw new ArgumentNullException(nameof (statementExecutor));
      StatementExecutor = statementExecutor1;
      IEntityMapper entityMapper1 = entityMapper;
      if (entityMapper1 == null)
        throw new ArgumentNullException(nameof (entityMapper));
      EntityMapper = entityMapper1;
      TableSchema = CustomAttributeHandle.DbTableSchema<TEntity>();
      TableName = CustomAttributeHandle.DbTableName<TEntity>();
      IWritablePropertyMatcher writablePropertyMatcher1 = writablePropertyMatcher;
      if (writablePropertyMatcher1 == null)
        throw new ArgumentNullException(nameof (writablePropertyMatcher));
      WritablePropertyMatcher = writablePropertyMatcher1;
    }

    protected IStatementExecutor StatementExecutor { get; }

    protected IWritablePropertyMatcher WritablePropertyMatcher { get; }

    protected IEntityMapper EntityMapper { get; }

    public string TableName { get; protected set; }

    public string TableSchema { get; protected set; }

    public abstract TResult Go();

    public abstract Task<TResult> GoAsync();

    public ISqlStatement<TResult> UseConnectionProvider(IConnectionProvider connectionProvider)
    {
      StatementExecutor.UseConnectionProvider(connectionProvider);
      return this;
    }
  }
}
