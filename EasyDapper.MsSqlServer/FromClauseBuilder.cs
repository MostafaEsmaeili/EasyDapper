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
            var tableSpecification = new TableSpecification
            {
                SpecificationType = specificationType,
                RightSchema = rightTableSchema,
                RightTable = rightTableName,
                RightAlias = rightTableAlias,
                RightType = typeof(TEntity)
            };
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
                joinCondition = new JoinCondition
                {
                    LogicalOperator = logicalOperator,
                    LeftTableAlias = currentTableSpecification.LeftAlias,
                    LeftTableSchema = currentTableSpecification.LeftSchema,
                    LeftTableName = currentTableSpecification.LeftTable,
                    LeftIdentifier = GetMemberName(body.Left),
                    RightTableAlias = currentTableSpecification.RightAlias,
                    RightTableSchema = currentTableSpecification.RightSchema,
                    RightTableName = currentTableSpecification.RightTable,
                    RightIdentifier = GetMemberName(AtkPartialEvaluator.Eval(body.Right)),
                    Operator = OperatorString(body.NodeType)
                };
            }

            return joinCondition;
        }
    }
}
