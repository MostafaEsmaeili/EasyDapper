using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.Core
{
    public abstract class UpdateStatementBase<TEntity> : SqlStatement<TEntity, int>, IUpdateStatement<TEntity>,
        ISqlStatement<int>, IClauseBuilder
        where TEntity : class, new()
    {
        protected readonly Dictionary<string, object> selectorswithValue = new Dictionary<string, object>();

        protected readonly IList<Expression<Func<TEntity, object>>> setSelectors =
            new List<Expression<Func<TEntity, object>>>();

        protected readonly IList<object> setValues = new List<object>();
        protected readonly IWhereClauseBuilder whereClauseBuilder;
        protected readonly IWritablePropertyMatcher writablePropertyMatcher;
        protected TEntity entity;
        protected bool paramSetMode;
        protected bool tableNameChange;
        protected bool whereAllreadyAdd;

        public UpdateStatementBase(
            IStatementExecutor statementExecutor,
            IEntityMapper entityMapper,
            IWritablePropertyMatcher writablePropertyMatcher,
            IWhereClauseBuilder whereClauseBuilder)
            : base(statementExecutor, entityMapper, writablePropertyMatcher)
        {
            this.writablePropertyMatcher = writablePropertyMatcher;
            this.whereClauseBuilder = whereClauseBuilder;
        }

        public IUpdateStatement<TEntity> And(Expression<Func<TEntity, bool>> expression)
        {
            whereClauseBuilder.And(expression, null, GetTableName(), null);
            return this;
        }

        public IUpdateStatement<TEntity> For(TEntity entity)
        {
            if (setSelectors.Any() || !whereClauseBuilder.IsClean)
                throw new InvalidOperationException(
                    "For cannot be used once Set or Where have been used, please create a new command.");
            IsClean = false;
            this.entity = entity;
            return this;
        }

        public override int Go()
        {
            if (paramSetMode)
                throw new InvalidOperationException(
                    "For cannot be used ParamSet have been used, please create a new command.");
            return StatementExecutor.ExecuteNonQuery(Sql());
        }

        public override async Task<int> GoAsync()
        {
            var num = await StatementExecutor.ExecuteNonQueryAsync(Sql());
            return num;
        }

        public IUpdateStatement<TEntity> NestedAnd(
            Expression<Func<TEntity, bool>> expression)
        {
            whereClauseBuilder.NestedAnd(expression, null, null, null);
            return this;
        }

        public IUpdateStatement<TEntity> NestedOr(
            Expression<Func<TEntity, bool>> expression)
        {
            whereClauseBuilder.NestedOr(expression, null, null, null);
            return this;
        }

        public IUpdateStatement<TEntity> Or(Expression<Func<TEntity, bool>> expression)
        {
            whereClauseBuilder.Or(expression, null, GetTableName(), null);
            return this;
        }

        public IUpdateStatement<TEntity> Set<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember value,
            string tableSchema = null,
            string tableName = null)
        {
            if (entity != null)
                throw new InvalidOperationException(
                    "Set cannot be used once For has been used, please create a new command.");
            IsClean = false;
            var selector1 = ConvertExpression(selector);
            if (!CustomAttributeHandle.IsIdentityField<TEntity>(GetMemberName(selector1)))
            {
                setSelectors.Add(selector1);
                setValues.Add(value);
            }

            selectorswithValue.Add(GetMemberName(selector1), value);
            TableSchema = tableSchema;
            TableName = GetTableNameChange(tableName);
            return this;
        }

        public IUpdateStatement<TEntity> UsingTableName(string tableName)
        {
            TableName = tableName;
            tableNameChange = true;
            return this;
        }

        public IUpdateStatement<TEntity> Where(
            Expression<Func<TEntity, bool>> expression)
        {
            if (entity != null)
                throw new InvalidOperationException("实例模式更新时，不能指定 Where 语句。");
            IsClean = false;
            if (whereAllreadyAdd)
            {
                And(expression);
            }
            else
            {
                whereAllreadyAdd = true;
                whereClauseBuilder.Where(expression, null, GetTableName(), null);
            }

            return this;
        }

        public IUpdateStatement<TEntity> WhereIn<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember[] values)
        {
            if (entity != null)
                throw new InvalidOperationException(
                    "Where cannot be used once For has been used, please create a new command.");
            IsClean = false;
            whereClauseBuilder.WhereIn(selector, values, null, GetTableName(), null);
            return this;
        }

        public abstract string ParamSql();


        public ValueTuple<string, TEntity> ParamSqlWithEntity()
        {
            return new ValueTuple<string, TEntity>(ParamSql(), GetEntityFromwithValue());
        }

        public IUpdateStatement<TEntity> ParamSet(
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            SetParam(selector);
            foreach (var additionalSelector in additionalSelectors)
                SetParam(additionalSelector);
            return this;
        }

        protected string GetTableNameChange(string atkTableName = null)
        {
            if (tableNameChange)
                return TableName;
            return atkTableName;
        }

        protected TEntity GetEntityFromwithValue()
        {
            return setSelectors.Any()
                ? DataReaderEntityMapper.MapSelectorWithVales<TEntity>(selectorswithValue)
                : entity;
        }

        protected string FormatColumnValuePairs(IEnumerable<string> columnValuePairs)
        {
            return string.Join(" , ", columnValuePairs);
        }

        protected string FormatWhereValuePairs(IEnumerable<string> columnValuePairs)
        {
            return string.Join(" And ", columnValuePairs);
        }

        protected string GetSetClause(string perParam = "")
        {
            return setSelectors.Any() ? GetSetClauseFromSelectors(perParam) : GetSetClauseFromEntity(perParam);
        }

        protected abstract string GetSetClauseFromEntity(string perParam);

        protected abstract string GetSetClauseFromSelectors(string perParam);

        protected string GetTableName()
        {
            return string.IsNullOrEmpty(TableName) ? TableNameFromType<TEntity>() : TableName;
        }

        protected abstract string GetWhereClause(string perParam);

        protected IUpdateStatement<TEntity> SetParam(
            Expression<Func<TEntity, object>> selector)
        {
            paramSetMode = true;
            IsClean = false;
            var selector1 = ConvertExpression(selector);
            setSelectors.Add(selector1);
            selectorswithValue.Add(GetMemberName(selector1), null);
            return this;
        }
    }
}