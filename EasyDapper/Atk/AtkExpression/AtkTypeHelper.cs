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
      PropertyInfo element = ((IEnumerable<PropertyInfo>) typeof (TEntity).GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name == columnName)).FirstOrDefault<PropertyInfo>();
      if (element == (PropertyInfo) null)
        return columnName;
      ColumnAttribute customAttribute = (ColumnAttribute) element.GetCustomAttribute(typeof (ColumnAttribute));
      if (customAttribute != null)
        return customAttribute.Name;
      return columnName;
    }

    public static Type FindIEnumerable(Type seqType)
    {
      if (seqType == (Type) null || seqType == typeof (string))
        return (Type) null;
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
          Type ienumerable = AtkTypeHelper.FindIEnumerable(seqType1);
          if (ienumerable != (Type) null)
            return ienumerable;
        }
      }
      if (seqType.BaseType != (Type) null && seqType.BaseType != typeof (object))
        return AtkTypeHelper.FindIEnumerable(seqType.BaseType);
      return (Type) null;
    }

    public static Type GetSequenceType(Type elementType)
    {
      return typeof (IEnumerable<>).MakeGenericType(elementType);
    }

    public static Type GetElementType(Type seqType)
    {
      Type ienumerable = AtkTypeHelper.FindIEnumerable(seqType);
      if (ienumerable == (Type) null)
        return seqType;
      return ienumerable.GetGenericArguments()[0];
    }

    public static bool IsNullableType(Type type)
    {
      return type != (Type) null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public static bool IsNullAssignable(Type type)
    {
      return !type.IsValueType || AtkTypeHelper.IsNullableType(type);
    }

    public static Type GetNonNullableType(Type type)
    {
      if (AtkTypeHelper.IsNullableType(type))
        return type.GetGenericArguments()[0];
      return type;
    }

    public static Type GetNullAssignableType(Type type)
    {
      if (AtkTypeHelper.IsNullAssignable(type))
        return type;
      return typeof (Nullable<>).MakeGenericType(type);
    }

    public static ConstantExpression GetNullConstant(Type type)
    {
      return Expression.Constant((object) null, AtkTypeHelper.GetNullAssignableType(type));
    }

    public static Type GetMemberType(MemberInfo mi)
    {
      FieldInfo fieldInfo = mi as FieldInfo;
      if (fieldInfo != (FieldInfo) null)
        return fieldInfo.FieldType;
      PropertyInfo propertyInfo = mi as PropertyInfo;
      if (propertyInfo != (PropertyInfo) null)
        return propertyInfo.PropertyType;
      EventInfo eventInfo = mi as EventInfo;
      if (eventInfo != (EventInfo) null)
        return eventInfo.EventHandlerType;
      MethodInfo methodInfo = mi as MethodInfo;
      if (methodInfo != (MethodInfo) null)
        return methodInfo.ReturnType;
      return (Type) null;
    }

    public static object GetDefault(Type type)
    {
      if (type.IsValueType && !AtkTypeHelper.IsNullableType(type))
        return Activator.CreateInstance(type);
      return (object) null;
    }

    public static bool IsReadOnly(MemberInfo member)
    {
      switch (member.MemberType)
      {
        case MemberTypes.Field:
          return (uint) (((FieldInfo) member).Attributes & FieldAttributes.InitOnly) > 0U;
        case MemberTypes.Property:
          PropertyInfo propertyInfo = (PropertyInfo) member;
          return !propertyInfo.CanWrite || propertyInfo.GetSetMethod() == (MethodInfo) null;
        default:
          return true;
      }
    }

    public static bool IsInteger(Type type)
    {
      AtkTypeHelper.GetNonNullableType(type);
      return (uint) (Type.GetTypeCode(type) - 5) <= 7U;
    }
  }
}
