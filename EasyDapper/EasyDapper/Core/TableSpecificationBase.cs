using System;
using System.Collections.Generic;

namespace EasyDapper.Core
{
  public abstract class TableSpecificationBase
  {
    protected const string Template = "{0} [{1}].[{2}]{3}{4}";

    public TableSpecificationBase()
    {
      Conditions = new List<JoinConditionBase>();
    }

    public IList<JoinConditionBase> Conditions { get; }

    public string LeftAlias { get; set; }

    public string LeftSchema { get; set; }

    public string LeftTable { get; set; }

    public Type LeftType { get; set; }

    public string RightAlias { get; set; }

    public string RightSchema { get; set; }

    public string RightTable { get; set; }

    public Type RightType { get; set; }

    public string SpecificationType { get; set; }
  }
}
