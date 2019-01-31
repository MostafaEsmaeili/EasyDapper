using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
    public abstract class SelectClauseBaseBuilder : ClauseBuilder, ISelectClauseBuilder, IClauseBuilder
    {
        protected const string ClauseTemplate = "SELECT {0}{1}";
        protected readonly IList<ColumnSelectionBase> selections = new List<ColumnSelectionBase>();
        protected int? topRows;

        public ISelectClauseBuilder Avg<TEntity>(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddColumnSelection<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Avg);
            return this;
        }

        public ISelectClauseBuilder Count<TEntity>(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddColumnSelection<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Count);
            return this;
        }

        public abstract ISelectClauseBuilder CountAll();

        public ISelectClauseBuilder For<TEntity>(
            TEntity entity,
            string alias = null,
            string tableSchema = null,
            string tableName = null)
        {
            foreach (var propertyInfo in entity.GetType().GetProperties().Where(p => p.CanWrite))
                AddColumnSelection<TEntity>(alias, tableName, tableSchema, propertyInfo.Name, Aggregation.None);
            IsClean = false;
            return this;
        }

        public ISelectClauseBuilder Max<TEntity>(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddColumnSelection<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Max);
            return this;
        }

        public ISelectClauseBuilder Min<TEntity>(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddColumnSelection<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Min);
            return this;
        }

        public ISelectClauseBuilder Select<TEntity>(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            string tableName = null,
            string tableSchema = null,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            AddColumnSelection<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.None);
            foreach (var additionalSelector in additionalSelectors)
                AddColumnSelection<TEntity>(alias, tableName, tableSchema, GetMemberName(additionalSelector),
                    Aggregation.None);
            IsClean = false;
            return this;
        }

        public ISelectClauseBuilder SelectAll<TEntity>(
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddColumnSelection<TEntity>(alias, tableName, tableSchema, "*", Aggregation.None);
            return this;
        }

        public ISelectClauseBuilder Sum<TEntity>(
            Expression<Func<TEntity, object>> selector,
            string alias = null,
            string tableName = null,
            string tableSchema = null)
        {
            AddColumnSelection<TEntity>(alias, tableName, tableSchema, GetMemberName(selector), Aggregation.Sum);
            return this;
        }

        public ISelectClauseBuilder Top(int rows)
        {
            topRows = rows;
            return this;
        }

        protected abstract void AddColumnSelection<TEntity>(
            string alias,
            string tableName,
            string tableSchema,
            string name,
            Aggregation aggregation = Aggregation.None);
    }
}