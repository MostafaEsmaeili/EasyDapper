using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyDapper.Core
{
    public static class Reflect<TTarget>
    {
        public static MethodInfo GetMethod(Expression<Action<TTarget>> method)
        {
            return GetMethodInfo(method);
        }

        public static MethodInfo GetMethod<T1>(Expression<Action<TTarget, T1>> method)
        {
            return GetMethodInfo(method);
        }

        public static MethodInfo GetMethod<T1, T2>(Expression<Action<TTarget, T1, T2>> method)
        {
            return GetMethodInfo(method);
        }

        public static MethodInfo GetMethod<T1, T2, T3>(
            Expression<Action<TTarget, T1, T2, T3>> method)
        {
            return GetMethodInfo(method);
        }

        private static MethodInfo GetMethodInfo(Expression method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            var lambdaExpression = method as LambdaExpression;
            if (lambdaExpression == null)
                throw new ArgumentException("Not a lambda expression", nameof(method));
            if (lambdaExpression.Body.NodeType != ExpressionType.Call)
                throw new ArgumentException("Not a method call", nameof(method));
            return ((MethodCallExpression) lambdaExpression.Body).Method;
        }

        public static PropertyInfo GetProperty(Expression<Func<TTarget, object>> property)
        {
            var memberInfo = GetMemberInfo(property) as PropertyInfo;
            if (memberInfo == null)
                throw new ArgumentException("Member is not a property");
            return memberInfo;
        }

        public static PropertyInfo GetProperty<P>(Expression<Func<TTarget, P>> property)
        {
            var memberInfo = GetMemberInfo(property) as PropertyInfo;
            if (memberInfo == null)
                throw new ArgumentException("Member is not a property");
            return memberInfo;
        }

        public static FieldInfo GetField(Expression<Func<TTarget, object>> field)
        {
            var memberInfo = GetMemberInfo(field) as FieldInfo;
            if (memberInfo == null)
                throw new ArgumentException("Member is not a field");
            return memberInfo;
        }

        private static MemberInfo GetMemberInfo(Expression member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            var lambdaExpression = member as LambdaExpression;
            if (lambdaExpression == null)
                throw new ArgumentException("Not a lambda expression", nameof(member));
            var memberExpression = (MemberExpression) null;
            if (lambdaExpression.Body.NodeType == ExpressionType.Convert)
                memberExpression = ((UnaryExpression) lambdaExpression.Body).Operand as MemberExpression;
            else if (lambdaExpression.Body.NodeType == ExpressionType.MemberAccess)
                memberExpression = lambdaExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Not a member access", nameof(member));
            return memberExpression.Member;
        }

        public static List<PropertyInfo> GetPropertys<P>(
            Expression<Func<TTarget, P>> property)
        {
            var memberInfos = GetMemberInfos(property);
            var propertyInfoList = new List<PropertyInfo>();
            foreach (var memberInfo in memberInfos)
                propertyInfoList.Add(memberInfo as PropertyInfo);
            return propertyInfoList;
        }

        private static List<MemberInfo> GetMemberInfos(Expression member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            var lambdaExpression = member as LambdaExpression;
            if (lambdaExpression == null)
                throw new ArgumentException("Not a lambda expression", nameof(member));
            var memberExpression = (MemberExpression) null;
            var memberInfoList = new List<MemberInfo>();
            if (lambdaExpression.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression) lambdaExpression.Body).Operand as MemberExpression;
            }
            else if (lambdaExpression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = lambdaExpression.Body as MemberExpression;
            }
            else if (lambdaExpression.Body.NodeType == ExpressionType.New)
            {
                var body = lambdaExpression.Body as NewExpression;
                foreach (var member1 in (lambdaExpression.Body as NewExpression).Members)
                    memberInfoList.Add(member1);
            }

            if (memberInfoList.Count == 0)
            {
                if (memberExpression == null)
                    throw new ArgumentException("Not a member access", nameof(member));
                memberInfoList.Add(memberExpression.Member);
            }

            return memberInfoList;
        }
    }
}