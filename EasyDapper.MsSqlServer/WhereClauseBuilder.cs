// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.WhereClauseBuilder
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Linq;
using System.Linq.Expressions;
using Atk.AtkExpression;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer
{
    public class WhereClauseBuilder : WhereClauseBaseBuilder
    {
        protected override void AddBetweenConditionToCurrentGroup<TEntity, TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember start,
            TMember end,
            LogicalOperator locigalOperator,
            string alias,
            string tableName,
            string tableSchema)
        {
            var conditions1 = currentGroup.Conditions;
            var whereClauseCondition1 = new WhereClauseCondition();
            whereClauseCondition1.Alias = alias;
            whereClauseCondition1.LocigalOperator = locigalOperator;
            whereClauseCondition1.LeftTable =
                string.IsNullOrWhiteSpace(tableName) ? TableNameFromType<TEntity>() : tableName;
            whereClauseCondition1.LeftSchema = string.IsNullOrWhiteSpace(tableSchema) ? "dbo" : tableSchema;
            whereClauseCondition1.Left = GetMemberColumnName(ConvertExpression(selector));
            whereClauseCondition1.Operator = ">=";
            whereClauseCondition1.Right = FormatValue(start);
            conditions1.Add(whereClauseCondition1);
            var conditions2 = currentGroup.Conditions;
            var whereClauseCondition2 = new WhereClauseCondition();
            whereClauseCondition2.Alias = alias;
            whereClauseCondition2.LocigalOperator = LogicalOperator.And;
            whereClauseCondition2.LeftTable =
                string.IsNullOrWhiteSpace(tableName) ? TableNameFromType<TEntity>() : tableName;
            whereClauseCondition2.LeftSchema = string.IsNullOrWhiteSpace(tableSchema) ? "dbo" : tableSchema;
            whereClauseCondition2.Left = GetMemberColumnName(ConvertExpression(selector));
            whereClauseCondition2.Operator = "<=";
            whereClauseCondition2.Right = FormatValue(end);
            conditions2.Add(whereClauseCondition2);
            IsClean = false;
        }

        protected override IWhereClauseBuilder AddConditionToCurrentGroup<TEntity>(
            Expression<Func<TEntity, bool>> expression,
            LogicalOperator locigalOperator,
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            ThrowIfNotInitialised();
            IsClean = false;
            var conditions = currentGroup.Conditions;
            var whereClauseCondition = new WhereClauseCondition();
            whereClauseCondition.Alias = alias;
            whereClauseCondition.LocigalOperator = locigalOperator;
            whereClauseCondition.LeftTable =
                string.IsNullOrWhiteSpace(tableName) ? TableNameFromType<TEntity>() : tableName;
            whereClauseCondition.LeftSchema = string.IsNullOrWhiteSpace(tableSchema) ? "dbo" : tableSchema;
            whereClauseCondition.Left = "_LambdaTree_";
            whereClauseCondition.Right =
                AtkExpressionWriterSql<TEntity>.AtkWhereWriteToString(expression, AtkExpSqlType.atkWhere, "[", "]");
            conditions.Add(whereClauseCondition);
            return this;
        }

        protected override void AddInConditionToCurrentGroup<TEntity, TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember[] values,
            LogicalOperator locigalOperator,
            string alias,
            string tableName,
            string tableSchema)
        {
            if (values == null || !values.Any())
                return;
            var conditions = currentGroup.Conditions;
            var whereClauseCondition = new WhereClauseCondition();
            whereClauseCondition.Alias = alias;
            whereClauseCondition.LocigalOperator = locigalOperator;
            whereClauseCondition.LeftTable =
                string.IsNullOrWhiteSpace(tableName) ? TableNameFromType<TEntity>() : tableName;
            whereClauseCondition.LeftSchema = string.IsNullOrWhiteSpace(tableSchema) ? "dbo" : tableSchema;
            whereClauseCondition.Left = GetMemberColumnName(ConvertExpression(selector));
            whereClauseCondition.Operator = "IN";
            whereClauseCondition.Right = "(" + string.Join(", ", values.Select(v => FormatValue(v))) + ")";
            conditions.Add(whereClauseCondition);
            IsClean = false;
        }

        protected override IWhereClauseBuilder AddNestedGroupToCurrentGroup<TEntity>(
            Expression<Func<TEntity, bool>> expression,
            WhereClauseGroupType groupType,
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            ThrowIfNotInitialised();
            var whereClauseGroup1 = new WhereClauseGroup();
            whereClauseGroup1.GroupType = groupType;
            whereClauseGroup1.Parent = currentGroup;
            var whereClauseGroup2 = whereClauseGroup1;
            currentGroup.Groups.Add(whereClauseGroup2);
            currentGroup = whereClauseGroup2;
            return AddConditionToCurrentGroup(expression, LogicalOperator.NotSet, alias, tableName, tableSchema);
        }

        protected override void Initialise()
        {
            var whereClauseGroup = new WhereClauseGroup();
            whereClauseGroup.GroupType = WhereClauseGroupType.Where;
            rootGroup = whereClauseGroup;
            currentGroup = rootGroup;
        }
    }
}