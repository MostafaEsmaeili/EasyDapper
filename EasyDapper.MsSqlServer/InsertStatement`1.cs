// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.InsertStatement`1
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Collections.Generic;
using System.Linq;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer
{
    public class InsertStatement<TEntity> : InsertStatementBase<TEntity> where TEntity : class, new()
    {
        private const string StatementTemplateAutoInc =
            "INSERT [{0}].[{1}]({2})\nVALUES({3}){4};\nSELECT {5}\nFROM [{0}].[{1}]\nWHERE [{6}] = SCOPE_IDENTITY();";

        private const string StatementTemplate = "INSERT [{0}].[{1}]({2})\nVALUES({3}){4};";

        public InsertStatement(
            IStatementExecutor statementExecutor,
            IEntityMapper entityMapper,
            IWritablePropertyMatcher writablePropertyMatcher)
            : base(statementExecutor, entityMapper, writablePropertyMatcher)
        {
        }

        public override string Sql()
        {
            if (paramWithMode)
                throw new InvalidOperationException(
                    "For cannot be used ParamWith have been used, please create a new command.");
            if (entity == null && !selectors.Any())
                throw new InvalidOperationException(
                    "Sql cannot be used on a command that has not been initialised using With or For.");
            if (IsAutoIncrement)
                return string.Format(
                    "INSERT [{0}].[{1}]({2})\nVALUES({3}){4};\nSELECT {5}\nFROM [{0}].[{1}]\nWHERE [{6}] = SCOPE_IDENTITY();",
                    (object) TableSchema, (object) TableName, (object) GetColumnsList(""), (object) GetValuesList(),
                    (object) string.Empty, (object) GetColumnsListBack(), (object) IdentityFiled);
            return string.Format("INSERT [{0}].[{1}]({2})\nVALUES({3}){4};", (object) TableSchema, (object) TableName,
                (object) GetColumnsList(""), (object) GetValuesList(), (object) string.Empty);
        }

        public override string ParamSql(bool IsOne = true)
        {
            CheckIdentityFiled();
            if (IsAutoIncrement & IsOne)
                return string.Format(
                    "INSERT [{0}].[{1}]({2})\nVALUES({3}){4};\nSELECT {5}\nFROM [{0}].[{1}]\nWHERE [{6}] = SCOPE_IDENTITY();",
                    (object) TableSchema, (object) TableName, (object) GetColumnsList(""),
                    (object) GetColumnsParamList("@"), (object) string.Empty, (object) GetColumnsListBack(),
                    (object) IdentityFiled);
            return string.Format("INSERT [{0}].[{1}]({2})\nVALUES({3}){4};", (object) TableSchema, (object) TableName,
                (object) GetColumnsList(""), (object) GetColumnsParamList("@"), (object) string.Empty);
        }

        public IInsertStatement<TEntity> UsingTableSchema(string tableSchema)
        {
            TableSchema = tableSchema;
            return this;
        }

        protected override string FormatColumnNames(IEnumerable<string> names, string perParam)
        {
            if (string.IsNullOrWhiteSpace(perParam))
                return string.Join(",", names.Select(n => "[" + n + "]"));
            return string.Join(",", names.Select(n => perParam + n));
        }

        protected override string FormatColumnNames(IDictionary<string, string> names, string perParam = "")
        {
            if (string.IsNullOrWhiteSpace(perParam))
                return string.Join(",", names.Select(n =>
                {
                    if (!(n.Key == n.Value))
                        return "[" + n.Value + "] as " + n.Key;
                    return "[" + n.Key + "]";
                }));
            return string.Join(",", names.Select(n => perParam + n.Key));
        }
    }
}