using System.Collections.Generic;

namespace EasyDapper.Core
{
  public abstract class WhereClauseGroupBase
  {
    public WhereClauseGroupBase()
    {
      Groups = new List<WhereClauseGroupBase>();
      Conditions = new List<WhereClauseConditionBase>();
    }

    public WhereClauseGroupBase Parent { get; set; }

    public WhereClauseGroupType GroupType { get; set; }

    public IList<WhereClauseGroupBase> Groups { get; set; }

    public IList<WhereClauseConditionBase> Conditions { get; set; }
  }
}
