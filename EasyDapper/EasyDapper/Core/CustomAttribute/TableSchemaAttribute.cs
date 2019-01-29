using System;

namespace EasyDapper.Core.CustomAttribute
{
  [AttributeUsage(AttributeTargets.Class)]
  public class TableSchemaAttribute : Attribute
  {
    private string tableSchema;

    public TableSchemaAttribute(string tableschema)
    {
      tableSchema = tableschema;
    }

    public string TableSchema
    {
      get
      {
        return tableSchema;
      }
    }
  }
}
