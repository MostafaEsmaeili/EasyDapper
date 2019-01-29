using System;
using System.Reflection;

namespace EasyDapper.Abstractions
{
  public interface IWritablePropertyMatcher
  {
    bool Test(Type type);

    bool TestIsDbField(PropertyInfo propertyInfo);
  }
}
