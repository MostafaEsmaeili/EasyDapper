using System;
using System.Linq;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.MsSqlServer
{
    public class DeleteStatement<TEntity> : DeleteStatementBase<TEntity> where TEntity : class, new()
    {
        private const string StatementTemplate = "DELETE [{0}].[{1}]{2};";

        public DeleteStatement(
            IStatementExecutor statementExecutor,
            IEntityMapper entityMapper,
            IWhereClauseBuilder whereClauseBuilder,
            IWritablePropertyMatcher writablePropertyMatcher)
            : base(statementExecutor, entityMapper, whereClauseBuilder, writablePropertyMatcher)
        {
        }

        public override string Sql()
        {
            if (entity != null && !typeof(TEntity).GetProperties().Any(p => p.IsKeyField<TEntity>()))
                throw new InvalidOperationException("以实例删除时，实例类必需至少有一个属性标记为[Key] 特性！");
            return $"DELETE [{TableSchema}].[{TableName}]{GetWhereClause()};";
        }

        private string GetWhereClause()
        {
            if (entity != null)
            {
                var strings = typeof(TEntity).GetProperties().Where(p => p.IsKeyField<TEntity>()).Select(p =>
                    " ([" + p.ColumnName() + "] = " + FormatValue(p.GetValue(entity)) + ")");
                if (strings.Count() > 0)
                    return " WHERE " + FormatWhereValuePairs(strings);
            }

            var str = whereClauseBuilder.Sql();
            return string.IsNullOrWhiteSpace(str) ? string.Empty : "\n" + str;
        }
    }
}