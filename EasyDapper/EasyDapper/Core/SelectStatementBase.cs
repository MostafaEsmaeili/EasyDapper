using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper.Core
{
    public abstract class SelectStatementBase<TEntity> : SqlStatement<TEntity, IEnumerable<TEntity>>,
        ISelectStatement<TEntity>, ISqlStatement<IEnumerable<TEntity>>, IClauseBuilder
        where TEntity : class, new()
    {
        protected FilterGroupBase currentFilterGroup;
        protected FilterGroupBase rootFilterGroup;
        protected bool whereAllreadyAdd;

        protected SelectStatementBase(
            IStatementExecutor statementExecutor,
            IEntityMapper entityMapper,
            IWritablePropertyMatcher writablePropertyMatcher)
            : base(statementExecutor, entityMapper, writablePropertyMatcher)
        {
            //  InitialiseConfig();
        }

        public SelectStatementSpecificationBase Specification { get; protected set; }

        public IEnumerable<TEntity> Union(List<UnionSql> Sqls)
        {
            if (Sqls.Count() == 0)
                return Go();
            using (var reader = StatementExecutor.ExecuteReader(UnionSql(Sqls)))
            {
                return EntityMapper.Map<TEntity>(reader);
            }
        }
       
        public async Task<IEnumerable<TEntity>> UnionAsync(List<UnionSql> Sqls)
        {
            if (Sqls.Count() == 0)
                return Go();
            using (var reader = await StatementExecutor.ExecuteReaderAsync(UnionSql(Sqls)))
            {
                return EntityMapper.Map<TEntity>(reader);
            }
        }

        public abstract string UnionSql(List<UnionSql> Sqls);

        public ISelectStatement<TEntity> And<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            if (Specification.Filters.Count == 0)
                return Where(selector, alias, false);
            ThrowIfFilteringNotInitialised();
            AddFilterCondition(selector, alias, LogicalOperator.And);
            return this;
        }

        public ISelectStatement<TEntity> And(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return And<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> AndBetween<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            if (Specification.Filters.Count == 0)
                return WhereBetween(selector, start, end, alias);
            ThrowIfFilteringNotInitialised();
            AddBetweenFilterCondition(selector, start, end, alias, LogicalOperator.And);
            return this;
        }

        public ISelectStatement<TEntity> AndBetween<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            return AndBetween<TEntity, TMember>(selector, start, end, alias);
        }

        public ISelectStatement<TEntity> AndIn<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            if (Specification.Filters.Count == 0)
                return WhereIn(selector, values, alias);
            ThrowIfFilteringNotInitialised();
            AddInFilterCondition(selector, values, alias, LogicalOperator.And);
            return this;
        }

        public ISelectStatement<TEntity> AndIn<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            if (Specification.Filters.Count == 0)
                return WhereIn<TMember>(selector, values, alias);
            return AndIn<TEntity, TMember>(selector, values, alias);
        }

        public ISelectStatement<TEntity> AndOn<TRight>(
            Expression<Func<TEntity, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null)
        {
            return AndOn<TEntity, TRight>(expression, leftTableAlias, rightTableAlias);
        }

        public ISelectStatement<TEntity> AndOn<TLeft, TRight>(
            Expression<Func<TLeft, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null)
        {
            if (Specification.Tables.Count < 2)
                throw new InvalidOperationException(
                    "On cannot be used before initialising a join with one of the Join methods.");
            AddJoinCondition(expression, leftTableAlias, rightTableAlias, LogicalOperator.And);
            return this;
        }

        public ISelectStatement<TEntity> Avg(
            Expression<Func<TEntity, object>> selector,
            string alias = null)
        {
            return Avg<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> Avg(
            Expression<Func<TEntity, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            return Avg<TEntity>(selector, showselector, alias);
        }

        public ISelectStatement<TEntity> Avg<T>(
            Expression<Func<T, object>> selector,
            string alias = null)
        {
            var memberName = GetMemberName(selector);
            AddColumnSelection<T>(memberName, memberName, alias, Aggregation.Avg);
            return this;
        }

        public ISelectStatement<TEntity> Avg<T>(
            Expression<Func<T, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            AddColumnSelection<T>(GetMemberName(selector), GetMemberName(showselector), alias, Aggregation.Avg);
            return this;
        }

        public ISelectStatement<TEntity> Count(
            Expression<Func<TEntity, object>> selector,
            string alias = null)
        {
            return Count<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> Count(
            Expression<Func<TEntity, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            return Count<TEntity>(selector, showselector, alias);
        }

        public ISelectStatement<TEntity> Count<T>(
            Expression<Func<T, object>> selector,
            string alias = null)
        {
            var memberName = GetMemberName(selector);
            AddColumnSelection<T>(memberName, memberName, alias, Aggregation.Count);
            return this;
        }

        public ISelectStatement<TEntity> Count<T>(
            Expression<Func<T, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            AddColumnSelection<T>(GetMemberName(selector), GetMemberName(showselector), alias, Aggregation.Count);
            return this;
        }

        public abstract ISelectStatement<TEntity> CountAll();

        public ISelectStatement<TEntity> EndNesting()
        {
            if (currentFilterGroup.Parent == null)
                return this;
            currentFilterGroup = currentFilterGroup.Parent;
            return this;
        }

        public ISelectStatement<TEntity> From(
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            var specificationBase = Specification.Tables.First();
            if (!string.IsNullOrEmpty(alias))
                specificationBase.Alias = alias;
            if (!string.IsNullOrEmpty(tableName))
                specificationBase.TableName = tableName;
            if (!string.IsNullOrEmpty(tableSchema))
                specificationBase.Schema = tableSchema;
            return this;
        }

        public override IEnumerable<TEntity> Go()
        {
            using (var reader = StatementExecutor.ExecuteReader(Sql()))
            {
                return EntityMapper.Map<TEntity>(reader);
            }
        }

        public override async Task<IEnumerable<TEntity>> GoAsync()
        {
            IEnumerable<TEntity> entities;
            using (var reader = await StatementExecutor.ExecuteReaderAsync(Sql()))
            {
                entities = EntityMapper.Map<TEntity>(reader);
            }

            return entities;
        }

        public ISelectStatement<TEntity> GroupBy<T>(
            Expression<Func<T, object>> selector,
            string alias = null,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            ThrowIfTableNotSpecified<T>(alias);
            AddGroupSpecification(selector, alias);
            foreach (var additionalSelector in additionalSelectors)
                AddGroupSpecification(additionalSelector, alias);
            return this;
        }

        public ISelectStatement<TEntity> GroupBy(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return GroupBy<TEntity>(selector, alias, additionalSelectors);
        }

        public ISelectStatement<TEntity> HavingAvg<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            ThrowIfGroupingNotInitialised();
            AddHavingSpecification(selector, Aggregation.Avg, alias);
            return this;
        }

        public ISelectStatement<TEntity> HavingAvg(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return HavingAvg<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> HavingCount<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            ThrowIfGroupingNotInitialised();
            AddHavingSpecification(selector, Aggregation.Count, alias);
            return this;
        }

        public ISelectStatement<TEntity> HavingCount(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return HavingCount<TEntity>(selector, alias);
        }

        public abstract ISelectStatement<TEntity> HavingCountAll<T>(
            Comparison comparison,
            int value);

        public ISelectStatement<TEntity> HavingCountAll(
            Comparison comparison,
            int value)
        {
            return HavingCountAll<TEntity>(comparison, value);
        }

        public ISelectStatement<TEntity> HavingMax<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            ThrowIfGroupingNotInitialised();
            AddHavingSpecification(selector, Aggregation.Max, alias);
            return this;
        }

        public ISelectStatement<TEntity> HavingMax(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return HavingMax<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> HavingMin<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            ThrowIfGroupingNotInitialised();
            AddHavingSpecification(selector, Aggregation.Min, alias);
            return this;
        }

        public ISelectStatement<TEntity> HavingMin(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return HavingMin<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> HavingSum<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            ThrowIfGroupingNotInitialised();
            AddHavingSpecification(selector, Aggregation.Sum, alias);
            return this;
        }

        public ISelectStatement<TEntity> HavingSum(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return HavingSum<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> InnerJoin<TRight>(
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddTableSpecification<TRight>(JoinType.Inner, alias, tableName, tableSchema);
            return this;
        }

        public ISelectStatement<TEntity> LeftOuterJoin<TRight>(
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddTableSpecification<TRight>(JoinType.LeftOuter, alias, tableName, tableSchema);
            return this;
        }

        public ISelectStatement<TEntity> Max(
            Expression<Func<TEntity, object>> selector,
            string alias = null)
        {
            return Max<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> Max(
            Expression<Func<TEntity, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            return Max<TEntity>(selector, showselector, alias);
        }

        public ISelectStatement<TEntity> Max<T>(
            Expression<Func<T, object>> selector,
            string alias = null)
        {
            var memberName = GetMemberName(selector);
            AddColumnSelection<T>(memberName, memberName, alias, Aggregation.Max);
            return this;
        }

        public ISelectStatement<TEntity> Max<T>(
            Expression<Func<T, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            AddColumnSelection<T>(GetMemberName(selector), GetMemberName(showselector), alias, Aggregation.Max);
            return this;
        }

        public ISelectStatement<TEntity> Min(
            Expression<Func<TEntity, object>> selector,
            string alias = null)
        {
            return Min<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> Min(
            Expression<Func<TEntity, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            return Min<TEntity>(selector, showselector, alias);
        }

        public ISelectStatement<TEntity> Min<T>(
            Expression<Func<T, object>> selector,
            string alias = null)
        {
            var memberName = GetMemberName(selector);
            AddColumnSelection<T>(memberName, memberName, alias, Aggregation.Min);
            return this;
        }

        public ISelectStatement<TEntity> Min<T>(
            Expression<Func<T, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            AddColumnSelection<T>(GetMemberName(selector), GetMemberName(showselector), alias, Aggregation.Min);
            return this;
        }

        public ISelectStatement<TEntity> NestedAnd<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            ThrowIfFilteringNotInitialised();
            AddNewFilterGroup(FilterGroupType.And);
            AddFilterCondition(selector, alias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> NestedAnd(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return NestedAnd<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> NestedAndBetween<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            ThrowIfFilteringNotInitialised();
            AddNewFilterGroup(FilterGroupType.And);
            AddBetweenFilterCondition(selector, start, end, alias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> NestedAndBetween<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            return NestedAndBetween<TEntity, TMember>(selector, start, end, alias);
        }

        public ISelectStatement<TEntity> NestedAndIn<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            ThrowIfFilteringNotInitialised();
            AddNewFilterGroup(FilterGroupType.And);
            AddInFilterCondition(selector, values, alias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> NestedAndIn<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            return NestedAndIn<TEntity, TMember>(selector, values, alias);
        }

        public ISelectStatement<TEntity> NestedOr<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            ThrowIfFilteringNotInitialised();
            AddNewFilterGroup(FilterGroupType.Or);
            AddFilterCondition(selector, alias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> NestedOr(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return NestedOr<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> NestedOrBetween<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            ThrowIfFilteringNotInitialised();
            AddNewFilterGroup(FilterGroupType.Or);
            AddBetweenFilterCondition(selector, start, end, alias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> NestedOrBetween<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            return NestedOrBetween<TEntity, TMember>(selector, start, end, alias);
        }

        public ISelectStatement<TEntity> NestedOrIn<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            ThrowIfFilteringNotInitialised();
            AddNewFilterGroup(FilterGroupType.Or);
            AddInFilterCondition(selector, values, alias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> NestedOrIn<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            return NestedOrIn<TEntity, TMember>(selector, values, alias);
        }

        public ISelectStatement<TEntity> NoLocks()
        {
            Specification.NoLocks = true;
            return this;
        }

        public ISelectStatement<TEntity> On<TRight>(
            Expression<Func<TEntity, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null)
        {
            return On<TEntity, TRight>(expression, leftTableAlias, rightTableAlias);
        }

        public ISelectStatement<TEntity> On<TLeft, TRight>(
            Expression<Func<TLeft, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null)
        {
            if (Specification.Tables.Count < 2)
                throw new InvalidOperationException(
                    "On cannot be used before initialising a join with one of the Join methods.");
            AddJoinCondition(expression, leftTableAlias, rightTableAlias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> On<TRight>(
            Expression<Func<TEntity, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null,
            params Expression<Func<TRight, object>>[] additionalSelectors)
        {
            if (Specification.Tables.Count < 2)
                throw new InvalidOperationException(
                    "On cannot be used before initialising a join with one of the Join methods.");
            AddJoinCondition(expression, leftTableAlias, rightTableAlias, LogicalOperator.NotSet);
            AddJoinsColumnSelection(rightTableAlias, additionalSelectors);
            return this;
        }

        public ISelectStatement<TEntity> On<TRight>(
            Expression<Func<TEntity, TRight, bool>> expression,
            params Expression<Func<TRight, object>>[] additionalSelectors)
        {
            return On(expression, null, null, additionalSelectors);
        }

        public ISelectStatement<TEntity> Or<T>(
            Expression<Func<T, bool>> selector,
            string alias = null)
        {
            if (Specification.Filters.Count == 0)
                return Where(selector, alias, false);
            ThrowIfFilteringNotInitialised();
            AddFilterCondition(selector, alias, LogicalOperator.Or);
            return this;
        }

        public ISelectStatement<TEntity> Or(
            Expression<Func<TEntity, bool>> selector,
            string alias = null)
        {
            return Or<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> OrBetween<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            ThrowIfFilteringNotInitialised();
            AddBetweenFilterCondition(selector, start, end, alias, LogicalOperator.Or);
            return this;
        }

        public ISelectStatement<TEntity> OrBetween<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            return OrBetween<TEntity, TMember>(selector, start, end, alias);
        }

        public ISelectStatement<TEntity> OrderBy<T>(
            Expression<Func<T, object>> selector,
            string alias = null,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            ThrowIfTableNotSpecified<T>(alias);
            AddOrderSpecification(selector, alias, OrderByDirection.Ascending, additionalSelectors);
            return this;
        }

        public ISelectStatement<TEntity> OrderBy(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return OrderBy<TEntity>(selector, alias, additionalSelectors);
        }

        public ISelectStatement<TEntity> OrderByDescending<T>(
            Expression<Func<T, object>> selector,
            string alias = null,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            ThrowIfTableNotSpecified<T>(alias);
            AddOrderSpecification(selector, alias, OrderByDirection.Descending, additionalSelectors);
            return this;
        }

        public ISelectStatement<TEntity> OrderByDescending(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return OrderByDescending<TEntity>(selector, alias, additionalSelectors);
        }

        public ISelectStatement<TEntity> OrIn<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            ThrowIfFilteringNotInitialised();
            AddInFilterCondition(selector, values, alias, LogicalOperator.Or);
            return this;
        }

        public ISelectStatement<TEntity> OrIn<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            return OrIn<TEntity, TMember>(selector, values, alias);
        }

        public ISelectStatement<TEntity> OrOn<TRight>(
            Expression<Func<TEntity, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null)
        {
            return OrOn<TEntity, TRight>(expression, leftTableAlias, rightTableAlias);
        }

        public ISelectStatement<TEntity> OrOn<TLeft, TRight>(
            Expression<Func<TLeft, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null)
        {
            if (Specification.Tables.Count < 2)
                throw new InvalidOperationException(
                    "On cannot be used before initialising a join with one of the Join methods.");
            AddJoinCondition(expression, leftTableAlias, rightTableAlias, LogicalOperator.Or);
            return this;
        }

        public ISelectStatement<TEntity> Percent(bool useTopPercent = true)
        {
            if (!Specification.Top.HasValue)
                throw new InvalidOperationException("Please call Top to set a value before calling Percent");
            Specification.UseTopPercent = useTopPercent;
            return this;
        }

        public ISelectStatement<TEntity> RightOuterJoin<TLeft>(
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddTableSpecification<TLeft>(JoinType.RightOuter, alias, tableName, tableSchema);
            return this;
        }

        public ISelectStatement<TEntity> Select(
            Expression<Func<TEntity, object>> selector,
            string alias,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return Select<TEntity>(selector, alias, additionalSelectors);
        }

        public ISelectStatement<TEntity> Select<T>(
            Expression<Func<T, object>> selector,
            string alias,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            AddColumnSelection(selector, alias, additionalSelectors);
            return this;
        }

        public ISelectStatement<TEntity> Select(
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return Select(selector, null, additionalSelectors);
        }

        public ISelectStatement<TEntity> Select<T>(
            Expression<Func<T, object>> selector,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            return Select(selector, null, additionalSelectors);
        }

        public ISelectStatement<TEntity> SelectAll(string alias = null)
        {
            return SelectAll<TEntity>(alias);
        }

        public ISelectStatement<TEntity> SelectAll<T>(string alias = null)
        {
            AddColumnSelectionAll<T>(alias);
            return this;
        }

        public ISelectStatement<TEntity> Sum(
            Expression<Func<TEntity, object>> selector,
            string alias = null)
        {
            return Sum<TEntity>(selector, alias);
        }

        public ISelectStatement<TEntity> Sum(
            Expression<Func<TEntity, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            return Sum<TEntity>(selector, showselector, alias);
        }

        public ISelectStatement<TEntity> Sum<T>(
            Expression<Func<T, object>> selector,
            string alias = null)
        {
            var memberName = GetMemberName(selector);
            AddColumnSelection<T>(memberName, memberName, alias, Aggregation.Sum);
            return this;
        }

        public ISelectStatement<TEntity> Sum<T>(
            Expression<Func<T, object>> selector,
            Expression<Func<TEntity, object>> showselector,
            string alias = null)
        {
            AddColumnSelection<T>(GetMemberName(selector), GetMemberName(showselector), alias, Aggregation.Sum);
            return this;
        }

        public ISelectStatement<TEntity> Top(int rows)
        {
            Specification.Top = rows;
            return this;
        }

        public ISelectStatement<TEntity> Page(int rows, int page)
        {
            Specification.Top = rows;
            Specification.Page = page;
            return this;
        }

        public ISelectStatement<TEntity> Where<T>(
            Expression<Func<T, bool>> selector,
            string alias = null,
            bool addnest = false)
        {
            if (whereAllreadyAdd) return addnest ? NestedAnd(selector, alias) : And(selector, alias);

            whereAllreadyAdd = true;
            ThrowIfTableNotSpecified<T>(alias);
            InitialiseFiltering();
            AddFilterCondition(selector, alias);
            return this;
        }

        public ISelectStatement<TEntity> Where(
            Expression<Func<TEntity, bool>> selector,
            string alias = null,
            bool addnest = false)
        {
            return Where<TEntity>(selector, alias, addnest);
        }

        public ISelectStatement<TEntity> WhereBetween<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            if (whereAllreadyAdd)
            {
                ThrowIfFilteringNotInitialised();
                AddBetweenFilterCondition(selector, start, end, alias, LogicalOperator.And);
                return this;
            }

            InitialiseFiltering();
            AddBetweenFilterCondition(selector, start, end, alias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> WhereBetween<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null)
        {
            if (whereAllreadyAdd)
                return AndBetween<TEntity, TMember>(selector, start, end, alias);
            return WhereBetween<TEntity, TMember>(selector, start, end, alias);
        }

        public ISelectStatement<TEntity> WhereIn<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            if (whereAllreadyAdd)
            {
                ThrowIfFilteringNotInitialised();
                AddInFilterCondition(selector, values, alias, LogicalOperator.And);
                return this;
            }

            InitialiseFiltering();
            AddInFilterCondition(selector, values, alias, LogicalOperator.NotSet);
            return this;
        }

        public ISelectStatement<TEntity> WhereIn<TMember>(
            Expression<Func<TEntity, TMember>> selector,
            TMember[] values,
            string alias = null)
        {
            if (whereAllreadyAdd)
                return AndIn<TEntity, TMember>(selector, values, alias);
            return WhereIn<TEntity, TMember>(selector, values, alias);
        }

        public ISelectStatement<TEntity> UsingTableName(string tableName)
        {
            Specification.Tables.First().TableName = tableName;
            return this;
        }

        public abstract int GetPageCount();


        public abstract ValueTuple<IEnumerable<TEntity>, int> PageGo();
        public abstract (string, List<PartialSelectSelectParameterDefinition>) SqlWithParams();


        public virtual ISelectStatement<TEntity> Distinct()
        {
            Specification.Distinct = true;
            return this;
        }

        public virtual TEntity FirstOrDefault()
        {
            Top(1);
            return Go().FirstOrDefault();
        }

        private string SelectFromSql()
        {
            var str = string.Join("\n, ", Specification.Columns.Select(c => c.Identifier).ToArray());
            return string.Format("SELECT {0}{1} From (", string.IsNullOrEmpty(str) ? "*" : str);
        }

        protected string GetNoSemicolonSql(string sql)
        {
            var length = sql.LastIndexOf(';');
            if (length > 5)
                return sql.Substring(0, length);
            return sql;
        }

        public List<TEntity> ListGo()
        {
            using (var reader = StatementExecutor.ExecuteReader(Sql()))
            {
                return EntityMapper.MapList<TEntity>(reader);
            }
        }

        public async Task<List<TEntity>> ListGoAsync()
        {
            List<TEntity> entityList;
            using (var reader = await StatementExecutor.ExecuteReaderAsync(Sql()))
            {
                entityList = EntityMapper.MapList<TEntity>(reader);
            }

            return entityList;
        }

        public TLEntity ListEntityGo<TLEntity>() where TLEntity : List<TEntity>, new()
        {
            using (var reader = StatementExecutor.ExecuteReader(Sql()))
            {
                return EntityMapper.MapEntityList<TLEntity, TEntity>(reader);
            }
        }

        public async Task<TLEntity> ListEntityGoAsync<TLEntity>() where TLEntity : List<TEntity>, new()
        {
            TLEntity lentity;
            using (var reader = await StatementExecutor.ExecuteReaderAsync(Sql()))
            {
                lentity = EntityMapper.MapEntityList<TLEntity, TEntity>(reader);
            }

            return lentity;
        }

        public async Task<List<TEntity>> ListEntityGoAsync()
        {
            List<TEntity> entityList;
            using (var reader = await StatementExecutor.ExecuteReaderAsync(Sql()))
            {
                entityList = EntityMapper.MapList<TEntity>(reader);
            }

            return entityList;
        }

        protected abstract void AddBetweenFilterCondition<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember start,
            TMember end,
            string alias = null,
            LogicalOperator logicalOperator = LogicalOperator.NotSet);

        protected void AddColumnSelectionAll<T>(string alias = null)
        {
            foreach (var keyValuePair in typeof(TEntity).GetProperties()
                .Where(p => WritablePropertyMatcher.TestIsDbField(p)).ToDictionary(p => p.Name, p => p.ColumnName()))
                AddColumnSelection<T>(keyValuePair.Key, keyValuePair.Value, alias, Aggregation.None);
        }

        protected void AppendColumnSelectionJoinOrUnion<T>(string alias = null)
        {
            var dictionary = typeof(TEntity).GetProperties()
                .Where(p => !WritablePropertyMatcher.TestIsDbField(p))
                .ToDictionary(p => p.Name,
                    p => p.ColumnName());
            foreach (var joinsColumn in Specification
                .JoinsColumns)
            foreach (var keyValuePair in dictionary)
            {
                var name = keyValuePair;
                if (Specification.Columns
                        .Where(
                            s => s.ColumnName == name.Value)
                        .FirstOrDefault() == null && joinsColumn.Identifier == name.Value)
                    AddColumnSelection<T>(name.Key, name.Value, alias, Aggregation.None);
            }
        }

        protected void AddColumnSelection<T>(
            Expression<Func<T, object>> selector,
            string alias = null,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            var memberName1 = GetMemberName(selector);
            AddColumnSelection<T>(memberName1, memberName1, alias, Aggregation.None);
            foreach (var additionalSelector in additionalSelectors)
            {
                var memberName2 = GetMemberName(additionalSelector);
                AddColumnSelection<T>(memberName2, memberName2, alias, Aggregation.None);
            }
        }

        protected void AddJoinsColumnSelection<T>(
            string alias = null,
            params Expression<Func<T, object>>[] additionalSelectors)
        {
            foreach (var additionalSelector in additionalSelectors)
                AddJoinsColumnSelection<T>(GetMemberColumnName(additionalSelector), alias);
        }

        protected abstract void AddColumnSelection<T>(
            string name,
            string showname,
            string alias = null,
            Aggregation aggregation = Aggregation.None);

        protected abstract void AddJoinsColumnSelection<T>(string name, string alias = null);

        protected abstract void AddFilterCondition<T>(
            Expression<Func<T, bool>> selector,
            string alias = null,
            LogicalOperator logicalOperator = LogicalOperator.NotSet);

        protected abstract void AddGroupSpecification<T>(
            Expression<Func<T, object>> selector,
            string alias = null,
            params Expression<Func<T, object>>[] additionalSelectors);

        protected abstract void AddHavingSpecification<T>(
            Expression<Func<T, bool>> selector,
            Aggregation aggregation,
            string alias = null);

        protected abstract void AddInFilterCondition<T, TMember>(
            Expression<Func<T, TMember>> selector,
            TMember[] values,
            string alias = null,
            LogicalOperator locigalOperator = LogicalOperator.NotSet);

        protected abstract void AddJoinCondition<TLeft, TRight>(
            Expression<Func<TLeft, TRight, bool>> expression,
            string leftTableAlias = null,
            string rightTableAlias = null,
            LogicalOperator locLogicalOperator = LogicalOperator.NotSet);

        protected abstract void AddNewFilterGroup(FilterGroupType filterGroupType);

        protected abstract void AddOrderSpecification<T>(
            Expression<Func<T, object>> selector,
            string alias = null,
            OrderByDirection orderByDirection = OrderByDirection.Ascending,
            params Expression<Func<T, object>>[] additionalSelectors);

        protected abstract void AddTableSpecification<T>(
            JoinType joinType,
            string alias = null,
            string tableName = null,
            string tableSchema = null);

        protected void FinalizeColumnSpecifications()
        {
            var specificationBase1 = Specification.Tables.First();
            if (Specification.Columns.Count == 0)
                AddColumnSelectionAll<TEntity>(specificationBase1.Alias);
            AppendColumnSelectionJoinOrUnion<TEntity>(specificationBase1.Alias);
            foreach (var column in Specification.Columns)
            {
                var specification = column;
                var tableSpecification1 = FindTableSpecification(specification.EntityType, specification.Alias);
                var specificationBase2 = Specification.JoinsColumns.Where(j =>
                {
                    if (!(j.Identifier == specification.Identifier))
                        return j.Identifier == specification.ColumnName;
                    return true;
                }).FirstOrDefault();
                if (specificationBase2 != null)
                {
                    var tableSpecification2 =
                        FindTableSpecification(specificationBase2.EntityType, specificationBase2.Alias);
                    specification.Table = tableSpecification2.TableName;
                    specification.Schema = tableSpecification2.Schema;
                }
                else
                {
                    specification.Table = tableSpecification1.TableName;
                    specification.Schema = tableSpecification1.Schema;
                }
            }
        }

        protected void FinalizeGroupings()
        {
            foreach (var grouping in Specification.Groupings)
            {
                var tableSpecification = FindTableSpecification(grouping.EntityType, grouping.Alias);
                grouping.TableName = tableSpecification.TableName;
                grouping.Schema = tableSpecification.Schema;
            }
        }

        protected void FinalizeHavings()
        {
            foreach (var having in Specification.Havings)
            {
                var tableSpecification = FindTableSpecification(having.EntityType, having.Alias);
                having.TableName = tableSpecification.TableName;
                having.Schema = tableSpecification.Schema;
            }
        }

        protected void FinalizeJoinConditions()
        {
            foreach (var join in Specification.Joins)
            {
                var tableSpecification1 = FindTableSpecification(join.LeftEntityType, join.LeftTableAlias);
                join.LeftTableName = tableSpecification1.TableName;
                join.LeftSchema = tableSpecification1.Schema;
                var tableSpecification2 = FindTableSpecification(join.RightEntityType, join.RightTableAlias);
                if (tableSpecification2 != null)
                {
                    join.RightTableName = tableSpecification2.TableName;
                    join.RightSchema = tableSpecification2.Schema;
                }
            }
        }

        protected void FinalizeOrderings()
        {
            foreach (var ordering in Specification.Orderings)
            {
                var tableSpecification = FindTableSpecification(ordering.EntityType, ordering.Alias);
                ordering.TableName = tableSpecification.TableName;
                ordering.Schema = tableSpecification.Schema;
            }
        }

        protected void FinalizeWhereConditions(IList<FilterGroupBase> filterGroups)
        {
            foreach (var filterGroup in filterGroups)
            {
                foreach (var condition in filterGroup.Conditions)
                {
                    var tableSpecification = FindTableSpecification(condition.EntityType, condition.Alias);
                    condition.TableName = tableSpecification.TableName;
                    condition.Schema = tableSpecification.Schema;
                }

                FinalizeWhereConditions(filterGroup.Groups);
            }
        }

        protected SelectStatementTableSpecificationBase FindTableSpecification<T>(
            string alias = null)
        {
            return FindTableSpecification(typeof(T), alias);
        }

        protected SelectStatementTableSpecificationBase FindTableSpecification(
            Type entityType,
            string alias = null)
        {
            return Specification.Tables.FirstOrDefault(e =>
            {
                if (e.EntityType == entityType)
                    return e.Alias == alias;
                return false;
            });
        }

        protected abstract void InitialiseConfig();

        protected abstract void InitialiseFiltering();

        protected string OperatorStringFromComparison(Comparison comparison)
        {
            switch (comparison)
            {
                case Comparison.NotEqual:
                    return "<>";
                case Comparison.LessThan:
                    return "<";
                case Comparison.LessThanOrEqual:
                    return "<=";
                case Comparison.GreaterThan:
                    return ">";
                case Comparison.GreaterThanOrEqual:
                    return ">=";
                default:
                    return "=";
            }
        }

        protected void ThrowIfFilteringNotInitialised()
        {
            if (Specification.Filters.Count == 0)
                throw new InvalidOperationException(
                    "Filtering has not been initialised, please use a Where method before any And or Or method.");
        }

        protected void ThrowIfGroupingNotInitialised()
        {
            if (Specification.Groupings.Count == 0)
                throw new InvalidOperationException(
                    "Grouping has not been initialised, pluase a GroupBy method before any Having method.");
        }

        protected void ThrowIfTableAlreadyJoined<T>(string alias = null, string tableName = null,
            string tableSchema = null)
        {
            if (FindTableSpecification<T>(alias) != null)
                throw new InvalidOperationException(
                    "The entity has already been joined into the query, you must use a unique alias, table name override or schema override to join it again.");
        }

        protected void ThrowIfTableNotSpecified<T>(string alias = null)
        {
            if (FindTableSpecification<T>(alias) == null)
                throw new InvalidOperationException(
                    "A table specification for the entity type and alias must be set using From or one of the Join methods before filtering, sorting or grouping can be applied.");
        }
    }
}