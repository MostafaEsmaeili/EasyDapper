using System;
using System.Linq;
using System.Reflection;
using EasyDapper.Abstractions;
using EasyDapper.Core.CustomAttribute;

namespace EasyDapper
{
    public class WritablePropertyMatcher : IWritablePropertyMatcher
    {
        private readonly Type[] additionalTypes;

        public WritablePropertyMatcher()
        {
            var typeArray = new Type[21]
            {
                typeof(System.Enum),
                typeof(string),
                typeof(bool),
                typeof(byte),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(byte[]),
                typeof(long),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(sbyte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(Guid),
                typeof(object),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan)
            };
            var second = typeArray.Where(t => t.GetTypeInfo().IsValueType)
                .Select(t => typeof(Nullable<>).MakeGenericType(t));
            additionalTypes = typeArray.Concat(second).ToArray();
        }

        public bool Test(Type type)
        {
            if (type.GetTypeInfo().IsValueType || additionalTypes.Any(x => x.IsAssignableFrom(type)))
                return true;
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null && underlyingType.GetTypeInfo().IsEnum;
        }

        public bool TestIsDbField(PropertyInfo propertyInfo)
        {
            return Test(propertyInfo.PropertyType) && propertyInfo.IsDbField() && !propertyInfo.IsNonDbField();
        }
    }
}