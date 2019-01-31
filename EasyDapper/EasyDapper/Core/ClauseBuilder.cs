using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Atk.AtkExpression;
using EasyDapper.Abstractions;
using EesyDapper.Core.CustomAttribute;

namespace EasyDapper.Core
{
    public abstract class ClauseBuilder : IClauseBuilder
    {
        public const string DefaultSchema = "dbo";

        protected ClauseBuilder()
        {
            IsClean = true;
        }

        public bool IsClean { get; set; }

        public abstract string Sql();

        protected Expression<Func<T, object>> ConvertExpression<T, TMember>(Expression<Func<T, TMember>> selector)
        {
            return Expression.Lambda<Func<T, object>>(Expression.Convert(selector.Body, typeof(object)),
                selector.Parameters);
        }

        protected internal virtual string FormatValue(object value)
        {
            if (value is string || value is Guid)
                return "'" + ReplaceSingleQuoteWithDoubleQuote(string.Format("{0}", value)) + "'";
            if (value is DateTime)
                return "'" + ((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            if (value is DateTimeOffset)
                return "'" + ((DateTimeOffset) value).ToString("yyyy-MM-dd HH:mm:ss.ffffffzzz") + "'";
            if (value is Enum)
                return Convert.ToInt32(value).ToString();
            if (value is byte[])
            {
                var base64String = Convert.ToBase64String((byte[]) value);
                var num = base64String.Length;
                if (string.IsNullOrWhiteSpace(base64String))
                    num = 1;
                return string.Format("CAST('{0}' as varbinary({1}))", base64String, num);
            }

            if (!(value is bool))
                return value?.ToString() ?? "NULL";
            return (bool) value ? "1" : "0";
        }

        protected object GetExpressionValue(Expression expression)
        {
            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null) return constantExpression.Value;

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null) return GetExpressionValue(unaryExpression.Operand);

            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                var binaryExpression = lambdaExpression.Body as BinaryExpression;
                if (binaryExpression != null)
                    return GetExpressionValue(AtkPartialEvaluator.Eval(binaryExpression.Right));

                var callExpression = lambdaExpression.Body as MethodCallExpression;
                if (callExpression != null) return GetExpressionValue(callExpression);
            }

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                var fieldsOfObj = memberExpression.Expression as ConstantExpression;
                var propertyInfo = memberExpression.Member as PropertyInfo;

                if (propertyInfo != null && !propertyInfo.PropertyType.IsSimpleType())
                    return propertyInfo.GetValue(fieldsOfObj, null);

                var value = GetExpressionValue(memberExpression.Expression);
                return this.ResolveValue((dynamic) memberExpression.Member, value);
            }

            throw new ArgumentException("Expected constant expression");
        }

        protected string GetExpressionValue(MethodCallExpression callExpression)
        {
            var expressionValue = (string) GetExpressionValue(callExpression.Arguments.First());
            var name = callExpression.Method.Name;
            if (name == "EndsWith")
                return string.Format("{0}{1}{2}", "%", expressionValue, string.Empty);
            if (name == "StartsWith")
                return string.Format("{0}{1}{2}", string.Empty, expressionValue, "%");
            return string.Format("{0}{1}{2}", "%", expressionValue, "%");
        }

        protected MemberExpression GetMemberExpression(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
                return memberExpression;
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
                return GetMemberExpression(binaryExpression.Left);
            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
                return GetMemberExpression(unaryExpression.Operand);
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
                return GetMemberExpression(methodCallExpression.Object);
            throw new ArgumentException("Member expression expected");
        }

        protected internal string GetMemberName<T, TMember>(Expression<Func<T, TMember>> selector)
        {
            return GetMemberName(GetMemberExpression(selector.Body));
        }

        protected internal string GetMemberColumnName<T, TMember>(Expression<Func<T, TMember>> selector)
        {
            return GetColumnAlias<T>(GetMemberName(GetMemberExpression(selector.Body)));
        }

        protected string GetColumnAlias<TEntity>(string columnName)
        {
            var element = typeof(TEntity).GetProperties().Where(p => p.Name == columnName).FirstOrDefault();
            if (element == null)
                return columnName;
            var customAttribute = (ColumnAttribute) element.GetCustomAttribute(typeof(ColumnAttribute));
            if (customAttribute != null)
                return customAttribute.Name;
            return columnName;
        }

        protected string GetMemberName(Expression expression)
        {
            return GetMemberExpression(expression).Member.Name;
        }

        protected string GetOperator<T, TMember>(Expression<Func<T, TMember>> expression)
        {
            LambdaExpression lambdaExpression = expression;
            if (lambdaExpression != null)
            {
                var body1 = lambdaExpression.Body as BinaryExpression;
                if (body1 != null)
                    return OperatorString(body1.NodeType);
                var body2 = lambdaExpression.Body as MethodCallExpression;
                if (body2 != null)
                    return OperatorString(body2.Method.Name);
            }

            return "=";
        }

        protected string OperatorString(ExpressionType @operator)
        {
            switch (@operator)
            {
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.Not:
                case ExpressionType.NotEqual:
                    return "<>";
                default:
                    return "=";
            }
        }

        protected string TableNameFromType<TEntity>()
        {
            return CustomAttributeHandle.DbTableName<TEntity>();
        }

        protected string OperatorString(string methodName)
        {
            return "LIKE";
        }

        protected string ReplaceSingleQuoteWithDoubleQuote(string originalString)
        {
            var str = originalString;
            for (var startIndex = 0; startIndex < str.Length; ++startIndex)
                if (str[startIndex] == '\'' && (startIndex == 0 || str[startIndex - 1] != '\'') &&
                    (startIndex == str.Length - 1 || str[startIndex + 1] != '\''))
                    str = str.Insert(startIndex, "'");
            return str;
        }

        protected object ResolveMethodCall(MethodCallExpression callExpression)
        {
            var array = callExpression.Arguments.Select(GetExpressionValue).ToArray();
            var obj = callExpression.Object != null ? GetExpressionValue(callExpression.Object) : array.First();
            return callExpression.Method.Invoke(obj, array);
        }

        protected object ResolveValue(PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }

        protected object ResolveValue(FieldInfo field, object obj)
        {
            return field.GetValue(obj);
        }
    }
}