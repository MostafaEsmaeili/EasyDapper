// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.WhereClauseCondition
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
    public class WhereClauseCondition : WhereClauseConditionBase
    {
        public override string ToString()
        {
            var str1 = LocigalOperator == LogicalOperator.NotSet
                ? string.Empty
                : LocigalOperator.ToString().ToUpperInvariant();
            var empty = string.Empty;
            if (Left == "_LambdaTree_")
            {
                string str2;
                if (!string.IsNullOrWhiteSpace(Alias))
                    str2 = "[" + Alias + "].";
                else
                    str2 = "[" + LeftSchema + "].[" + LeftTable + "].";
                var str3 = Right.Replace("_table_Alias_", str2 ?? "");
                return (str1 + " " + str3 + " ").Trim();
            }

            string str4;
            if (!string.IsNullOrWhiteSpace(Alias))
                str4 = "[" + Alias + "].[" + Left + "]";
            else
                str4 = "[" + LeftSchema + "].[" + LeftTable + "].[" + Left + "]";
            var str5 = str4;
            return (str1 + " " + str5 + " " + Operator + " " + Right).Trim();
        }
    }
}