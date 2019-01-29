// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.SelectStatementHavingSpecification
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
  public class SelectStatementHavingSpecification : SelectStatementHavingSpecificationBase
  {
    public override string ToString()
    {
      string str;
      if (!string.IsNullOrEmpty(Alias))
        str = "[" + Alias + "]";
      else
        str = "[" + Schema + "].[" + TableName + "]";
      return string.Format("{0} {1} {2}", ApplyAggregation(str + ".[" + Identifier + "]"), Operator, Value);
    }
  }
}
