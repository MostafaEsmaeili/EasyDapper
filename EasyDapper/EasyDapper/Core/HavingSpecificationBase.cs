namespace EasyDapper.Core
{
  public abstract class HavingSpecificationBase
  {
    public Aggregation Aggregation { get; set; }

    public string Alias { get; set; }

    public Comparison Comparison { get; set; }

    public string Name { get; set; }

    public string Schema { get; set; }

    public string Table { get; set; }

    public object Value { get; set; }

    protected string ApplyAggregation(string columnExpression)
    {
      if (Aggregation == Aggregation.Count && Name == "*")
        return "COUNT(*)";
      return Aggregation.ToString().ToUpperInvariant() + "(" + columnExpression + ")";
    }

    protected string ComparisonExpression()
    {
      return string.Format("{0} {1}", GetOperatorString(), Value);
    }

    protected string GetOperatorString()
    {
      switch (Comparison)
      {
        case Comparison.Equal:
          return "=";
        case Comparison.NotEqual:
          return "<>";
        case Comparison.LessThan:
          return "<";
        case Comparison.GreaterThan:
          return ">";
        case Comparison.Like:
          return "LIKE";
        case Comparison.NotLike:
          return "NOT LIKE";
        default:
          return null;
      }
    }
  }
}
