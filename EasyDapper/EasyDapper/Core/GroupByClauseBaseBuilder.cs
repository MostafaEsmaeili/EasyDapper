using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
    public abstract class GroupByClauseBaseBuilder : ClauseBuilder, IGroupByClauseBuilder, IClauseBuilder
    {
        protected readonly IList<GroupBySpecificationBase> GroupBySpecifications = new List<GroupBySpecificationBase>();
        protected readonly IList<HavingSpecificationBase> HavingSpecifications = new List<HavingSpecificationBase>();
        private const string ClauseTemplate = "GROUP BY {0}";
        private const string HavingClauseTemplate = "\nHAVING {0}";

        public IGroupByClauseBuilder By<TEntity>(Expression<Func<TEntity, object>> selector, string alias = null,
            string tableName = null, string tableSchema = null,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            AddGroupBySpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(selector));
            foreach (var additionalSelector in additionalSelectors)
                AddGroupBySpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(additionalSelector));
            IsClean = false;
            return this;
        }

        public IGroupByClauseBuilder HavingAvg<TEntity>(Expression<Func<TEntity, object>> selector,
            Comparison comparison, int value, string alias = null, string tableName = null, string tableSchema = null)
        {
            AddHavingSpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Avg,
                comparison, value);
            return this;
        }

        public IGroupByClauseBuilder HavingCount<TEntity>(Expression<Func<TEntity, object>> selector,
            Comparison comparison, int value, string alias = null, string tableName = null, string tableSchema = null)
        {
            AddHavingSpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Count,
                comparison, value);
            return this;
        }

        public IGroupByClauseBuilder HavingCountAll<TEntity>(Comparison comparison, int value)
        {
            AddHavingSpecification<TEntity>(null, null, null, "*", Aggregation.Count, comparison, value);
            return this;
        }

        public IGroupByClauseBuilder HavingMax<TEntity>(Expression<Func<TEntity, object>> selector,
            Comparison comparison, int value, string alias = null, string tableName = null, string tableSchema = null)
        {
            AddHavingSpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Max,
                comparison, value);
            return this;
        }

        public IGroupByClauseBuilder HavingMin<TEntity>(Expression<Func<TEntity, object>> selector,
            Comparison comparison, int value, string alias = null, string tableName = null, string tableSchema = null)
        {
            AddHavingSpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Min,
                comparison, value);
            return this;
        }

        public IGroupByClauseBuilder HavingSum<TEntity>(Expression<Func<TEntity, object>> selector,
            Comparison comparison, int value, string alias = null, string tableName = null, string tableSchema = null)
        {
            AddHavingSpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Sum,
                comparison, value);
            return this;
        }

        public override string Sql()
        {
            if (!GroupBySpecifications.Any())
                return string.Empty;
            var str = $"GROUP BY {string.Join(", ", GroupBySpecifications)}";
            if (HavingSpecifications.Any())
                str += $"\nHAVING {string.Join(", ", HavingSpecifications)}";
            return str;
        }

        protected abstract void AddGroupBySpecification<TEntity>(string alias, string tableName, string tableSchema,
            string name);

        protected abstract void AddHavingSpecification<TEntity>(string alias, string tableName, string tableSchema,
            string name, Aggregation aggregation, Comparison comparison, object value);
    }
}
