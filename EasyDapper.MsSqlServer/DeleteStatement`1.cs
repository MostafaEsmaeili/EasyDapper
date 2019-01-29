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
            return $"DELETE [{TableSchema}].[{TableName}]{GetWhereClause()};";
        }

        private string GetWhereClause()
        {
            if (entity != null)
            {
                var strings = typeof(TEntity).GetProperties().Where(p => p.IsKeyField())
                    .Select(p =>
                        " [" + p.ColumnName() + "] = " + FormatValue(p.GetValue(entity)));
                var columnValuePairs = strings.ToList();
                if (columnValuePairs.Any())
                    return " WHERE " + FormatColumnValuePairs(columnValuePairs);
            }

            var str = whereClauseBuilder.Sql();
            return string.IsNullOrWhiteSpace(str) ? string.Empty : "\n" + str;
        }
    }
}
