using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
  public class ColumnSpecification : ColumnSpecificationBase
  {
    public override string ToString()
    {
      string str1;
      if (!string.IsNullOrWhiteSpace(Alias))
        str1 = "[" + Alias + "].";
      else
        str1 = "[" + Schema + "].[" + Table + "].";
      var str2 = str1;
      var str3 = str2 + "[" + Identifier + "]";
      if (ColumnName != Identifier)
        str3 = str2 + "[" + ColumnName + "] as [" + Identifier + "]";
      var columnExpression = Identifier == "*" ? str2 + "*" : str3;
      return Aggregation == Aggregation.None ? columnExpression : ApplyAggregation(columnExpression);
    }

    protected string ApplyAggregation(string columnExpression)
    {
      if (Aggregation == Aggregation.Count && Identifier == "*")
        return "COUNT(*) AS [Count]";
      return Aggregation.ToString().ToUpperInvariant() + "(" + columnExpression + ") AS [" + AggregationColumnName + "]";
    }
  }
}
