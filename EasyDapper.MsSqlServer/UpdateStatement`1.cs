// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.UpdateStatement`1
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Linq;
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
        throw new InvalidOperationException("For cannot be used ParamSet have been used, please create a new command.");
      if (entity == null && !setSelectors.Any())
        throw new InvalidOperationException("Build cannot be used on a statement that has not been initialised using Set or For.");
      if (!typeof (TEntity).GetProperties().Any(p => p.IsKeyField() || p.IsIdField()))
        throw new InvalidOperationException("以实例更新时，实例类必需至少有一个属性标记为[KeyFiled] 特性！");
      return
          $"UPDATE [{(object) GetTableSchema()}].[{(object) GetTableName()}]\nSET {(object) GetSetClause("")}{(object) GetWhereClause("")};";
    }

      protected override string GetSetClauseFromEntity(string perParam)
      {
          return FormatColumnValuePairs(!string.IsNullOrWhiteSpace(perParam)
              ? typeof(TEntity).GetProperties().Where(p =>
              {
                  if (!p.IsIdField() && p.CanWrite)
                      return writablePropertyMatcher.TestIsDbField(p);
                  return false;
              }).Select(p => p.ColumnName() + "  = @" + p.Name)
              : typeof(TEntity).GetProperties().Where(p =>
              {
                  if (!p.IsIdField() && p.CanWrite)
                      return writablePropertyMatcher.TestIsDbField(p);
                  return false;
              }).Select(p => "[" + p.ColumnName() + "] = " + FormatValue(p.GetValue(entity))));
      }

      protected override string GetSetClauseFromSelectors(string perParam)
    {
      return FormatColumnValuePairs(!string.IsNullOrWhiteSpace(perParam) ? setSelectors.Select((e, i) => GetMemberColumnName(e) + "  = @" + GetMemberName(e)) : setSelectors.Select((e, i) => "[" + GetMemberColumnName(e) + "] = " + FormatValue(setValues[i])));
    }

    private string GetTableSchema()
    {
      return string.IsNullOrEmpty(TableSchema) ? "dbo" : TableSchema;
    }

      protected override string GetWhereClause(string perParam = "")
      {
          if (entity != null)
          {
              var columnValuePairs = !string.IsNullOrWhiteSpace(perParam)
                  ? typeof(TEntity).GetProperties().Where(p => p.IsKeyField() || p.IsIdField())
                      .Select(p => p.ColumnName() + "  = @" + p.Name)
                  : typeof(TEntity).GetProperties().Where(p => p.IsKeyField() || p.IsIdField()).Select(p =>
                      " [" + p.ColumnName() + "] = " + FormatValue(p.GetValue(entity)));
              return "\nWHERE " + FormatColumnValuePairs(columnValuePairs);
          }

          var str = whereClauseBuilder.Sql();
          return string.IsNullOrWhiteSpace(str) ? string.Empty : "\n" + str;
      }

      public override string ParamSql()
    {
      if (entity != null && !typeof (TEntity).GetProperties().Any(p => p.IsKeyField() || p.IsIdField()))
        throw new InvalidOperationException("以实例更新时，实例类必需至少有一个属性标记为[KeyFiled] 特性！");
      return
          $"UPDATE [{(object) GetTableSchema()}].[{(object) GetTableName()}]\nSET {(object) GetSetClause("@")}{(object) GetWhereClause("@")};";
    }
  }
}
