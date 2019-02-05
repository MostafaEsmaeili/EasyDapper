using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.MsSqlServer
{
    public class UpdateStatement<TEntity> : UpdateStatementBase<TEntity> where TEntity : class, new()
    {
        private const string StatementTemplate = "UPDATE [{0}].[{1}]\nSET {2}{3};";

        public UpdateStatement(
            IStatementExecutor statementExecutor,
            IEntityMapper entityMapper,
            IWritablePropertyMatcher writablePropertyMatcher,
            IWhereClauseBuilder whereClauseBuilder)
            : base(statementExecutor, entityMapper, writablePropertyMatcher, whereClauseBuilder)
        {
        }

        public override string Sql()
        {
            if (paramSetMode)
                throw new InvalidOperationException(
                    "For cannot be used ParamSet have been used, please create a new command.");
            if (entity == null && !setSelectors.Any())
                throw new InvalidOperationException("不能在未使用Set或For初始化的语句上使用。");
            if (entity != null && !typeof(TEntity).GetProperties().Any(p => p.IsKeyField<TEntity>()))
                throw new InvalidOperationException("以实例更新时，实例类必需至少有一个属性标记为[Key] 特性！");
            return
                $"UPDATE [{ GetTableSchema()}].[{ GetTableName()}]\nSET { GetSetClause("")}{ GetWhereClause("")};";
        }

        protected override string GetSetClauseFromEntity(string perParam)
        {
            IEnumerable<string> columnValuePairs;
            if (!string.IsNullOrWhiteSpace(perParam))
                columnValuePairs = typeof(TEntity).GetProperties().Where(p =>
                {
                    if (!p.IsIdField() && p.CanWrite)
                        return writablePropertyMatcher.TestIsDbField(p);
                    return false;
                }).Select(p => p.ColumnName() + "  = @" + p.Name);
            else
            {
                var columnValuePairs2 = typeof(TEntity).GetProperties().Where(p =>
                {
                    if (!p.IsIdField() && p.CanWrite)
                        return writablePropertyMatcher.TestIsDbField(p);
                    return false;
                });
                columnValuePairs=columnValuePairs2.Select(p => "[" + p.ColumnName() + "] = " + FormatValue(p.GetValue(entity)) ?? "");
            }


            return FormatColumnValuePairs(columnValuePairs);
        }

        protected override string GetSetClauseFromSelectors(string perParam)
        {
            return FormatColumnValuePairs(!string.IsNullOrWhiteSpace(perParam)
                ? setSelectors.Select((e, i) => GetMemberColumnName(e) + "  = @" + GetMemberName(e))
                : setSelectors.Select((e, i) =>
                    "[" + this.GetMemberColumnName(e) + "] = " + FormatValue(setValues[i])));
        }

        private string GetTableSchema()
        {
            return string.IsNullOrEmpty(TableSchema) ? "dbo" : TableSchema;
        }

        protected override string GetWhereClause(string perParam)
        {
            if (entity != null)
            {
                var columnValuePairs = !string.IsNullOrWhiteSpace(perParam)
                    ? typeof(TEntity).GetProperties().Where(p => p.IsKeyField<TEntity>())
                        .Select(p => "(" + p.ColumnName() + "  = @" + p.Name + ")")
                    : typeof(TEntity).GetProperties().Where(p => p.IsKeyField<TEntity>()).Select(p =>
                        " ([" + p.ColumnName() + "] = " + FormatValue(p.GetValue(entity)) + ")");
                return "\nWHERE " + FormatWhereValuePairs(columnValuePairs);
            }

            var str = whereClauseBuilder.Sql();
            return string.IsNullOrWhiteSpace(str) ? string.Empty : "\n" + str;
        }

        public override string ParamSql()
        {
            if (entity != null && !typeof(TEntity).GetProperties().Any(p => p.IsKeyField<TEntity>() || p.IsIdField()))
                throw new InvalidOperationException("以实例更新时，实例类必需至少有一个属性标记为[KeyFiled] 特性！");
            return
                $"UPDATE [{(object) GetTableSchema()}].[{(object) GetTableName()}]\nSET {(object) GetSetClause("@")}{(object) GetWhereClause("@")};";
        }
    }
}