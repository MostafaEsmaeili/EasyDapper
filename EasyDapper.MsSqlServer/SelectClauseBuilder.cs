﻿// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.SelectClauseBuilder
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System.Linq;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer
{
    public class SelectClauseBuilder : SelectClauseBaseBuilder
    {
        public override ISelectClauseBuilder CountAll()
        {
            var selections = this.selections;
            var columnSelection = new ColumnSelection();
            columnSelection.Name = "*";
            columnSelection.Aggregation = Aggregation.Count;
            selections.Add(columnSelection);
            return this;
        }

        public override string Sql()
        {
            var str = "*";
            if (selections.Any())
                str = string.Join(", ", selections);
            return string.Format("SELECT {0}{1}",
                topRows.HasValue ? string.Format("TOP {0} ", topRows.Value) : string.Empty, str);
        }

        protected override void AddColumnSelection<TEntity>(
            string alias,
            string tableName,
            string tableSchema,
            string name,
            Aggregation aggregation = Aggregation.None)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = TableNameFromType<TEntity>();
            if (string.IsNullOrWhiteSpace(tableSchema))
                tableSchema = "dbo";
            var selections = this.selections;
            var columnSelection = new ColumnSelection();
            columnSelection.Alias = alias;
            columnSelection.Table = tableName;
            columnSelection.Schema = tableSchema;
            columnSelection.Name = name;
            columnSelection.Aggregation = aggregation;
            selections.Add(columnSelection);
        }
    }
}