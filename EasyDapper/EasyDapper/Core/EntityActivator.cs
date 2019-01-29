using System;
using System.Linq.Expressions;

namespace EasyDapper.Core
{
  public static class EntityActivator
  {
    public static EntityActivator<T> GetActivator<T>()
    {
      return (EntityActivator<T>) Expression.Lambda(typeof (EntityActivator<T>), Expression.New(typeof (T).GetConstructor(Type.EmptyTypes)), Array.Empty<ParameterExpression>()).Compile();
    }
  }
}
