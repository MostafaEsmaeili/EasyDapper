// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.FilterCondition
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
    public class FilterCondition : FilterConditionBase
    {
        public override string ToString()
        {
            var str1 = LocigalOperator == LogicalOperator.NotSet
                ? string.Empty
                : LocigalOperator.ToString().ToUpperInvariant() + " ";
            string str2;
            if (!string.IsNullOrEmpty(Alias))
                str2 = "[" + Alias + "]";
            else
                str2 = "[" + Schema + "].[" + TableName + "]";
            var str3 = str2;
            if (Left == "_LambdaTree_")
                return str1 + LambdaTree.Replace("_table_Alias_", str3 + ".");
            if (Right != "NULL") return str1 + "(" + str3 + ".[" + Left + "] " + Operator + " " + Right + ")";
            switch (Operator)
            {
                case "=":
                    Operator = "IS";
                    break;
                case "<>":
                    Operator = "IS NOT";
                    break;
            }

            return str1 + "(" + str3 + ".[" + Left + "] " + Operator + " " + Right + ")";
        }
    }
}