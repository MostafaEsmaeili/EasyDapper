// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.FromClauseBuilder
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Linq;
using System.Linq.Expressions;
using Atk.AtkExpression;
using EasyDapper.Core;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.MsSqlServer
{
    public class FromClauseBuilder : FromClauseBaseBuilder
    {
        protected override void AddTableSpecification<TEntity>(
            string specificationType,
            string rightTableName,
            string rightTableSchema,
            string rightTableAlias,
            Type leftTableType = null,
            string leftTableAlias = null)
        {
            if (string.IsNullOrWhiteSpace(rightTableName))
                rightTableName = CustomAttributeHandle.DbTableName<TEntity>();
            if (string.IsNullOrWhiteSpace(rightTableSchema))
                rightTableSchema = "dbo";
            var tableSpecification = new TableSpecification();
            tableSpecification.SpecificationType = specificationType;
            tableSpecification.RightSchema = rightTableSchema;
            tableSpecification.RightTable = rightTableName;
            tableSpecification.RightAlias = rightTableAlias;
            tableSpecification.RightType = typeof(TEntity);
            currentTableSpecification = tableSpecification;
            if (leftTableType != null)
            {
                var specificationBase = tableSpecifications.FirstOrDefault(s =>
                {
                    if (!(s.RightType == leftTableType))
                        return false;
                    if (!string.IsNullOrWhiteSpace(s.RightAlias))
                        return s.RightAlias == leftTableAlias;
                    return true;
                });
                if (specificationBase != null)
                {
                    currentTableSpecification.LeftSchema = specificationBase.RightSchema;
                    currentTableSpecification.LeftTable = specificationBase.RightTable;
                    currentTableSpecification.LeftAlias = specificationBase.RightAlias;
                    currentTableSpecification.LeftType = specificationBase.RightType;
                }
            }

            tableSpecifications.Add(currentTableSpecification);
        }

        protected override JoinConditionBase GetCondition<TLeft, TRight>(
            LogicalOperator logicalOperator,
            Expression<Func<TLeft, TRight, bool>> expression)
        {
            JoinCondition joinCondition;
            if (!(expression.Body is BinaryExpression body))
            {
                joinCondition = null;
            }
            else
            {
                joinCondition = new JoinCondition();
                joinCondition.LogicalOperator = logicalOperator;
                joinCondition.LeftTableAlias = currentTableSpecification.LeftAlias;
                joinCondition.LeftTableSchema = currentTableSpecification.LeftSchema;
                joinCondition.LeftTableName = currentTableSpecification.LeftTable;
                joinCondition.LeftIdentifier = GetMemberName(body.Left);
                joinCondition.RightTableAlias = currentTableSpecification.RightAlias;
                joinCondition.RightTableSchema = currentTableSpecification.RightSchema;
                joinCondition.RightTableName = currentTableSpecification.RightTable;
                joinCondition.RightIdentifier = GetMemberName(AtkPartialEvaluator.Eval(body.Right));
                joinCondition.Operator = OperatorString(body.NodeType);
            }

            return joinCondition;
        }
    }
}