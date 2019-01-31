using System;
using System.Linq.Expressions;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.Core
{
    public abstract class OrderByClauseBaseBuilder : ClauseBuilder, IOrderByClauseBuilder, IClauseBuilder
    {
        protected const string ClauseTemplate = "ORDER BY {0}";

        public string ActiveAlias { get; private set; }

        public IOrderByClauseBuilder By<TEntity>(
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return By(ActiveAlias, TableNameFromType<TEntity>(), "dbo", selector, additionalSelectors);
        }

        public IOrderByClauseBuilder By<TEntity>(
            string alias,
            string tableName,
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return By(alias, tableName, "dbo", selector, additionalSelectors);
        }

        public IOrderByClauseBuilder ByDescending<TEntity>(
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return ByDescending(ActiveAlias, TableNameFromType<TEntity>(), "dbo", selector, additionalSelectors);
        }

        public IOrderByClauseBuilder ByDescending<TEntity>(
            string alias,
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            return ByDescending(alias, TableNameFromType<TEntity>(), "dbo", selector, additionalSelectors);
        }

        public abstract IOrderByClauseBuilder FromScratch();

        public IOrderByClauseBuilder UsingAlias(string alias)
        {
            ActiveAlias = alias;
            return this;
        }

        public IOrderByClauseBuilder By<TEntity>(
            string alias,
            string tableName,
            string tableSchema,
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            AddOrderBySpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(selector),
                OrderByDirection.Ascending);
            foreach (var additionalSelector in additionalSelectors)
                AddOrderBySpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(additionalSelector),
                    OrderByDirection.Ascending);
            IsClean = false;
            return this;
        }

        public IOrderByClauseBuilder ByDescending<TEntity>(
            string alias,
            string tableName,
            string tableSchema,
            Expression<Func<TEntity, object>> selector,
            params Expression<Func<TEntity, object>>[] additionalSelectors)
        {
            AddOrderBySpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(selector),
                OrderByDirection.Descending);
            foreach (var additionalSelector in additionalSelectors)
                AddOrderBySpecification<TEntity>(alias, tableName, tableSchema, GetMemberName(additionalSelector),
                    OrderByDirection.Descending);
            IsClean = false;
            return this;
        }

        protected abstract void AddOrderBySpecification<TEntity>(
            string alias,
            string tableName,
            string tableSchema,
            string name,
            OrderByDirection direction = OrderByDirection.Ascending);
    }
}