namespace EasyDapper.Core
{
  public abstract class JoinConditionBase
  {
    protected const string Template = "{0} {1} {2} {3}";

    public string LeftIdentifier { get; set; }

    public string LeftTableAlias { get; set; }

    public string LeftTableName { get; set; }

    public string LeftTableSchema { get; set; }

    public LogicalOperator LogicalOperator { get; set; }

    public string Operator { get; set; }

    public string RightIdentifier { get; set; }

    public string RightTableAlias { get; set; }

    public string RightTableName { get; set; }

    public string RightTableSchema { get; set; }
  }
}
