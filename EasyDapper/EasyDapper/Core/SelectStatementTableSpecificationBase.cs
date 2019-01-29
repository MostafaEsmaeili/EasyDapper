using System;

namespace EasyDapper.Core
{
  public abstract class SelectStatementTableSpecificationBase
  {
    public string Alias { get; set; }

    public Type EntityType { get; set; }

    public JoinType JoinType { get; set; }

    public bool NoLocks { get; set; }

    public string TableName { get; set; }

    public string Schema { get; set; }

    protected string GetPrefix()
    {
      switch (JoinType)
      {
        case JoinType.Inner:
          return "INNER JOIN";
        case JoinType.LeftOuter:
          return "LEFT OUTER JOIN";
        case JoinType.RightOuter:
          return "RIGHT OUTER JOIN";
        case JoinType.Full:
          return "FULL JOIN";
        case JoinType.Cross:
          return "CROSS JOIN";
        default:
          return "FROM";
      }
    }
  }
}
