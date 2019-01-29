// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.WhereClauseCondition
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
  public class WhereClauseCondition : WhereClauseConditionBase
  {
    public override string ToString()
    {
      if (Left == "_LambdaTree_")
      {
        string str;
        if (!string.IsNullOrWhiteSpace(Alias))
          str = "[" + Alias + "].";
        else
          str = "[" + LeftSchema + "].[" + LeftTable + "].";
        return Right.Replace("_table_Alias_", str ?? "");
      }
      var str1 = LocigalOperator == LogicalOperator.NotSet ? string.Empty : LocigalOperator.ToString().ToUpperInvariant();
      string str2;
      if (!string.IsNullOrWhiteSpace(Alias))
        str2 = "[" + Alias + "].[" + Left + "]";
      else
        str2 = "[" + LeftSchema + "].[" + LeftTable + "].[" + Left + "]";
      var str3 = str2;
      return (str1 + " " + str3 + " " + Operator + " " + Right).Trim();
    }
  }
}
