// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.OrderByClauseBuilder
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System.Collections.Generic;
using System.Linq;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer
{
    public class OrderByClauseBuilder : OrderByClauseBaseBuilder
    {
        private readonly IList<OrderBySpecification> orderBySpecifications = new List<OrderBySpecification>();

        public override IOrderByClauseBuilder FromScratch()
        {
            orderBySpecifications.Clear();
            IsClean = true;
            return this;
        }

        public override string Sql()
        {
            return orderBySpecifications.Any()
                ? string.Format("ORDER BY {0}", string.Join(", ", orderBySpecifications))
                : string.Empty;
        }

        protected override void AddOrderBySpecification<TEntity>(
            string alias,
            string tableName,
            string tableSchema,
            string name,
            OrderByDirection direction = OrderByDirection.Ascending)
        {
            if (string.IsNullOrWhiteSpace(alias))
                alias = ActiveAlias;
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = TableNameFromType<TEntity>();
            if (string.IsNullOrWhiteSpace(tableSchema))
                tableSchema = "dbo";
            var bySpecifications = orderBySpecifications;
            var orderBySpecification = new OrderBySpecification();
            orderBySpecification.Alias = alias;
            orderBySpecification.Table = tableName;
            orderBySpecification.Schema = tableSchema;
            orderBySpecification.Name = name;
            orderBySpecification.Direction = direction;
            bySpecifications.Add(orderBySpecification);
        }
    }
}