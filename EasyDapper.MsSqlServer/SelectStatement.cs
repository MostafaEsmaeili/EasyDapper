using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Atk.AtkExpression;
using EasyDapper.Abstractions;
using EasyDapper.Core;
using EasyDapper.Core.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.MsSqlServer
{
    public class SelectStatement<TEntity> : SelectStatementBase<TEntity> where TEntity : class, new()
    {
        public SelectStatement(
            IStatementExecutor statementExecutor,
            IEntityMapper entityMapper,
            IWritablePropertyMatcher writablePropertyMatcher)
            : base(statementExecutor, entityMapper, writablePropertyMatcher)
        {
           InitialiseConfig();
        }

        public override ISelectStatement<TEntity> CountAll()
        {
            var columns = Specification.Columns;
            var columnSpecification1 = new ColumnSpecification
            {
                Identifier = "*", Aggregation = Aggregation.Count, EntityType = typeof(TEntity)
            };
            var columnSpecification2 = columnSpecification1;
            columns.Add(columnSpecification2);
            return this;
        }

        public override string UnionSql(List<UnionSql> Sqls)
        {
            string oldValue;
            if (!string.IsNullOrWhiteSpace(TableSchema))
                oldValue = " [" + TableSchema + "].[" + TableName + "]";
            else
                oldValue = "[" + TableName + "]";
            var input = GetNoSemicolonSql(Sql())
                .Replace(Specification.Tables.FirstOrDefault()?.ToString(),
                    "\nFROM ( _replace_union_query )\nAS  _this_is_union").Replace(oldValue, " [_this_is_union]");
            var replacement = string.Empty;
            foreach (var sql in Sqls)
            {
                var noSemicolonSql = GetNoSemicolonSql(sql.SqlClause.Sql());
                if (string.IsNullOrWhiteSpace(replacement))
                    replacement = noSemicolonSql;
                else
                    switch (sql.UnionType)
                    {
                        case UnionType.Union:
                            replacement = replacement + " \nUNION\n " + noSemicolonSql;
                            break;
                        case UnionType.UnionAll:
                            replacement = replacement + " \nUNION ALL\n " + noSemicolonSql;
                            break;
                        case UnionType.UnionDistinct:
                            replacement = replacement + " \nUNION DISTINCT\n " + noSemicolonSql;
                            break;
                    }
            }

            return Regex.Replace(input, "_replace_union_query", replacement);
        }

        public override ISelectStatement<TEntity> HavingCountAll<T>(
            Comparison comparison,
            int value)
        {
            ThrowIfGroupingNotInitialised();
            var havings = Specification.Havings;
            var havingSpecification1 = new SelectStatementHavingSpecification();
            havingSpecification1.Aggregation = Aggregation.Count;
            havingSpecification1.EntityType = typeof(T);
            havingSpecification1.Identifier = "*";
            havingSpecification1.Operator = OperatorStringFromComparison(comparison);
            havingSpecification1.Value = FormatValue(value);
            var havingSpecification2 = havingSpecification1;
            havings.Add(havingSpecification2);
            return this;
        }

        public override string Sql()
        {
            
            FinalizeColumnSpecifications();
            FinalizeJoinConditions();
            FinalizeWhereConditions(Specification.Filters);
            FinalizeGroupings();
            FinalizeOrderings();
            FinalizeHavings();
            return Specification.ToString();
        }
        public override (string, List<PartialSelectSelectParameterDefinition>) SqlWithParams()
        {
            
            FinalizeColumnSpecifications();
            FinalizeJoinConditions();
            FinalizeWhereConditions(Specification.Filters);
            FinalizeGroupings();
            FinalizeOrderings();
            FinalizeHavings();
            var sql= Specification.ToString();
            var parameters = new List<PartialSelectSelectParameterDefinition>();
            foreach (var f in Specification.Filters)
            {
                foreach (var condition in f.Conditions)
                {
                    if (condition.ParameterDefinitions.Any())
                    {
                        parameters.AddRange(condition.ParameterDefinitions);
                    }
                }
            }
            return (sql, parameters);
        }
        protected override void AddBetweenFilterCondition<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null,
            LogicalOperator logicalOperator = LogicalOperator.NotSet)
        {
            var conditions1 = currentFilterGroup.Conditions;
            var filterCondition1 = new FilterCondition
            {
                Alias = alias,
                EntityType = typeof(T),
                Operator = ">=",
                Left = GetMemberName(selector),
                LocigalOperator = logicalOperator,
                Right = FormatValue(start)
            };
            var filterCondition2 = filterCondition1;
            conditions1.Add(filterCondition2);
            var conditions2 = currentFilterGroup.Conditions;
            var filterCondition3 = new FilterCondition
            {
                Alias = alias,
                EntityType = typeof(T),
                Operator = "<=",
                Left = GetMemberName(selector),
                LocigalOperator = LogicalOperator.And,
                Right = FormatValue(end)
            };
            var filterCondition4 = filterCondition3;
            conditions2.Add(filterCondition4);
        }

        protected override void AddColumnSelection<T>(
            string name,
            string showname = "",
            string alias = null,
            Aggregation aggregation = Aggregation.None)
        {
            var columns = Specification.Columns;
            var columnSpecification1 = new ColumnSpecification
            {
                Aggregation = aggregation,
                Alias = alias,
                EntityType = typeof(T),
                Identifier = name,
                ColumnName = GetColumnAlias<TEntity>(name),
                AggregationColumnName = showname == "" ? name : showname
            };
            var columnSpecification2 = columnSpecification1;
            columns.Add(columnSpecification2);
        }

        protected override void AddJoinsColumnSelection<T>(string name, string alias = null)
        {
            var joinsColumns = Specification.JoinsColumns;
            var columnSpecification1 = new ColumnSpecification();
            columnSpecification1.Aggregation = Aggregation.None;
            columnSpecification1.Alias = alias;
            columnSpecification1.EntityType = typeof(T);
            columnSpecification1.Identifier = name;
            columnSpecification1.ColumnName = GetColumnAlias<TEntity>(name);
            var columnSpecification2 = columnSpecification1;
            joinsColumns.Add(columnSpecification2);
        }

        protected override void AddFilterCondition<T>(
            Expression<Func<T, bool>> selector,
            string alias = null,
            LogicalOperator logicalOperator = LogicalOperator.NotSet)
        {


            var conditions = currentFilterGroup.Conditions;
            var lambdaTree =
                AtkExpressionWriterSql<T>.AtkWhereWriteToStringWithParameters(selector, AtkExpSqlType.AtkWhere, "[",
                    "]");
            var filterCondition1 = new FilterCondition
            {
                Alias = alias,
                EntityType = typeof(T),
                Left = "_LambdaTree_",
                LocigalOperator = logicalOperator,
                LambdaTree = lambdaTree?.Sql,
                ParameterDefinitions = lambdaTree?.Parameters,
                ExpressionOperator = lambdaTree?.Operator
            };

            conditions.Add(filterCondition1);
        }

        protected override void AddGroupSpecification<T>(
            Expression<Func<T, object>> selector,
            string alias = null,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            var type = typeof(T);
            var groupings1 = Specification.Groupings;
            var groupSpecification1 = new GroupSpecification();
            groupSpecification1.Alias = alias;
            groupSpecification1.EntityType = type;
            groupSpecification1.Identifer = GetMemberColumnName(selector);
            groupings1.Add(groupSpecification1);
            foreach (var additionalSelector in additionalSelectors)
            {
                var groupings2 = Specification.Groupings;
                var groupSpecification2 = new GroupSpecification();
                groupSpecification2.Alias = alias;
                groupSpecification2.EntityType = type;
                groupSpecification2.Identifer = GetMemberColumnName(additionalSelector);
                groupings2.Add(groupSpecification2);
            }
        }

        protected override void AddHavingSpecification<T>(
            Expression<Func<T, bool>> selector,
            Aggregation aggregation,
            string alias = null)
        {
            var body = selector.Body as BinaryExpression;
            var havings = Specification.Havings;
            var havingSpecification1 = new SelectStatementHavingSpecification();
            havingSpecification1.Aggregation = aggregation;
            havingSpecification1.Alias = alias;
            havingSpecification1.EntityType = typeof(T);
            havingSpecification1.Identifier = GetMemberName(body.Left);
            havingSpecification1.Operator = OperatorString(body.NodeType);
            havingSpecification1.Value = FormatValue(GetExpressionValue(selector));
            var havingSpecification2 = havingSpecification1;
            havings.Add(havingSpecification2);
        }

        protected override void AddInFilterCondition<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember[] values,
            string alias = null,
            LogicalOperator locigalOperator = LogicalOperator.NotSet)
        {
            if (values == null || !values.Any())
                return;
            var conditions = currentFilterGroup.Conditions;
            var filterCondition1 = new FilterCondition();
            filterCondition1.Alias = alias;
            filterCondition1.EntityType = typeof(T);
            filterCondition1.LocigalOperator = locigalOperator;
            filterCondition1.Left = GetMemberColumnName(ConvertExpression(selector));
            filterCondition1.Operator = "IN";
            filterCondition1.Right = "(" + string.Join(", ", values.Select(v => FormatValue(v))) + ")";
            var filterCondition2 = filterCondition1;
            conditions.Add(filterCondition2);
        }

        protected override void AddJoinCondition<TLeft, TRight>(
            Expression<Func<TLeft, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null,
            LogicalOperator locLogicalOperator = LogicalOperator.NotSet)
        {
            var body = expression.Body as BinaryExpression;
            var joins = Specification.Joins;
            var joinSpecification1 = new JoinSpecification
            {
                LeftEntityType = typeof(TLeft),
                LeftIdentifier = GetMemberName(body.Left).ColumnName<TRight>(),
                LeftTableAlias = leftTableAlias,
                Operator = OperatorString(body.NodeType),
                RightEntityType = typeof(TRight),
                RightIdentifier = GetMemberName(AtkPartialEvaluator.Eval(body.Right)).ColumnName<TRight>(),
                RightTableAlias = rightTableAlias,
                LogicalOperator = locLogicalOperator
            };
            var joinSpecification2 = joinSpecification1;
            joins.Add(joinSpecification2);
        }

        protected override void AddNewFilterGroup(FilterGroupType filterGroupType)
        {
            var filterGroup1 = new FilterGroup {GroupType = filterGroupType, Parent = currentFilterGroup};
            var filterGroup2 = filterGroup1;
            currentFilterGroup.Groups.Add(filterGroup2);
            currentFilterGroup = filterGroup2;
        }

        protected override void AddOrderSpecification<T>(
            Expression<Func<T, object>> selector,
            string alias = null,
            OrderByDirection orderByDirection = OrderByDirection.Ascending,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            var type = typeof(T);
            var orderings1 = Specification.Orderings;
            var orderSpecification1 = new OrderSpecification
            {
                Alias = alias, Direction = orderByDirection, EntityType = type, Identifer = GetMemberName(selector)
            };
            orderings1.Add(orderSpecification1);
            foreach (var additionalSelector in additionalSelectors)
            {
                var orderings2 = Specification.Orderings;
                var orderSpecification2 = new OrderSpecification
                {
                    Alias = alias,
                    Direction = orderByDirection,
                    EntityType = type,
                    Identifer = GetMemberName(additionalSelector)
                };
                orderings2.Add(orderSpecification2);
            }
        }

        protected override void AddTableSpecification<T>(
            JoinType joinType,
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            ThrowIfTableAlreadyJoined<T>(alias, tableName, tableSchema);
            var tables = Specification.Tables;
            var tableSpecification1 = new SelectStatementTableSpecification
            {
                Alias = alias,
                EntityType = typeof(T),
                JoinType = joinType,
                TableName = string.IsNullOrEmpty(tableName) ? CustomAttributeHandle.DbTableName<T>() : tableName,
                Schema = string.IsNullOrEmpty(tableSchema)
                    ? CustomAttributeHandle.DbTableSchema<T>()
                    : tableSchema
            };
            var tableSpecification2 = tableSpecification1;
            tables.Add(tableSpecification2);
        }

        protected sealed override void InitialiseConfig()
        {
            Specification = new SelectStatementSpecification();
            var type = typeof(TEntity);
            var tables = Specification.Tables;
            var tableSpecification = new SelectStatementTableSpecification
            {
                EntityType = type, Schema = "dbo", TableName = CustomAttributeHandle.DbTableName<TEntity>()
            };
            tables.Add(tableSpecification);
        }

        protected override void InitialiseFiltering()
        {
            rootFilterGroup = new FilterGroup();
            Specification.Filters.Add(rootFilterGroup);
            currentFilterGroup = rootFilterGroup;
        }

        public override int GetPageCount()
        {
            FinalizeColumnSpecifications();
            FinalizeJoinConditions();
            FinalizeWhereConditions(Specification.Filters);
            FinalizeGroupings();
            FinalizeOrderings();
            FinalizeHavings();
            using (var dataReader = StatementExecutor.ExecuteReader(Specification.GetCountSqlString()))
            {
                var num = 0;
                if (dataReader.Read())
                    num = dataReader.GetInt32(0);
                return num;
            }
        }

        public override ValueTuple<IEnumerable<TEntity>, int> PageGo()
        {
            return new ValueTuple<IEnumerable<TEntity>, int>(Go(), GetPageCount());
        }
    }
}