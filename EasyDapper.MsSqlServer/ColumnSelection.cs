using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
    public class ColumnSelection : ColumnSelectionBase
    {
        public string Schema { get; set; }

        public override string ToString()
        {
            string str1;
            if (!string.IsNullOrWhiteSpace(Alias))
                str1 = "[" + Alias + "].";
            else
                str1 = "[" + Schema + "].[" + Table + "].";
            var str2 = str1;
            var columnExpression = Name == "*" ? str2 + "*" : str2 + "[" + Name + "]";
            return Aggregation == Aggregation.None ? columnExpression : ApplyAggregation(columnExpression);
        }

        private string ApplyAggregation(string columnExpression)
        {
            if (Aggregation == Aggregation.Count && Name == "*")
                return "COUNT(*)";
            return Aggregation.ToString().ToUpperInvariant() + "(" + columnExpression + ") AS [" + Name + "]";
        }
    }
}