// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.DeleteStatement`1
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Linq;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;
using EesyDapper.Core.CustomAttribute;

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
            if (entity != null && typeof(TEntity).GetProperties().Where(p => p.IsKeyField<TEntity>()).Count() == 0)
                throw new InvalidOperationException("以实例删除时，实例类必需至少有一个属性标记为[Key] 特性！");
            return string.Format("DELETE [{0}].[{1}]{2};", TableSchema, TableName, GetWhereClause());
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