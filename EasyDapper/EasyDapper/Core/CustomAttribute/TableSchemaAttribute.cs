using System;

namespace EasyDapper.Core.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableSchemaAttribute : Attribute
    {
        public TableSchemaAttribute(string tableschema)
        {
            TableSchema = tableschema;
        }

        public string TableSchema { get; }
    }
}