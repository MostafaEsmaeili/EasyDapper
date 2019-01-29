// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.JoinCondition
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
  internal class JoinCondition : JoinConditionBase
  {
    public override string ToString()
    {
      var str1 = LogicalOperator == LogicalOperator.NotSet ? "ON" : LogicalOperator.ToString().ToUpperInvariant();
      string str2;
      if (!string.IsNullOrWhiteSpace(LeftTableAlias))
        str2 = "[" + LeftTableAlias + "].[" + LeftIdentifier + "]";
      else
        str2 = "[" + LeftTableSchema + "].[" + LeftTableName + "].[" + LeftIdentifier + "]";
      var str3 = str2;
      string str4;
      if (!string.IsNullOrWhiteSpace(RightTableAlias))
        str4 = "[" + RightTableAlias + "].[" + RightIdentifier + "]";
      else
        str4 = "[" + RightTableSchema + "].[" + RightTableName + "].[" + RightIdentifier + "]";
      var str5 = str4;
      return string.Format("{0} {1} {2} {3}", (object) str1, (object) str3, (object) Operator, (object) str5);
    }
  }
}
