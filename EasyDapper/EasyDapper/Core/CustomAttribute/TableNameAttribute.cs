using System;

namespace EasyDapper.Core.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string tablename)
        {
            TableName = tablename;
            ExcludeIdKey = false;
        }

        public TableNameAttribute(string tablename, bool excludeIdKey)
        {
            TableName = tablename;
            ExcludeIdKey = excludeIdKey;
        }

        public bool ExcludeIdKey { get; }

        public string TableName { get; }
    }
}