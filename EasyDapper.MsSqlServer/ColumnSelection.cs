// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.ColumnSelection
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

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