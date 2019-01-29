using System;
using System.Linq;

namespace EasyDapper
{
  public static class TypeExtensions
  {
    public static bool IsSimpleType(this Type type)
    {
      int num;
      if (!type.IsPrimitive)
      {
        if (!new Type[2]
        {
            typeof (string),
            typeof (Decimal)
        }.Contains(type))
        {
          num = Convert.GetTypeCode(type) != TypeCode.Object ? 1 : 0;
          goto label_4;
        }
      }
      num = 1;
label_4:
      return num != 0;
    }
  }
}
