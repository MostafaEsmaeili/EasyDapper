
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Atk.AtkExpression
{
  internal static class AtkTypeHelper
  {
    public static string GetColumnAlias<TEntity>(string columnName)
    {
      PropertyInfo element = typeof (TEntity).GetProperties().Where(p => p.Name == columnName).FirstOrDefault();
      if (element == null)
        return columnName;
      ColumnAttribute customAttribute = (ColumnAttribute) element.GetCustomAttribute(typeof (ColumnAttribute));
      if (customAttribute != null)
        return customAttribute.Name;
      return columnName;
    }

    public static Type FindIEnumerable(Type seqType)
    {
      if (seqType == null || seqType == typeof (string))
        return null;
      if (seqType.IsArray)
        return typeof (IEnumerable<>).MakeGenericType(seqType.GetElementType());
      if (seqType.IsGenericType)
      {
        foreach (Type genericArgument in seqType.GetGenericArguments())
        {
          Type type = typeof (IEnumerable<>).MakeGenericType(genericArgument);
          if (type.IsAssignableFrom(seqType))
            return type;
        }
      }
      Type[] interfaces = seqType.GetInterfaces();
      if (interfaces != null && (uint) interfaces.Length > 0U)
      {
        foreach (Type seqType1 in interfaces)
        {
          Type ienumerable = FindIEnumerable(seqType1);
          if (ienumerable != null)
            return ienumerable;
        }
      }
      if (seqType.BaseType != null && seqType.BaseType != typeof (object))
        return FindIEnumerable(seqType.BaseType);
      return null;
    }

    public static Type GetSequenceType(Type elementType)
    {
      return typeof (IEnumerable<>).MakeGenericType(elementType);
    }

    public static Type GetElementType(Type seqType)
    {
      Type ienumerable = FindIEnumerable(seqType);
      if (ienumerable == null)
        return seqType;
      return ienumerable.GetGenericArguments()[0];
    }

    public static bool IsNullableType(Type type)
    {
      return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public static bool IsNullAssignable(Type type)
    {
      return !type.IsValueType || IsNullableType(type);
    }

    public static Type GetNonNullableType(Type type)
    {
      if (IsNullableType(type))
        return type.GetGenericArguments()[0];
      return type;
    }

    public static Type GetNullAssignableType(Type type)
    {
      if (IsNullAssignable(type))
        return type;
      return typeof (Nullable<>).MakeGenericType(type);
    }

    public static ConstantExpression GetNullConstant(Type type)
    {
      return Expression.Constant(null, GetNullAssignableType(type));
    }

    public static Type GetMemberType(MemberInfo mi)
    {
      FieldInfo fieldInfo = mi as FieldInfo;
      if (fieldInfo != null)
        return fieldInfo.FieldType;
      PropertyInfo propertyInfo = mi as PropertyInfo;
      if (propertyInfo != null)
        return propertyInfo.PropertyType;
      EventInfo eventInfo = mi as EventInfo;
      if (eventInfo != null)
        return eventInfo.EventHandlerType;
      MethodInfo methodInfo = mi as MethodInfo;
      if (methodInfo != null)
        return methodInfo.ReturnType;
      return null;
    }

    public static object GetDefault(Type type)
    {
      if (type.IsValueType && !IsNullableType(type))
        return Activator.CreateInstance(type);
      return null;
    }

    public static bool IsReadOnly(MemberInfo member)
    {
      switch (member.MemberType)
      {
        case MemberTypes.Field:
          return (uint) (((FieldInfo) member).Attributes & FieldAttributes.InitOnly) > 0U;
        case MemberTypes.Property:
          PropertyInfo propertyInfo = (PropertyInfo) member;
          return !propertyInfo.CanWrite || propertyInfo.GetSetMethod() == null;
        default:
          return true;
      }
    }

    public static bool IsInteger(Type type)
    {
      GetNonNullableType(type);
      return (uint) (Type.GetTypeCode(type) - 5) <= 7U;
    }
  }
}
