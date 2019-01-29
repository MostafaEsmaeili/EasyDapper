using System;

namespace EasyDapper.Core.CustomAttribute
{
  [AttributeUsage(AttributeTargets.Class)]
  public class TableNameAttribute : Attribute
  {
    private string tableName;

    public TableNameAttribute(string tablename)
    {
      tableName = tablename;
    }

    public string TableName
    {
      get
      {
        return tableName;
      }
    }
  }
}
