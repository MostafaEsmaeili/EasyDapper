
using System;
using System.Reflection;

namespace Atk.AtkExpression
{
  internal static class AtkReflectionExtensions
  {
    public static object GetValue(this MemberInfo member, object instance)
    {
      switch (member.MemberType)
      {
        case MemberTypes.Field:
          return ((FieldInfo) member).GetValue(instance);
        case MemberTypes.Property:
          return ((PropertyInfo) member).GetValue(instance, null);
        default:
          throw new InvalidOperationException();
      }
    }

    public static void SetValue(this MemberInfo member, object instance, object value)
    {
      switch (member.MemberType)
      {
        case MemberTypes.Field:
          ((FieldInfo) member).SetValue(instance, value);
          break;
        case MemberTypes.Property:
          ((PropertyInfo) member).SetValue(instance, value, null);
          break;
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
