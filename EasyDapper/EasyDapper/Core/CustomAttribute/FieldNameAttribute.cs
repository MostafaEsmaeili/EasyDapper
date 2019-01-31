using System;

namespace EasyDapper.Core.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FieldNameAttribute : Attribute
    {
        public FieldNameAttribute(string fieldname)
        {
            Name = fieldname;
        }

        public string Name { get; }
    }
}