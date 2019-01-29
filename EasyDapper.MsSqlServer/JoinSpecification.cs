// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.JoinSpecification
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
  public class JoinSpecification : JoinSpecificationBase
  {
    public override string ToString()
    {
      string str1;
      if (!string.IsNullOrEmpty(LeftTableAlias))
        str1 = "[" + LeftTableAlias + "]";
      else
        str1 = "[" + LeftSchema + "].[" + LeftTableName + "]";
      var str2 = str1;
      string str3;
      if (!string.IsNullOrEmpty(RightTableAlias))
        str3 = "[" + RightTableAlias + "]";
      else
        str3 = "[" + RightSchema + "].[" + RightTableName + "]";
      var str4 = str3;
      return "\n" + GetPrefix() + " " + str2 + ".[" + LeftIdentifier + "] " + Operator + " " + str4 + ".[" + RightIdentifier + "]";
    }
  }
}
