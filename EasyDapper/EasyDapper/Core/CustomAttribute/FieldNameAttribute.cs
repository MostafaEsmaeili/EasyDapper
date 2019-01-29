using System;

namespace EasyDapper.Core.CustomAttribute
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class FieldNameAttribute : Attribute
  {
    private string _fieldname;

    public FieldNameAttribute(string fieldname)
    {
      _fieldname = fieldname;
    }

    public string Name
    {
      get
      {
        return _fieldname;
      }
    }
  }
}
