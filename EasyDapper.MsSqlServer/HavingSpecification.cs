// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.HavingSpecification
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
  internal class HavingSpecification : HavingSpecificationBase
  {
    public override string ToString()
    {
      string str1;
      if (!string.IsNullOrWhiteSpace(Alias))
        str1 = "[" + Alias + "].";
      else
        str1 = "[" + Schema + "].[" + Table + "].";
      var str2 = str1;
      return ApplyAggregation(Name == "*" ? str2 + "*" : str2 + "[" + Name + "]") + " " + ComparisonExpression();
    }
  }
}
